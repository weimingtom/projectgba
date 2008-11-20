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
							str += String.Format("add sp,#0x{0:X}", ((opcode & 0x7F) * 4));
						}
						else
						{
							str += String.Format("add sp,#-0x{0:X}", ((opcode & 0x7F) * 4));
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
		
		 #region Shift helpers
        //SHIFT_ROR
        public string imm_rotate(uint opcode)
		{
			uint	imm_val;
			int 	shift;
			string 	str = "";
				
			shift	= (int)(((opcode >> 8) & 0xF)*2);
			imm_val	= opcode & 0xFF;
            imm_val = (imm_val >> shift) | (imm_val << (32 - shift));

            str = String.Format("#0x{0:X}",imm_val);
            
            return str;
		}
        
        //Barrel Shifter
        private const byte SHIFT_LSL = 0;
        private const byte SHIFT_LSR = 1;
        private const byte SHIFT_ASR = 2;
        private const byte SHIFT_ROR = 3;
        
        public string imm_shift(uint opcode)
		{
			byte	shift_type;
			int 	shift;
			uint 	rm;
			int 	valueSigned;
			string 	str = "";
				
			rm = registers[opcode & 0x0F];
			shift_type = (byte)((opcode >> 5) & 3);
			
			if((opcode & 0x08)!=0)
			{
				shift = (int)((opcode >> 8) & 0xF);
				if (shift == 15)
                {
                    shift = (int)((registers[shift] + 0x4) & 0xFF);
                }
                else
                {
                    shift = (int)(registers[shift] & 0xFF);
                }

                if ((opcode & 0xF) == 15)
                {
                    rm += 4;
                }
			}
			else
			{
				shift = (int)((opcode >> 7) & 0x1F);
			}
            
			if(rm == 15) 
			{
				switch(shift_type)
				{
					case SHIFT_LSL:
						rm = (rm << shift);
						break;
					case SHIFT_LSR:
						if (shift!=0) 
						{
							rm = (rm >> shift);
						} 
						break;
					case SHIFT_ASR:
						if (shift!=0) 
						{
						//ASR #32 for c  = 0					//Get last bit shifted
							valueSigned = (int)rm;			//Convert value to signed
							rm = (uint)(valueSigned >> shift);
						}
						break;
					case SHIFT_ROR:
						if (shift!=0) 
						{
							//ROR
							rm = (rm << (32 - shift)) | (rm >> shift);						
						} 
						else 
						{		
							//RRX		
							//if (carry!=0) 
							//{
								rm = ((rm>>1)|0x80000000);
							//} else {
							//	rm = rm >> 1;
							//}
						}
						break;
				}
				str = String.Format("#0x{0:d}",rm);
			} 
			else
			{
				if(shift>0)
				{
					switch(shift_type)
					{
						case 0:	/*00 = logical left(lsl) */
							str = String.Format("r{0:d},lsl #{1:d}",opcode & 0x0F, shift);
							break;
						case 1:	/*01 = logical right(lsr) */
							str = String.Format("r{0:d},lsr #{1:d}",opcode & 0x0F, shift);
							break;
						case 2:	/*10 = arithmetic right(asr) */
							str = String.Format("r{0:d},asr #{1:d}",opcode & 0x0F, shift);
							break;
						case 3:	/*11 = rotate right(ror) */
							str = String.Format("r{0:d},ror #{1:d}",opcode & 0x0F, shift);
							break;
					}
				}
				else
				{
					str = String.Format("r{0:d}",opcode & 0x0F);
				}
			}
			
            return str;
		}
        
        private bool BIT_N(uint a, int b)
		{
			uint test = ((a>>b)&1);
			return (bool)(test==1) ? true : false;
		}
        
        #endregion
		
		private string arm_loadstore(uint opcode, bool load)
		{
			string str = null;
			
			uint rd = 0;
        	uint rn = 0;
        	uint offset = 0;
        	uint address = 0;
        	uint fimmed = 0;
        	uint fpreindex = 0;
        	uint faddbase = 0;
        	uint fbyte = 0;
        	uint fwriteback = 0;
        	uint amount = 0;
        	
        	rd = (opcode >> 12) & 0x0F;
        	rn = (opcode >> 16) & 0x0F;
        	
        	fimmed = 	((opcode >> 25) & 1);
        	fpreindex = ((opcode >> 24) & 1);
        	faddbase = 	((opcode >> 23) & 1);
        	fbyte = 	((opcode >> 22) & 1);
        	fwriteback =((opcode >> 21) & 1);
        	
        	address = registers[rn];
        	
        	if(rn==15) address += 4;
			
			if(load)
			{
				str += String.Format("ldr{0} ",opCond[opcode>>28]);
			}
			else
			{
				str += String.Format("str{0} ",opCond[opcode>>28]);
			}
			
			str += String.Format("r{0:d}",rd);
			
			if(fimmed!=0)
        	{
				//offset = myEngine.myCPU.myArm.imm_shift(opcode);
        	}
        	else
        	{
				offset = opcode & 0xFFF;
			}
        	
        	amount = address + offset;
        	
        	str += String.Format(", [#0x{0:X8}]",amount+8);
			
			
		
			return str;
		}
		
		private string arm_dataproc(uint opcode)
		{
        	string operand = null;
        	uint rd = 0;
        	uint rn = 0;
        	
        	rd = (opcode >> 12) & 0x0F;
        	rn = (opcode >> 16) & 0x0F;
        		
        	if(BIT_N(opcode,25)){	/*Immediate Operand*/
				operand = imm_rotate(opcode);
			}else{
				operand = imm_shift(opcode);
			}
        	
        	switch((byte)((opcode >> 21) & 0x0F)){
				case 0x0: 
        			/*and*/
        			return String.Format("and{0}{1} r{2:d},r{3:d},{4}", opCond[(opcode >> 28) & 0xF], (((opcode>>20)&1)==1)?"s":"", rd, rn, operand);
				case 0x1: 
        			/*eor*/ 
        			return String.Format("eor{0}{1} r{2:d},r{3:d},{4}", opCond[(opcode >> 28) & 0xF], (((opcode>>20)&1)==1)?"s":"", rd, rn, operand);
				case 0x2: 
        			/*sub*/
        			return String.Format("sub{0}{1} r{2:d},r{3:d},{4}", opCond[(opcode >> 28) & 0xF], (((opcode>>20)&1)==1)?"s":"", rd, rn, operand);
				case 0x3: 
        			/*rsb*/ 
        			return String.Format("rsb{0}{1} r{2:d},r{3:d},{4}", opCond[(opcode >> 28) & 0xF], (((opcode>>20)&1)==1)?"s":"", rd, rn, operand);
				case 0x4: 
        			/*add*/ 
        			return String.Format("add{0}{1} r{2:d},r{3:d},{4}", opCond[(opcode >> 28) & 0xF], (((opcode>>20)&1)==1)?"s":"", rd, rn, operand);
				case 0x5: 
        			/*adc*/ 
        			return String.Format("adc{0}{1} r{2:d},r{3:d},{4}", opCond[(opcode >> 28) & 0xF], (((opcode>>20)&1)==1)?"s":"", rd, rn, operand);
				case 0x6: 
        			/*sbc*/ 
        			return String.Format("sbc{0}{1} r{2:d},r{3:d},{4}", opCond[(opcode >> 28) & 0xF], (((opcode>>20)&1)==1)?"s":"", rd, rn, operand);
				case 0x7: 
        			/*rsc*/ 
        			return String.Format("rsc{0}{1} r{2:d},r{3:d},{4}", opCond[(opcode >> 28) & 0xF], (((opcode>>20)&1)==1)?"s":"", rd, rn, operand);
				case 0x8: 
        			/*tst*/ 
        			return String.Format("tst{0} r{1:d},{2}", opCond[(opcode >> 28) & 0xF], rn, operand);
				case 0x9: 
        			/*teq*/ 
        			return String.Format("teq{0} r{1:d},{2}", opCond[(opcode >> 28) & 0xF], rn, operand);
				case 0xA: 
        			/*cmp*/ 
        			return String.Format("cmp{0} r{1:d},{2}", opCond[(opcode >> 28) & 0xF], rn, operand);
				case 0xB: 
        			/*cmn*/ 
        			return String.Format("cmn{0} r{1:d},{2}", opCond[(opcode >> 28) & 0xF], rn, operand);
				case 0xC: 
        			/*orr*/
        			return String.Format("orr{0} r{1:d},r{2:d},{3}", opCond[(opcode >> 28) & 0xF], rd, rn, operand);
				case 0xD: 
        			/*mov*/
        			return String.Format("mov{0} r{1:d},{2}", opCond[(opcode >> 28) & 0xF], rd, operand);
				case 0xE: 
        			/*bic*/
        			return String.Format("bic{0} r{1:d},r{2:d},{3}", opCond[(opcode >> 28) & 0xF], rd, rn, operand);
				case 0xF: 
        			/*mvn*/ 
        			return String.Format("mvn{0} r{1:d},{2}", opCond[(opcode >> 28) & 0xF], rd, operand);
			}
        	
        	return "error";
		}
		        
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
				case 0x1:	/*0001*/
					if(((opcode >> 4) & 0xFF) == 0x09)
					{	
						/*00001001*/
						/*Single Data Swap*/
						str += String.Format("swp{0}{1} r{2:d},r{3:d},r{4:d}", opCond[(opcode >> 28) & 0xF], (((opcode>>22)&1)==1)?"b":"", ((opcode>>12)&0x0F), (opcode&0x0F), ((opcode>>16)&0x0F));
						break;
					}
					if((opcode & 0x0FFFFFF0) == 0x012FFF10)
					{
						/*Branch and Exchange*/
						str += String.Format("bx{0} r{1:d}", opCond[(opcode >> 28) & 0xF], (opcode&0x0F));
						break;
					}
					if((opcode & 0x0FFF0000) == 0x016F0000)
					{
			        //  arm_clz();	/*Count Leading Zeros*/
						break;
			        }
					if((opcode&0xFFF00000)==0xE1000000){
		            	//arm_mrs();
		            	str += "mrs";
		            	break;
		            }
		            if((opcode&0xFFF00000)==0xE1200000){
		            	//arm_msr();
		            	str += "msr";
		            	break;
		            }
					if(BIT_N(opcode,4))
					{	
						/*Halfword data Transfer:*/
						if(BIT_N(opcode,22))
						{	
							/*immdiate offset*/
							if(BIT_N(opcode,20))
							{
			            		/*Load from memory*/
								//arm_ldrs_imm();
							}
			            	else
			            	{					
			            		/*Store to memory*/
								//arm_strs_imm();
							}
							break;
						}
						else
						{					
							/*register offset*/
							if(BIT_N(opcode,20))
							{	
								/*Load from memory*/
								//arm_ldrs();
							}
							else
							{					
								/*Store to memory*/
								//arm_strs();
							}
							break;
						}
					}
					
					/*Data Processing can be eitehr 0x1 or 0x3*/
					str += arm_dataproc(opcode);
					break;
	            case 0x2:	/*0010*/
				case 0x3:	/*0011*/
					/*Data Processing can be eitehr 0x1 or 0x3*/
					str += arm_dataproc(opcode);
					break;
				/*Single Data Transfer*/
				case 0x4:	/*0100*/
				case 0x5:	/*0101*/
				case 0x6:	/*0110*/
				case 0x7:	/*0111*/
					if(((opcode>>20)&1)==1)
					{	
						/*Load from memory*/
						str += arm_loadstore(opcode,true);
						break;
					}
					else if(((opcode>>20)&1)==0)
					{
						/*Store to memory*/
						str += arm_loadstore(opcode,false);
						break;
					}
					if(((opcode>>4)&1)==1)
					{
						str += String.Format("UNDEFINED OPCODE!");
						break;
					}
					break;
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
