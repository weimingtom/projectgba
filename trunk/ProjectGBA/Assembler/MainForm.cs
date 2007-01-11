/*
 * Created by SharpDevelop.
 * User: Spikeman
 * Date: 1/10/2007
 * Time: 7:08 PM
 * 
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ProjectGBA
{

	public partial class MainForm : Form
	{
		
		#region Defines
		
		char[] whitespace = new char[] {' ', '	'};
		char[] comment = new char[] {';'};
		
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
		
		void BAssembleClick(object sender, EventArgs e)
		{
			int address = 0;			//keeps track of address, etc.
			
			string[] lines = tCode.Text.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
			foreach(string line in lines) {
				string tmp = line;
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
							switch(args[0]) {
								default:
									System.Diagnostics.Trace.WriteLine("OPCODE: " + args[0]);
									break;
							}
							address += 2; //each opcode is 2 bytes? (except for bl?)
						} else {
							System.Diagnostics.Trace.WriteLine("Label: " + args[0] + " = 0x" + Convert.ToString(address, 16));
						}
					}
				}
			}
		}
		
	}
}
