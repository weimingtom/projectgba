/*
 * Created by SharpDevelop.
 * User: Spikeman
 * Date: 1/10/2007
 * Time: 7:08 PM
 * 
 * Todo:
 * - Need to convert [num] to PC relative offset (subtract 12(?) because PC is always ahead)
 *   - Need to subtract?
 * - Need to convert offsets on branches to pc relative (must be halfword aligned, so #>>1)
 *   - Maybe give conditional branches own opcode type to shift them
 * - Convert BL into two opcodes
 * - Convert add rx,pc/sp,# to add rx,[pc/sp,#] or rearrange all the argument types so the regex will work :P
 * - Support for =address
 * - Add ~ (not) and - (unary minus) to expression evaluation
 * - Make precidence matter in expressions evaluation
 * - All error handling
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace ProjectGBA
{

	public partial class MainForm : Form
	{
		
		#region Defines
		
		char[] whitespace = new char[] {' ', '	'};
		char[] comment = new char[] {';'};
		
		Dictionary<string, string> defines = new Dictionary<string, string>();
		
		#endregion
		
		#region Generated Stuff
		
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		public MainForm()
		{

			InitializeComponent();
			
		}
		
		#endregion
		
		#region Parsing and Expressions
		
		int parseInt(string str) {
			string[] prefixes = new string[] {"0x","#0x","$","%"};	//prefixes
			int[] bases = new int[] {16, 16, 16, 2};				//corresponding bases
			for(int i = 0; i < prefixes.Length; ++i) {
				if(str.StartsWith(prefixes[i]))
					return Convert.ToInt32(str.Substring(prefixes[i].Length), bases[i]);
			}
			string[] suffixes = new string[] {"h","d","b"};		//suffixes
			bases = new int[] {16, 10, 2};						//corresponding bases
			for(int i = 0; i < suffixes.Length; ++i) {
				if(str.EndsWith(suffixes[i]))
					return Convert.ToInt32(str.Substring(0,str.LastIndexOf(suffixes[i])), bases[i]);
			}
			return Convert.ToInt32(str, 10); //if no prefix or suffix, assume decimal
		}
		
		bool canBeParsed(string str) {
			try {
				parseInt(str);
				return true;
			} catch {
				return false;
			}
		}
		
		string eval(string exp) {
			//needs precidence
			//need to evaluate ~ and unary -
			try {
				return Convert.ToString(parseInt(exp)); //return parsed number if possible
			} catch {
				List<string> nums = new List<string>();
				List<string> ops = new List<string>();
				nums.AddRange(exp.Split(new string[] {"+","-","*","/",">>","<<","&","|","^","?"},
				                        StringSplitOptions.None));
				ops.AddRange(Regex.Split(exp, @"\d+")); //get list of operators
				ops.RemoveAt(0);
				ops.RemoveAt(ops.Count - 1);
				while(ops.Count > 0) {
					int result = 0;
					int num0 = Convert.ToInt32(nums[0], 10);
					int num1 = Convert.ToInt32(nums[1], 10);
					switch(ops[0]) {
						case "+":
							result = num0 + num1;
							break;
						case "-":
							result = num0 - num1;
							break;
						case "/":
							result = num0 / num1;
							break;
						case "*":
							result = num0 * num1;
							break;
						case ">>":
							result = num0 >> num1;
							break;
						case "<<":
							result = num0 << num1;
							break;
						case "&":
							result = num0 & num1;
							break;
						case "|":
							result = num0 | num1;
							break;
						case "^":
							result = num0 ^ num1;
							break;
						case "?":							//mod?
							result = num0 % num1;
							break;
					}
					nums.RemoveAt(0);
					nums[0] = Convert.ToString(result);	//replace first two with result
					ops.RemoveAt(0);
				}
				return nums[0];
			}
		}
		
		string evalExp(string str) {
			MatchEvaluator matchEval = new MatchEvaluator(this.matchEval);
			//regular expression finds number, matchEval puts them into decimal
			string decstr = Regex.Replace(str, @"#{0,1}(0x|\$|%){0,1}[0-9a-fA-F]+[hdb]{0,1}", matchEval);
			int idx;
			while((idx=decstr.IndexOf(')')) != -1) {
				int idx2 = decstr.LastIndexOf('(', idx);
				decstr = decstr.Substring(0, idx2) + eval(decstr.Substring(idx2 + 1, idx - idx2 - 1))
					+ decstr.Substring(idx + 1);
			}
			return eval(decstr);
		}
		
		string matchEval(Match m) {
			return  Convert.ToString(parseInt(m.Value));
		}
		
		#endregion
		
		#region Assembling Functions
		
		int rStr2Int(string rx, bool allowHighRegisters) {
			int rstr = Convert.ToInt32(rx.Substring(1)); //check for error
			//don't need to check if lower than 0, regex wouldn't get it
			if(rstr < (allowHighRegisters ? 16 : 8)) {
				return rstr;
			} else {
				System.Diagnostics.Trace.WriteLine("Error: Invalid register");
				return -1;
			}
		}
		
		int rList2Int(string rl) {
			string[] rs = rl.Split(new char[] {','});
			int ret = 0;
			for(int i = 0; i < rs.Length; i++) {
				Match m = Regex.Match(rs[i], @"r(\d)-r(\d)");
				if(m.Success) {
					int r1 = Convert.ToInt32(m.Groups[1].Value);
					int r2 = Convert.ToInt32(m.Groups[2].Value);
					for(int j = r1; j <= r2; j++) {
						ret |= 1 << j;
					}
				} else {
					ret |= 1 << Convert.ToInt32(rs[i].Substring(1));
				}
			}
			
			return ret;
		}
		
		#endregion
		
		void BAssembleClick(object sender, EventArgs e)
		{
			int address = 0;			//keeps track of address, etc.
			
			string[] lines = tCode.Text.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
			ArrayList nextLines = new ArrayList();
			
			// The first pass(es) processes whitespace, comments, and all defines, labels, etc.
			#region First Pass
			
			bool needsToBeParsed; //keep track if everything is parsed
			int numPasses = 0;		//for debug, counts passes
			do {
				needsToBeParsed = false;
				foreach(string line in lines) {
					bool addLine = true;	//keeps track of if to keep line (for removing defines and labels)
					string tmp = line;
					foreach(KeyValuePair<string, string> kvp in defines) {
						tmp = tmp.Replace(kvp.Key, kvp.Value);	//replace defines
					}
					int c = tmp.IndexOfAny(comment);		//only accepts chars, so can't do C-style comments like this
					if(c != -1)
						tmp = tmp.Substring(0, c);			//strip comments
					tmp = tmp.Trim();						//defaults to stripping whitespace
					tmp = Regex.Replace(tmp, @"\s+", " ");	//replace long whitespace to single space
					tmp = Regex.Replace(tmp, @"\s*,\s*", ","); //remove space around commas (for splitting)
					if(tmp != "") {
						if(tmp.StartsWith("@")) {			//directive
							string[] args = tmp.Split(new char[] {' '});
							switch(args[0]) {
								case "@arm":
									System.Diagnostics.Trace.WriteLine("== ARM Mode ==");
									break;
								case "@dcd":
									System.Diagnostics.Trace.WriteLine("Data: " + args[1]);
									if(!canBeParsed(args[1]))
										needsToBeParsed = true;
									address += 4;
									break;
								case "@define":
									System.Diagnostics.Trace.WriteLine("Define Added: " + args[1] + " = " + args[2]);
									defines.Add(args[1], args[2]);
									addLine = false;
									break;
								case "@echo":
									string[] strs = args[1].Split(new char[] {','});
									foreach(string str in strs) {
										System.Diagnostics.Trace.WriteLine("Echo:" + str);
									}
									break;
								case "@endarea":
									System.Diagnostics.Trace.WriteLine("== End Area ==");
									break;
								case "@textarea":
									System.Diagnostics.Trace.WriteLine("== Area: " + args[1] + " ==");
									address = parseInt(args[1]);
									break;
								case "@thumb":
									System.Diagnostics.Trace.WriteLine("== THUMB Mode ==");
									break;
								default:
									System.Diagnostics.Trace.WriteLine("Directive: " + args[0].Substring(1));
									break;
							}
						} else {
							string[] args = tmp.Split(new char[] {' '});
							if(args.Length > 1) {
								System.Diagnostics.Trace.WriteLine("OPCODE: " + args[0]);
								address += 2; //each opcode is 2 bytes? (except for bl?)
							} else {
								string addressStr = "0x" + Convert.ToString(address, 16);
								System.Diagnostics.Trace.WriteLine("Label: " + args[0] + " = " + addressStr);
								defines.Add(args[0], addressStr);
								addLine = false;
							}
						}
					} else {
						addLine = false; //remove empty lines
					}
					if(addLine)
						nextLines.Add(tmp);
				}
				lines = new string[nextLines.Count];
				nextLines.CopyTo(lines);
				nextLines.Clear();
				++numPasses;
				System.Diagnostics.Trace.WriteLine("\n==== End of Pass " + numPasses.ToString() + " ====\n");
			} while(needsToBeParsed);
			
			#endregion
			
			// The second pass evaluates expressions in arguments
			#region Second Pass
			
			foreach(string line in lines) {
				string tmp = line;
				foreach(KeyValuePair<string, string> kvp in defines) {
						tmp = tmp.Replace(kvp.Key, kvp.Value);	//replace defines
				}
				if(!tmp.StartsWith("@")) {
				   	string[] parts = tmp.Split(' ');
					string[] args = Regex.Split(parts[1], @"(?![\[\{]{1}[^,]*),(?![^,\[\{]*[\]\}]{1})"); //match commas not inside braces
					for(int i = 0; i < args.Length; ++i) {
						if(args[i].StartsWith("[")) {
							string[] subargs = args[i].Substring(1, args[1].Length - 2).Split(',');
							
							for(int j = 0; j < subargs.Length; ++j) {
								//System.Diagnostics.Trace.WriteLine(subargs[j]);
								if(!Regex.IsMatch(subargs[j], @"r{1}\d{1,2}"))
									subargs[j] = evalExp(subargs[j]);
							}
							args[i] = "[" + String.Join(",",subargs) + "]";
						}
						if(!Regex.IsMatch(args[i], @"r{1}\d{1,2}")) //only if not r0-15
						   args[i] = evalExp(args[i]);
					}
					parts[1] = String.Join(",",args);
					tmp = String.Join(" ", parts);
				}
				nextLines.Add(tmp);
			}
			nextLines.CopyTo(lines);
			nextLines.Clear();
			
			#endregion
			
			tCode.Text = String.Join(Environment.NewLine, lines);
			
			#region Final Pass
			
			
			
			#endregion
			
		}
		
		void BDebugClick(object sender, EventArgs e)
		{
			FileStream fs = new FileStream(Application.StartupPath + "\\opcodes.txt", FileMode.Open);
			TextReader sr = new StreamReader(fs);
			Dictionary<string,int[]> opcodes = new Dictionary<string,int[]>();
			
			string str="";
			while(str != null) {
				str = sr.ReadLine();
				if(str != null) {
					string[] parts = str.Split(' ');
					opcodes.Add((parts[0] + parts[1]).ToLower(), new int[] {Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3], 16), (parts.Length > 4) ? Convert.ToInt32(parts[4], 16) : 0});
				}
			}
			
			string[] argTypes = new string[] {
				@"^(r13,){2}(?<num>\d*)$",
				@"^(?<rx>r{1}\d{1,2}),(?<ry>r{1}\d{1,2}),(?<rz>r{1}\d{1,2})$",
				@"^(?<rx>r{1}\d{1,2}),(?<ry>r{1}\d{1,2}),(?<num>\d*)$",
				@"^(?<rx>r{1}\d{1,2}),(?<ry>r{1}\d{1,2})$",
				@"^(?<rx>r{1}\d{1,2}),(?<num>\d*)$",
				@"^(?<rx>r{1}\d{1,2})$",
				@"^(?<rx>r{1}\d{1,2}),\[(r15){1},(?<num>\d*)]$",
				@"^(?<rx>r{1}\d{1,2}),\[(r13){1},(?<num>\d*)]$",
				@"^(?<rx>r{1}\d{1,2}),\[(?<ry>r{1}\d{1,2}),(?<rz>r{1}\d{1,2})]$",
				@"^(?<rx>r{1}\d{1,2}),\[(?<ry>r{1}\d{1,2}),(?<num>\d*)]$",
				@"^{(?<rl>(?:r\d[,-]*)+)?,?(?<rx>r14|r15)?}$",
				@"^(?<rx>r{1}\d{1,2})!,{(?<rl>[^}]+)}$",
				@"^(?<num>\d*)$"
			};
			
			string test = "add r0,r1";
			
			string[] pieces = test.Split(' ');
			
			for(int i = 0; i < argTypes.Length; ++i) {
				Match m = Regex.Match(pieces[1], argTypes[i]);
				if(m.Success) {
					string key = pieces[0].ToLower() + i.ToString();
					int opType = opcodes[key][0];
					int opBase = opcodes[key][1];
					int altOp = opcodes[key][2];
					int rx, ry, rz, rl, num, opcode, hh;
					switch(opType) {
						case 0:
							rx = rStr2Int(m.Groups["rx"].Value, false);
							ry = rStr2Int(m.Groups["ry"].Value, false);
							num = Convert.ToInt32(m.Groups["num"].Value) & 0x1f; //bitmask to 5 bits
							opcode = opBase | rx | (ry << 3) | (num << 6);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 1:
							rx = rStr2Int(m.Groups["rx"].Value, false);
							ry = rStr2Int(m.Groups["ry"].Value, false);
							num = Convert.ToInt32(m.Groups["num"].Value) & 0xf; //bitmask to 4 bits
							opcode = opBase | rx | (ry << 3) | (num << 6);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 2:
							rx = rStr2Int(m.Groups["rx"].Value, false);
							ry = rStr2Int(m.Groups["ry"].Value, false);
							num = Convert.ToInt32(m.Groups["num"].Value) & 0x07; //bitmask to 3 bits
							opcode = opBase | rx | (ry << 3) | (num << 6);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 3:
							rx = rStr2Int(m.Groups["rx"].Value, false);
							ry = rStr2Int(m.Groups["ry"].Value, false);
							rz = rStr2Int(m.Groups["rz"].Value, false);
							opcode = opBase | rx | (ry << 3) | (rz << 6);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 4:
							rx = rStr2Int(m.Groups["rx"].Value, true);
							ry = rStr2Int(m.Groups["ry"].Value, true);
							hh = ((rx & 8) >> 2) | (ry >> 3);
							rx = rx & 7;
							ry = ry & 7;
							if(hh != 0) {
								opcode = opBase | rx | (ry << 3) | (hh << 6);
							} else {
								opcode = altOp | rx | (ry << 3);
							}
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 5:
							rx = rStr2Int(m.Groups["rx"].Value, true);
							opcode = opBase | (rx << 3);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 6:
							num = ((Convert.ToInt32(m.Groups["num"].Value) >> 2)) & 0x7f;
							opcode = opBase | num;
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 7:
							rx = rStr2Int(m.Groups["rx"].Value, false);
							num = Convert.ToInt32(m.Groups["num"].Value) & 0xff; //bitmask to 8 bits
							opcode = opBase | num | (rx << 8);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 8:
							rx = rStr2Int(m.Groups["rx"].Value, false);
							ry = rStr2Int(m.Groups["ry"].Value, false);
							opcode = opBase | rx | (ry << 3);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 9:
							rx = rStr2Int(m.Groups["rx"].Value, false);
							num = ((Convert.ToInt32(m.Groups["num"].Value) >> 2)) & 0xff; //bitmask to 8 bits
							opcode = opBase | num | (rx << 8);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 10:
							rx = rStr2Int(m.Groups["rx"].Value, false);
							ry = rStr2Int(m.Groups["ry"].Value, false);
							num = (Convert.ToInt32(m.Groups["num"].Value) >> 2) & 0x1f; //bitmask to 5 bits
							opcode = opBase | rx | (ry << 3) | (num << 6);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 11:
							rx = m.Groups["rx"].Success ? rStr2Int(m.Groups["rx"].Value, true) : 0; //lr or pc
							rl = m.Groups["rl"].Success ? rList2Int(m.Groups["rl"].Value) : 0;
							int bit = ((rx == 15 && pieces[0].ToLower() == "pop") || (rx == 14 && pieces[0].ToLower() == "push")) ? 1 : 0;
							opcode = opBase | rl | (bit << 8);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 12:
							rx = rStr2Int(m.Groups["rx"].Value, false);
							rl = rList2Int(m.Groups["rl"].Value);
							opcode = opBase | rl | (rx << 8);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 13:
							num = Convert.ToInt32(m.Groups["num"].Value) & 0xff; //bitmask to 8 bits
							opcode = opBase | num;
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 14: //branches
							num = Convert.ToInt32(m.Groups["num"].Value) & 0x7ff; //bitmask to 11 bits
							opcode = opBase | num;
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						case 15: //halfword things
							rx = rStr2Int(m.Groups["rx"].Value, false);
							ry = rStr2Int(m.Groups["ry"].Value, false);
							num = (Convert.ToInt32(m.Groups["num"].Value) >> 1) & 0x1f; //bitmask to 5 bits
							opcode = opBase | rx | (ry << 3) | (num << 6);
							System.Diagnostics.Trace.WriteLine(Convert.ToString(opcode, 16));
							break;
						
					}
					break;
				} 
				
			}
			
			sr.Dispose();
			fs.Dispose();
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			{	//everything has these defines
				defines.Add("pc","r15");
				defines.Add("lr","r14");
				defines.Add("sp","r13");
			}
		}
	}
}
