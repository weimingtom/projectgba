/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/16/2008
 * Time: 5:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace pGBA
{
	/// <summary>
	/// Description of Disassembler.
	/// </summary>
	public class Disassembler
	{
		private Engine myEngine;
		private ushort opcode;
		
		public Disassembler(Engine engine)
		{
			myEngine = engine;
		}
		
		#region Thumb
		
		public string DisasmThumb(uint adr)
		{
			string str;
			byte	opcode_11_5, opcode_6_5, opcode_9_3;
			
			opcode		= myEngine.myMemory.ReadShort(adr);
			opcode_11_5	= (byte)((opcode >> 11) & 0x1F);	/*11-15 5bit*/
			opcode_6_5	= (byte)((opcode >> 6) & 0x1F); 	/*6-10 5bit*/
			opcode_9_3	= (byte)((opcode >> 9) & 0x07);		/*9-11 3bit*/
			
			str = Convert.ToString(adr,16).PadLeft(8,'0') + " ".PadRight(8) + Convert.ToString(opcode,16).PadLeft(4,'0') + " ".PadRight(8);
			
			//MessageBox.Show("opcode_11_5 = 0x" + Convert.ToString(opcode_11_5,16).PadLeft(2,'0') + "(" + Convert.ToString(opcode,16).PadLeft(4,'0') +")");
			
			switch(opcode_11_5){
			case 0x00:	/*00000*/
				str += String.Format("lsl r{0:d}, r{1:d}, #0x{2:x2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x1F));
				break;
			case 0x01:	/*00001*/
				str += String.Format("lsr r{0:d}, r{1:d}, #0x{2:x2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x1F));
				break;
			case 0x02:	/*00010*/
				str += String.Format("asr r{0:d}, r{1:d}, #0x{2:x2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x1F));
				break;
			case 0x03:	/*00011*/
				switch((opcode >> 9)&3)
				{
					case 0: 
						str += String.Format("add r{0:d}, r{1:d}, r{2:d}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x07));
						break;
					case 1:
						str += String.Format("sub r{0:d}, r{1:d}, r{2:d}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x07));
						break;
					case 2: 
						str += String.Format("add r{0:d}, r{1:d}, #0x{2:x2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x07));
						break;
					case 3: 
						str += String.Format("sub r{0:d}, r{1:d}, #0x{2:x2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x07));
						break;
				}
				break;
			case 0x04:	/*00100*/
				str += String.Format("mov r{0:d}, #0x{1:x2}", ((opcode >> 8) & 0x07), (opcode & 0xFF));
				break;
			case 0x05:	/*00101*/
				str += String.Format("cmp r{0:d}, #0x{1:x2}", ((opcode >> 8) & 0x07), (opcode & 0xFF));
				break;
			case 0x06:	/*00110*/
				str += String.Format("add r{0:d}, #0x{1:x2}", ((opcode >> 8) & 0x07), (opcode & 0xFF));
				break;
			case 0x07:	/*00111*/
				str += String.Format("sub r{0:d}, #0x{1:x2}", ((opcode >> 8) & 0x07), (opcode & 0xFF));
				break;
				
			default: 
				str +=  "Unknown";
				break;
			}
			
			return str;
		}
		
		#endregion
		
		#region Arm
		
		#endregion
	}
}
