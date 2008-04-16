/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/13/2008
 * Time: 5:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace pGBA
{
	/// <summary>
	/// Description of Thumb.
	/// </summary>
	public class Thumb
	{
		private const int COND_EQ = 0;	    // Z set
        private const int COND_NE = 1;	    // Z clear
        private const int COND_CS = 2;	    // C set
        private const int COND_CC = 3;	    // C clear
        private const int COND_MI = 4;	    // N set
        private const int COND_PL = 5;	    // N clear
        private const int COND_VS = 6;	    // V set
        private const int COND_VC = 7;	    // V clear
        private const int COND_HI = 8;	    // C set and Z clear
        private const int COND_LS = 9;	    // C clear or Z set
        private const int COND_GE = 10;	    // N equals V
        private const int COND_LT = 11;	    // N not equal to V
        private const int COND_GT = 12; 	// Z clear AND (N equals V)
        private const int COND_LE = 13; 	// Z set OR (N not equal to V)
        private const int COND_AL = 14; 	// Always
        private const int COND_NV = 15; 	// Never execute

        private const int OP_AND = 0x0;
        private const int OP_EOR = 0x1;
        private const int OP_LSL = 0x2;
        private const int OP_LSR = 0x3;
        private const int OP_ASR = 0x4;
        private const int OP_ADC = 0x5;
        private const int OP_SBC = 0x6;
        private const int OP_ROR = 0x7;
        private const int OP_TST = 0x8;
        private const int OP_NEG = 0x9;
        private const int OP_CMP = 0xA;
        private const int OP_CMN = 0xB;
        private const int OP_ORR = 0xC;
        private const int OP_MUL = 0xD;
        private const int OP_BIC = 0xE;
        private const int OP_MVN = 0xF;
        
		private Engine myEngine;
		private ushort	opcode, cycles;
		private uint zero, carry, negative, overflow;
		
		public Thumb(Engine engine)
		{
			myEngine = engine;
		}
		
		#region Overflow Handlers
        public void OverflowCarryAdd(uint a, uint b, uint r)
        {
            overflow = ((a & b & ~r) | (~a & ~b & r)) >> 31;
            carry = ((a & b) | (a & ~r) | (b & ~r)) >> 31;
        }

        public void OverflowCarrySub(uint a, uint b, uint r)
        {
            overflow = ((a & ~b & ~r) | (~a & b & r)) >> 31;
            carry = ((a & ~b) | (a & ~r) | (~b & ~r)) >> 31;
        }
        #endregion
		
        #region Opcodes
		void thumb_lsl_imm()
		{
			// 0x00 - 0x07
            // lsl rd, rm, #immed
			int rd = opcode & 0x7;
            int rm = (opcode >> 3) & 0x7;
            int immed = (opcode >> 6) & 0x1F;

            if (immed == 0)
            {
                myEngine.myCPU.Registers[rd] = myEngine.myCPU.Registers[rm];
            } else
            {
                carry = (myEngine.myCPU.Registers[rm] >> (32 - immed)) & 0x1;
                myEngine.myCPU.Registers[rd] = myEngine.myCPU.Registers[rm] << immed;
            }

            negative = myEngine.myCPU.Registers[rd] >> Armcpu.N_BIT;
            zero = myEngine.myCPU.Registers[rd] == 0 ? 1U : 0U;

			cycles = 1;
		}
		#endregion
		
		private void PackFlags()
        {
			myEngine.myCPU.Registers[16] &= 0x0FFFFFFF;
            myEngine.myCPU.Registers[16] |= negative << Armcpu.N_BIT;
            myEngine.myCPU.Registers[16] |= zero << Armcpu.Z_BIT;
            myEngine.myCPU.Registers[16] |= carry << Armcpu.C_BIT;
            myEngine.myCPU.Registers[16] |= overflow << Armcpu.V_BIT;
        }

        private void UnpackFlags()
        {
            negative = (myEngine.myCPU.Registers[16] >> Armcpu.N_BIT) & 1;
            zero = (myEngine.myCPU.Registers[16] >> Armcpu.Z_BIT) & 1;
            carry = (myEngine.myCPU.Registers[16] >> Armcpu.C_BIT) & 1;
            overflow = (myEngine.myCPU.Registers[16] >> Armcpu.V_BIT) & 1;
        }
		
		public uint Emulate()
		{
			byte	opcode_11_5, opcode_6_5, opcode_9_3;
			uint 	address=myEngine.myCPU.Registers[15];
			
			cycles		= 1; //Only here incase i forget to set the cycles for an opcode and it creates an infinite loop
			opcode		= 0x0088; //myEngine.myMemory.ReadShort(address);
			opcode_11_5	= (byte)((opcode >> 11) & 0x11);	/*11-15 5bit*/
			opcode_6_5	= (byte)((opcode >> 6) & 0x1F);
			opcode_9_3	= (byte)((opcode >> 9) & 0x07);
			
			UnpackFlags();
			
			switch(opcode_11_5){
			case 0x00:	/*00000*/
				thumb_lsl_imm();/*LSL Rd,Rs,#Offset5 - 1*/
				break;
			case 0x01:	/*00001*/
				//thumb_lsr_imm();/*LSR Rd,Rs,#Offset5 - 1*/
				break;
			case 0x02:	/*00010*/
				//thumb_asr_imm();/*ASR Rd,Rs,#Offset5 - 1*/
				break;
			case 0x03:	/*00011*/
				//if((bool)((opcode >> 9)&0x01)){
				//	arm7tdmi_thumb_sub();/*Œ¸ŽZ - 2*/
				//}else{
				//	arm7tdmi_thumb_add();/*‰ÁŽZ - 2*/
				//}
				break;
			case 0x04:	/*00100*/
				//arm7tdmi_thumb_mov();/*MOV Rd,#Offset8 ˆÚ“® - 3*/
				break;
			case 0x05:	/*00101*/
				//arm7tdmi_thumb_cmp();/*CMP Rd,#Offset8 ”äŠr - 3*/
				break;
			case 0x06:	/*00110*/
				//arm7tdmi_thumb_add_imm();/*ADD Rd,#Offset8 ‰ÁŽZ - 3*/
				break;
			case 0x07:	/*00111*/
				//arm7tdmi_thumb_sub_imm();/*SUB Rd,#Offset8 Œ¸ŽZ - 3*/
				break;
			case 0x08:	/*01000*/
				switch(opcode_6_5){
				case 0x00:	/*HiƒŒƒWƒXƒ^‘€ì/•ªŠòƒXƒe[ƒg*/
				case 0x01:
				case 0x02:
					//arm7tdmi_thumb_add_hi();	/*HiƒŒƒWƒXƒ^‘€ì - 5*/
					break;
				case 0x03:
				case 0x04:
				case 0x05:
					//arm7tdmi_thumb_cmp_hi();	/*HiƒŒƒWƒXƒ^‘€ì - 5*/
					break;
				case 0x06:
				case 0x07:
				case 0x08:
					//arm7tdmi_thumb_mov_hi();	/*HiƒŒƒWƒXƒ^‘€ì - 5*/
					break;
				case 0x09:
				case 0x0a:
				case 0x0b:
					//arm7tdmi_thumb_bx_hi();	/*•ªŠò‚ÆƒXƒe[ƒg•ÏX - 5*/
					break;
				case 0x10:	/*ALU‰‰ŽZ - 4*/
					//arm7tdmi_thumb_and();
					break;
				case 0x11:
					//arm7tdmi_thumb_eor();
					break;
				case 0x12:
					//arm7tdmi_thumb_lsl();
					break;
				case 0x13:
					//arm7tdmi_thumb_lsr();
					break;
				case 0x14:
					//arm7tdmi_thumb_asr();
					break;
				case 0x15:
					//arm7tdmi_thumb_adc();
					break;
				case 0x16:
					//arm7tdmi_thumb_sbc();
					break;
				case 0x17:
					//arm7tdmi_thumb_ror();
					break;
				case 0x18:
					//arm7tdmi_thumb_tst();
					break;
				case 0x19:
					//arm7tdmi_thumb_neg();
					break;
				case 0x1A:
					//arm7tdmi_thumb_cmp();
					break;
				case 0x1B:
					//arm7tdmi_thumb_cmn();
					break;
				case 0x1C:
					//arm7tdmi_thumb_orr();
					break;
				case 0x1D:
					//arm7tdmi_thumb_mul();
					break;
				case 0x1E:
					//arm7tdmi_thumb_bic();
					break;
				case 0x1F:
					//arm7tdmi_thumb_mvn();
					break;
				}
				break;
			case 0x09:	/*01001*/
				//arm7tdmi_thumb_ldr_pc();	/*PC‘Š‘Îƒ[ƒh - 6*/
				break;
			case 0x0A:	/*01010*/
			case 0x0B:	/*01011*/
				switch(opcode_9_3){
				/*ƒŒƒWƒXƒ^ƒIƒtƒZƒbƒg‚É‚æ‚éƒ[ƒh/ƒXƒgƒA - 7*/
				case 0x0:	/*000 LB0*/
					//arm7tdmi_thumb_str();
					break;
				case 0x2:	/*010 LB0*/
					//arm7tdmi_thumb_strb();
					break;
				case 0x4:	/*100 LB0*/
					//arm7tdmi_thumb_ldr();
					break;
				case 0x6:	/*110 LB0*/
					//arm7tdmi_thumb_ldrb();
					break;
				/*ƒoƒCƒg^ƒn[ƒtƒ[ƒh‚Ìƒ[ƒh^ƒXƒgƒA‚Æ•„†Šg’£ - 8*/
				case 0x1:	/*000 HS0*/
					//arm7tdmi_thumb_strh();
					break;
				case 0x3:	/*010 HS0*/
					//arm7tdmi_thumb_ldsb();
					break;
				case 0x5:	/*100 HS0*/
					//arm7tdmi_thumb_ldrh();
					break;
				case 0x7:	/*110 HS0*/
					//arm7tdmi_thumb_ldsh();
					break;
				}
				break;	
			/*ƒCƒ~ƒfƒBƒGƒCƒgƒIƒtƒZƒbƒg‚É‚æ‚éƒ[ƒh^ƒXƒgƒA - 9*/
			case 0x0C:	/*01100 - BL=00*/
				//arm7tdmi_thumb_str_imm();	/*str rd,[rb,#imm]*/
				break;
			case 0x0D:	/*01101 - BL=01*/
				//arm7tdmi_thumb_ldr_imm();	/*ldr rd,[rb,#imm]*/
				break;
			case 0x0E:	/*01110 - BL=10*/
				//arm7tdmi_thumb_strb_imm();/*strb rd,[rb,#imm]*/
				break;
			case 0x0F:	/*01111 - BL=11*/
				//arm7tdmi_thumb_ldrb_imm();/*ldrb rd,[rb,#imm]*/
				break;
			/*ƒn[ƒtƒ[ƒh‚Ìƒ[ƒh^ƒXƒgƒA - 10*/
			case 0x10:	/*10000 - L=0*/
				//arm7tdmi_thumb_strh_imm();/*strh rd,[rb,#imm]*/
				break;
			case 0x11:	/*10001 - L=1*/
				//arm7tdmi_thumb_ldrh_imm();/*ldrh rd,[rb,#imm]*/
				break;
			/*SP‘Š‘Îƒ[ƒh^ƒXƒgƒA - 11*/
			case 0x12:	/*10010 - S=0*/
				//arm7tdmi_thumb_str_sp();/*str rd,[SP,#imm]*/
				break;
			case 0x13:	/*10011 - S=1*/
				//arm7tdmi_thumb_ldr_sp();/*ldr rd,[SP,#imm]*/
				break;
			/*ƒAƒhƒŒƒX‚Ìƒ[ƒh - 12*/
			case 0x14:	/*10100 - S=0*//*add rd,PC,#imm*/
			case 0x15:	/*10101 - S=1*//*add rd,SP,#imm*/
				//arm7tdmi_thumb_add_adr();
				break;
			case 0x16:	/*10110*/
			case 0x17:	/*10111*/
				//if(BIT_N(opcode,10)){
			/*ƒŒƒWƒXƒ^‚ÌPUSH/POP - 14*/
				//	if(BIT_N(opcode,11)){	/*L*/
				//		arm7tdmi_thumb_pop();/*POP {Rlist}*/
				//	}else{
				//		arm7tdmi_thumb_push();/*PUSH {Rlist}*/
				//	}
				//}else{
			/*ƒXƒ^ƒbƒNƒ|ƒCƒ“ƒ^‚ÉƒIƒtƒZƒbƒg‚ð‰ÁŽZ - 13*/
				//	if(!((opcode >> 8) & 0x7)){	/*000S*/
				//		arm7tdmi_thumb_add_sp();/*add SP,#+-imm*/
				//	}
				//	break;
				//}
				break;
			/*•¡”ƒŒƒWƒXƒ^‚Ìƒ[ƒh^ƒXƒgƒA - 15*/
			case 0x18:	/*11000*/
				//arm7tdmi_thumb_stmia();/*stmia rb!,{Rlist}*/
				break;
			case 0x19:	/*11001*/
				//arm7tdmi_thumb_ldmia();/*ldmia rb!,{Rlist}*/
				break;
			case 0x1A:	/*11010*/
			case 0x1B:	/*11011*/
				//if(((opcode >> 8) & 0xF) == 0xF){
			/*ƒ\ƒtƒgƒEƒFƒAŠ„‚èž‚Ý - 17*/
				//	arm7tdmi_thumb_swi();
				//}else{
			/*ðŒ•ªŠò - 16*/
				//	arm7tdmi_thumb_bxx();
				//}
				break;
			/*–³ðŒ•ªŠò - 18*/
			case 0x1C:	/*11100*/
				//arm7tdmi_thumb_b();
				break;
			/*’·‹——£•ªŠò‚ÆƒŠƒ“ƒN - 19*/
			case 0x1E:	/*11110*/
			case 0x1F:	/*11111*/
				//arm7tdmi_thumb_bl();
				break;
			default:
				break;
			}
			
			PackFlags();
			
			myEngine.myCPU.Registers[15] += 2;
			
			return cycles;
		}
	}
}
