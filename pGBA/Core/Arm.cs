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

        public Arm(Engine engine)
        {
            myEngine = engine;
        }

        public void Begin()
        {
            registers = myEngine.myCPU.Registers;
            
            //Flush queue here because arm is always executed first
            //FlushQueue();
        }
        
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

            opcode = opcodeQueue;
            FlushQueue();
            
            opcode_24_4	= (byte)((opcode >> 24) & 0x0F);	/*24-27 4bit*/

            UnpackFlags();

            switch (opcode_24_4)
            {
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
