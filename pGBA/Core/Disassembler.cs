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
		private uint[] registers;
		
		private string[] aluOps = new string[16]
            {
               "and","eor","lsl","lsr","asr","adc","sbc","ror","tst","neg","cmp","cmn","orr","mul","bic","mvn"
            };
			
		private string[] opCond = new string[]
            {
	            "eq","ne","cs","cc","mi","pl","vs","vc","hi","ls","ge","lt","gt","le","","nv"
            };
		
		public Disassembler(Engine engine)
		{
			myEngine = engine;
			
			registers = engine.myCPU.Registers;
		}
		
		#region Thumb
		public string ThumbRegList(ushort opcode, int type)
		{
			string bleh = "";
			int start = -1;
			int end = -1;
			
			for (int i = 0; i < 9; i++)
            {
				if ((((opcode >> i) & 1) == 1) && i!=8)
                {
                    if(start == -1) start = i;
                } 
                else 
                {
                	if (start != -1)
                	{
                		end = i - 1;
                		if(start == end)
                			bleh += String.Format("r{0:d},", start);
                		else
                			bleh += String.Format("r{0:d}-r{1:d},", start, end);
                	
                		start = -1;
                	}
                }
            }
			
			if(((opcode >> 8) & 1) == 1) 
			{
				switch(type)
				{
					case 1:
						bleh += "pc";
						break;
					case 2:
						bleh += "lr";
						break;
					default:
						break;
				}
			}
			
			if (bleh.Length == 0) return "";
			if (bleh[bleh.Length - 1] == ',') bleh = bleh.Remove(bleh.Length - 1);
			return bleh;
		}
		public string DisasmThumb(uint adr)
		{
			string 	str;
			uint 	offset;
			ushort	opcode;
			byte	opcode_11_5, opcode_6_5, opcode_9_3;
			
			opcode		= myEngine.myMemory.ReadShort(adr);
			opcode_11_5	= (byte)((opcode >> 11) & 0x1F);	/*11-15 5bit*/
			opcode_6_5	= (byte)((opcode >> 6) & 0x1F); 	/*6-10 5bit*/
			opcode_9_3	= (byte)((opcode >> 9) & 0x07);		/*9-11 3bit*/
			
			str = Convert.ToString(adr,16).PadLeft(8,'0') + " ".PadRight(2) + Convert.ToString(opcode,16).PadLeft(4,'0') + " ".PadRight(2);
			str = str.ToUpper();
			
			//MessageBox.Show("opcode_11_5 = 0x" + Convert.ToString(opcode_11_5,16).PadLeft(2,'0') + "(" + Convert.ToString(opcode,16).PadLeft(4,'0') +")");
			
			switch(opcode_11_5){
			case 0x00:	/*00000*/
				str += String.Format("lsl r{0:d}, r{1:d}, #0x{2:X2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x1F));
				break;
			case 0x01:	/*00001*/
				str += String.Format("lsr r{0:d}, r{1:d}, #0x{2:X2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x1F));
				break;
			case 0x02:	/*00010*/
				str += String.Format("asr r{0:d}, r{1:d}, #0x{2:X2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x1F));
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
						str += String.Format("add r{0:d}, r{1:d}, #0x{2:X2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x07));
						break;
					case 3: 
						str += String.Format("sub r{0:d}, r{1:d}, #0x{2:X2}", (opcode & 0x7), ((opcode >> 3) & 0x7), ((opcode >> 6) & 0x07));
						break;
				}
				break;
			case 0x04:	/*00100*/
				str += String.Format("mov r{0:d}, #0x{1:X2}", ((opcode >> 8) & 0x07), (opcode & 0xFF));
				break;
			case 0x05:	/*00101*/
				str += String.Format("cmp r{0:d}, #0x{1:X2}", ((opcode >> 8) & 0x07), (opcode & 0xFF));
				break;
			case 0x06:	/*00110*/
				str += String.Format("add r{0:d}, #0x{1:X2}", ((opcode >> 8) & 0x07), (opcode & 0xFF));
				break;
			case 0x07:	/*00111*/
				str += String.Format("sub r{0:d}, #0x{1:X2}", ((opcode >> 8) & 0x07), (opcode & 0xFF));
				break;
			case 0x08:	/*01000*/
				if(((opcode >> 10)&1)==0)
				{
					str += String.Format("{0:s} r{1:d}, r{2:d}", aluOps[((opcode >> 6)&0x0F)], (opcode & 0x7), ((opcode >> 3) & 0x7));
				}
				else
				{
					switch(((opcode >> 8)&3))
					{
						case 0:
							str += String.Format("addhi r{0:d}, r{1:d}", (opcode & 0x7), ((opcode >> 3) & 0x7));
							break;
						case 1:
							str += String.Format("cmphi r{0:d}, r{1:d}", (opcode & 0x7), ((opcode >> 3) & 0x7));
							break;
						case 2:
							if(((opcode & 0x7)==0x08) && (((opcode >> 3) & 0x7)==0x08))
							{
								str += "nop";
							}
							else
							{
								str += String.Format("movhi r{0:d}, r{1:d}", (opcode & 0x7), ((opcode >> 3) & 0x7));
							}
							break;
						case 3:
							str += String.Format("bx r{0:d}", ((opcode >> 3) & 0x7));
							break;
					}
				}
				break;
			case 0x09:	/*01001*/
			str += String.Format("ldr r{0:d},[pc,#0x{1:X2}]", ((opcode>>8) & 0x7), ((opcode & 0xFF)*4));
				break;
			case 0x0A:	/*01010*/
			case 0x0B:	/*01011*/
				switch(opcode_9_3){
				case 0x0:
					str += String.Format("str r{0:d},[r{1:d},r{2:d}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x07));
					break;
				case 0x2:
					str += String.Format("strb r{0:d},[r{1:d},r{2:d}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x07));
					break;
				case 0x4:
					str += String.Format("ldr r{0:d},[r{1:d},r{2:d}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x07));
					break;
				case 0x6:
					str += String.Format("ldrb r{0:d},[r{1:d},r{2:d}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x07));
					break;
				case 0x1:
					str += String.Format("strh r{0:d},[r{1:d},r{2:d}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x07));
					break;
				case 0x3:
					str += String.Format("ldsb r{0:d},[r{1:d},r{2:d}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x07));
					break;
				case 0x5:
					str += String.Format("ldrh r{0:d},[r{1:d},r{2:d}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x07));
					break;
				case 0x7:
					str += String.Format("ldsh r{0:d},[r{1:d},r{2:d}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x07));
					break;
				}
				break;	
			case 0x0C:	/*01100*/
				str += String.Format("str r{0:d},[r{1:d},#0x{2:X2}]", (opcode & 0x07), ((opcode>>3) & 0x07), (((opcode>>6) & 0x1F) * 4));
				break;
			case 0x0D:	/*01101*/
				str += String.Format("ldr r{0:d},[r{1:d},#0x{2:X2}]", (opcode & 0x07), ((opcode>>3) & 0x07), (((opcode>>6) & 0x1F) * 4));
				break;
			case 0x0E:	/*01110*/
				str += String.Format("strb r{0:d},[r{1:d},#0x{2:X2}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x1F));
				break;
			case 0x0F:	/*01111*/
				str += String.Format("ldrb r{0:d},[r{1:d},#0x{2:X2}]", (opcode & 0x07), ((opcode>>3) & 0x07), ((opcode>>6) & 0x1F));
				break;
			case 0x10:	/*10000*/
				str += String.Format("strh r{0:d},[r{1:d},#0x{2:X2}]", (opcode & 0x07), ((opcode>>3) & 0x07), (((opcode>>6) & 0x1F) * 2));
				break;
			case 0x11:	/*10001*/
				str += String.Format("ldrh r{0:d},[r{1:d},#0x{2:X2}]", (opcode & 0x07), ((opcode>>3) & 0x07), (((opcode>>6) & 0x1F) * 2));
				break;
			case 0x12:	/*10010*/
				str += String.Format("str r{0:d},[sp,#0x{1:X2}]", ((opcode>>8) & 0x07), (((opcode>>6) & 0xFF) * 4));
				break;
			case 0x13:	/*10011*/
				str += String.Format("ldr r{0:d},[sp,#0x{1:X2}]", ((opcode>>8) & 0x07), (((opcode>>6) & 0xFF) * 4));
				break;
			case 0x14:	/*10100*/
				str += String.Format("add r{0:d},pc,#0x{1:X2}", ((opcode>>8) & 0x07), (((opcode>>6) & 0xFF) * 4));
				break;
			case 0x15:	/*10101*/
				str += String.Format("add r{0:d},sp,#0x{1:X2}", ((opcode>>8) & 0x07), (((opcode>>6) & 0xFF) * 4));
				break;
			case 0x16:	/*10110*/
			case 0x17:	/*10111*/
				if(((opcode>>10)&1)==1)
				{
					if(((opcode>>11)&1)==1)
					{
						str += String.Format("pop {{{0}}}", ThumbRegList(opcode,1));
					}
					else
					{
						str += String.Format("push {{{0}}}", ThumbRegList(opcode,2));
					}
				}
				else
				{
					if(((opcode >> 8) & 0x7)==0) /*000S*/
					{
						if(((opcode >> 7)&1) == 0)
						{
							str += String.Format("add sp,#0x{1:X2}", ((opcode & 0x7F) * 4));
						}
						else
						{
							str += String.Format("add sp,#-0x{1:X2}", ((opcode & 0x7F) * 4));
						}
					}
				}
				break;	
			case 0x18:	/*11000*/
				str += String.Format("stmia r{0}!, {{{1}}}", ((opcode >> 8) & 0x7), ThumbRegList(opcode,0));
				break;
			case 0x19:	/*11001*/
				str += String.Format("ldmia r{0}!, {{{1}}}", ((opcode >> 8) & 0x7), ThumbRegList(opcode,0));
				break;
			case 0x1A:	/*11010*/
			case 0x1B:	/*11011*/
				if(((opcode >> 8) & 0xF) == 0xF){
					str += String.Format("swi #0x{0:X}", (opcode & 0xFF));
				}else{
					offset = (uint)(opcode & 0xFF);
                	if ((offset & 0x80) != 0) offset |= 0xFFFFFF00;
                    str += String.Format("b{0} #0x{1:X8}", opCond[(opcode >> 8) & 0xF], adr + 4U + (offset << 1));
				}
				break;
			case 0x1C:	/*11100*/
				offset = (uint)(opcode & 0x7FF);
                if ((offset & (1 << 10)) != 0) offset |= 0xFFFFF800;
                str += String.Format("b #0x{0:X8}", adr + 4U + (offset << 1));
				break;
			case 0x1E:	/*11110*/
				offset = (uint)(opcode & 0x7FF);
                if ((offset & (1 << 10)) != 0) offset |= 0xFFFFF800;
                offset = (uint)(offset << 12) | (uint)((myEngine.myMemory.ReadShort(adr + 2U) & 0x7FF) << 1);
                str += String.Format("bl #0x{0:X8}", adr + 4U + offset);
				break;
			case 0x1F:	/*11111*/
				str += String.Format("bl part2???");
				break;
			default: 
				str +=  "Unknown";
				break;
			}
			
			return str;
		}
		
		#endregion
		
		#region Arm
		public string DisasmArm(uint adr)
		{
			string 	str;
			uint 	opcode;
			uint 	offset;
			byte	opcode_24_4;
			
			opcode		= myEngine.myMemory.ReadWord(adr);
			opcode_24_4	= (byte)((opcode >> 24) & 0x0F);	/*24-27 4bit*/
			
			str = Convert.ToString(adr,16).PadLeft(8,'0') + " ".PadRight(2) + Convert.ToString(opcode,16).PadLeft(8,'0') + " ".PadRight(2);
			str = str.ToUpper();
			
			adr += 4;
			
			switch (opcode_24_4)
            {
				/*Branch*/
				case 0xA:	/*1010*/
					offset = opcode & 0x00FFFFFF;
		            if (offset >> 23 == 1) offset |= 0xFF000000;
                    str += String.Format("b{0} #0x{1:X8}", opCond[(opcode >> 28) & 0xF], adr + 4U + (offset << 2));
					break;
				/*Branch with link*/
				case 0xB:	/*1011*/
					offset = opcode & 0x00FFFFFF;
		            if (offset >> 23 == 1) offset |= 0xFF000000;
                    str += String.Format("bl{0} #0x{1:X8}", opCond[(opcode >> 28) & 0xF], adr + 4U + (offset << 2));
					break;
			}
			
			return str;
		}
		#endregion
	}
}
