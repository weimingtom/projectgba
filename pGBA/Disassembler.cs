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
		
		#region Opcodes
		string thumb_lsl_imm()
		{
			// 0x00 - 0x07
            // lsl rd, rm, #immed
			int rd = this.opcode & 0x7;
            int rm = (this.opcode >> 3) & 0x7;
            int immed = (this.opcode >> 6) & 0x1F;

            return String.Format("lsl r{0:d}, r{1:d}, #0x{2:x2}", rd, rm, immed);
		}
		#endregion
		
		public string DisasmThumb(uint adr)
		{
			string str;
			byte	opcode_11_5, opcode_6_5, opcode_9_3;
			
			opcode		= 0x0088; //myEngine.myMemory.ReadShort(adr);
			opcode_11_5	= (byte)((opcode >> 11) & 0x11);	/*11-15 5bit*/
			opcode_6_5	= (byte)((opcode >> 6) & 0x1F);
			opcode_9_3	= (byte)((opcode >> 9) & 0x07);
			
			str = Convert.ToString(adr,16).PadLeft(8,'0') + " ".PadRight(8) + Convert.ToString(opcode,16).PadLeft(4,'0') + " ".PadRight(8);
			
			switch(opcode_11_5){
			case 0x00:	/*00000*/
				str += thumb_lsl_imm(); /*LSL Rd,Rs,#Offset5 - 1*/
				break;
			case 0x01:	/*00001*/
				//thumb_lsr_imm();/*LSR Rd,Rs,#Offset5 - 1*/
				str +=  "Unknown";
				break;
			case 0x02:	/*00010*/
				//thumb_asr_imm();/*ASR Rd,Rs,#Offset5 - 1*/
				str +=  "Unknown";
				break;
			default: 
				str +=  "Unknown";
				break;
			}
			
			adr += 2;
			
			return str;
		}
		
		#endregion
		
		#region Arm
		
		#endregion
	}
}
