/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 6/8/2008
 * Time: 8:01 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace pGBA
{
    /// <summary>
    /// Description of Arm.
    /// </summary>
    public class Arm
    {
        private Engine myEngine;
        private uint opcode, opcodeQueue, cycles;
        private uint zero, carry, negative, overflow;
        private uint[] registers;
        private uint shifterCarry;

        public Arm(Engine engine)
        {
            myEngine = engine;
        }

        public void Begin()
        {
            registers = myEngine.myCPU.Registers;
            
            //Flush queue here because arm is always executed first
            FlushQueue();
        }
        
        #region Shift helpers
        //SHIFT_ROR
        uint imm_rotate(uint opcode)
		{
			uint	imm_val;
			int 	shift;
				
			shift	= (int)(((opcode >> 8) & 0xF)*2);
			imm_val	= opcode & 0xFF;

            imm_val = (imm_val >> shift) | (imm_val << (32 - shift));

            if (shift == 0)
            {
                shifterCarry = carry;
            }
            else
            {
                shifterCarry = (imm_val >> 31) & 1;
            }
            
            return imm_val;
		}
        #endregion
        
        #region Opcodes
        
        #region Section 4
        void arm_b()
		{
            uint offset = opcode & 0x00FFFFFF;
            if (offset >> 23 == 1) offset |= 0xFF000000;

            registers[15] += offset << 2;

            FlushQueue();
            
            cycles = 3;
		}
        void arm_bl()
		{
            uint offset = opcode & 0x00FFFFFF;
            if (offset >> 23 == 1) offset |= 0xFF000000;

            registers[14] = (registers[15] - 4U) & ~3U;
            registers[15] += offset << 2;

            FlushQueue();
            
            cycles = 3;
		}
        #endregion
        
        #region Section 5
        void arm_mov()
		{
        	uint operand = 0;
        	uint rd = 0;
        	
        	rd = ((opcode >> 12) & 0xF);
        		
        	if((opcode>>25&1)==1){	/*Immediate Operand*/
				operand = imm_rotate(opcode);
			}else{
				;//operand = imm_shift(opcode);
			}

            registers[rd] = operand;

            negative = registers[rd] >> 31;
            zero = registers[rd] == 0 ? 1U : 0U;
            carry = shifterCarry;
            
            cycles = 1;
		}
        #endregion
        
        #endregion
        
        #region Flag Handling
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
        #endregion

        public uint Emulate()
        {
            byte opcode_24_4;
            uint cond = 0;

            opcode = opcodeQueue;
            FlushQueue();
            
            opcode_24_4	= (byte)((opcode >> 24) & 0x0F);	/*24-27 4bit*/

            UnpackFlags();
           
            switch ((opcode >> 28) & 0xF)
            {
            	case Armcpu.COND_EQ:
            		cond = zero;
            		break;
            	case Armcpu.COND_NE:
            		cond = 1 - zero;
            		break;
            	case Armcpu.COND_CS:
            		cond = carry;
            		break;
            	case Armcpu.COND_CC:
            		cond = 1 - carry;
            		break;
            	case Armcpu.COND_MI:
            		cond = negative;
            		break;
            	case Armcpu.COND_PL:
            		cond = 1 - negative;
            		break;
            	case Armcpu.COND_VS:
            		cond = overflow;
            		break;
            	case Armcpu.COND_VC:
            		cond = 1 - overflow;
            		break;
            	case Armcpu.COND_HI:
            		cond = carry & (1 - zero);
            		break;
            	case Armcpu.COND_LS:
            		cond = (1 - carry) | zero;
            		break;
            	case Armcpu.COND_GE:
            		cond = (1 - negative) ^ overflow;
            		break;
            	case Armcpu.COND_LT:
            		cond = negative ^ overflow;
            		break;
            	case Armcpu.COND_GT: 
            		cond = (1 - zero) & (negative ^ (1 - overflow)); 
            		break;
                case Armcpu.COND_LE: 
            		cond = (negative ^ overflow) | zero; 
            		break;
            	default:
            		cond = 1;
            		break;
            }
            
            if (cond == 1)
            {
	            switch (opcode_24_4)
	            {
	            	/*Data Processing*/
	            	case 0x2:	/*0010*/
					case 0x3:	/*0011*/
				    if((opcode&0xFFF00000)==0xE1000000){
		            	//arm_mrs();
		            	break;
		            }
		            if((opcode&0xFFF00000)==0xE1200000){
		            	//arm_msr();
		            	break;
		            }
		            
					switch((byte)((opcode >> 21) & 0x0F)){
						case 0x0: /*arm_and();*/ break;
						case 0x1: /*arm_eor();*/ break;
						case 0x2: /*arm_sub();*/ break;
						case 0x3: /*arm_rsb();*/ break;
						case 0x4: /*arm_add();*/ break;
						case 0x5: /*arm_adc();*/ break;
						case 0x6: /*arm_sbc();*/ break;
						case 0x7: /*arm_rsc();*/ break;
						case 0x8: /*arm_tst();*/ break;
						case 0x9: /*arm_teq();*/ break;
						case 0xA: /*arm_cmp();*/ break;
						case 0xB: /*arm_cmn();*/ break;
						case 0xC: /*arm_orr();*/ break;
						case 0xD: arm_mov(); break;
						case 0xE: /*arm_bic();*/ break;
						case 0xF: /*arm_mvn();*/ break;
					}
					break;
	            	/*Single Data Transfer*/
					case 0x4:	/*0100*/
					case 0x5:	/*0101*/
					case 0x6:	/*0110*/
					case 0x7:	/*0111*/
						if(((opcode>>20)&1)==1){	/*Load from memory*/
							//arm7tdmi_arm_ldr();
							break;
						}else if(((opcode>>20)&1)==0){					/*Store to memory*/
							//arm7tdmi_arm_str();
							break;
						}
						if(((opcode>>4)&1)==1){
							//arm7tdmi_set_cpu_mode(ARM_MODE_UND);/*UNDƒ‚[ƒh*/
							break;
						}
						break;
					/*Block Data Transfer*/
					case 0x8:	/*1000*/
					case 0x9:	/*1001*/
						if(((opcode>>20)&1)==1){	/*Load from */
							//arm_ldm();
						}else{
							//arm_stm();
						}
						break;
	            	/*Branch*/
					case 0xA:	/*1010*/
						arm_b();
						break;
					/*Branch with link*/
					case 0xB:	/*1011*/
						arm_bl();
						break;
					/*Software interrupt*/
					case 0xF:	/*1111*/
						//arm_swi();
						break;
					/*Default*/
	                default:
	                    cycles = 1;
	                    break;
	            }
            }

            PackFlags();

            return cycles;
        }

        private void FlushQueue()
        {
            opcodeQueue = myEngine.myMemory.ReadWord(registers[15]);
            registers[15] += 4;
        }
    }
}
