/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 6/8/2008
 * Time: 8:01 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
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
        private uint opcode, opcodeQueue;
        private int cycles;
        public uint zero, carry, negative, overflow;
        private uint[] registers;

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
        public uint imm_rotate(uint opcode)
		{
			uint	imm_val;
			int 	shift;
				
			shift	= (int)(((opcode >> 8) & 0xF)*2);
			imm_val	= opcode & 0xFF;

            return (imm_val >> shift) | (imm_val << (32 - shift));
		}
        
        //Barrel Shifter
        private const byte SHIFT_LSL = 0;
        private const byte SHIFT_LSR = 1;
        private const byte SHIFT_ASR = 2;
        private const byte SHIFT_ROR = 3;
        
        public uint imm_shift(uint opcode)
		{
			byte	shift_type;
			int 	shift;
			uint 	rm;
			int 	valueSigned;
				
			rm = registers[opcode & 0x0F];
			shift_type = (byte)((opcode >> 5) & 3);		
			shift = (int)((opcode >> 7) & 0x1F);
            
			switch(shift_type)
			{
				case SHIFT_LSL:
                    if (shift == 0)
                    {
                        return rm;
                    }
                    else
                    {
                        carry = (rm >> (32 - shift)) & 1;
                        return rm << shift;
                    }
				case SHIFT_LSR:
                    if (shift == 0)
                    {
                        carry = (rm >> 31) & 1;
                        return 0;
                    }
                    else
                    {
                        carry = (rm >> (shift - 1)) & 1;
                        return rm >> shift;
                    }
				case SHIFT_ASR:
                    if (shift == 0)
                    {
                        if ((rm & (1 << 31)) == 0)
                        {
                            carry = 0;
                            return 0;
                        }
                        else
                        {
                            carry = 1;
                            return 0xFFFFFFFF;
                        }
                    }
                    else
                    {
                        carry = (rm >> (shift - 1)) & 1;
                        return (uint)(((int)rm) >> shift);
                    }
				case SHIFT_ROR:
                    if (shift == 0)
                    {
                        // Actually an RRX
                        valueSigned = (int)(rm & 1); //Temp
                        rm = (carry << 31) | (rm >> 1);
                        carry = (uint)valueSigned;
                    }
                    else
                    {
                        carry = (rm >> (shift - 1)) & 1;
                        return (rm >> shift) | (rm << (32 - shift));
                    }
					break;
			}
			
            return rm;
		}

        public uint reg_shift(uint opcode)
        {
            byte shift_type;
            int shift;
            uint rm;

            rm = registers[opcode & 0x0F];
            shift_type = (byte)((opcode >> 5) & 3);

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

            if (shift == 0)
            {
                return rm;
            }

            switch (shift_type)
            {
                case SHIFT_LSL:
                    if (shift < 32)
                    {
                        carry = (rm >> (32 - shift)) & 1;
                        return rm << shift;
                    }
                    else if (shift == 32)
                    {
                        carry = rm & 1;
                        return 0;
                    }
                    else
                    {
                        carry = 0;
                        return 0;
                    }
                case SHIFT_LSR:
                    if (shift < 32)
                    {
                        carry = (rm >> (shift - 1)) & 1;
                        return rm >> shift;
                    }
                    else if (shift == 32)
                    {
                        carry = (rm >> 31) & 1;
                        return 0;
                    }
                    else
                    {
                        carry = 0;
                        return 0;
                    }
                case SHIFT_ASR:
                    if (shift >= 32)
                    {
                        if ((rm & (1 << 31)) == 0)
                        {
                            carry = 0;
                            return 0;
                        }
                        else
                        {
                            carry = 1;
                            return 0xFFFFFFFF;
                        }
                    }
                    else
                    {
                        carry = (rm >> (shift - 1)) & 1;
                        return (uint)(((int)rm) >> shift);
                    }
                case SHIFT_ROR:
                    if ((shift & 0x1F) == 0)
                    {
                        carry = (rm >> 31) & 1;
                        return rm;
                    }
                    else
                    {
                        shift &= 0x1F;
                        carry = (rm >> shift) & 1;
                        return (rm >> shift) | (rm << (32 - shift));
                    }
            }

            return rm;
        }
        #endregion
        
        #region Opcodes
        
        #region Section 3
        void arm_bx()
        {
        	uint rn = opcode & 0x0F;
        	
        	if(rn==15) 
        	{
        		myEngine.myLog.WriteLog("BX addressing r15 is an undefined action.");
        		myEngine.emulate = false;
        	}

        	registers[15] = registers[rn];
        	
        	if((registers[15]&1)!=0)
        	{
        		//Remove thumb bit
        		registers[15] -= 1;
        		
        		//Set it to proper address
        		//registers[15] += 2;
        		
        		//Set cpu mode to thumb
        		registers[16] |= 1 << Armcpu.T_BIT;

                myEngine.myCPU.myThumb.FlushQueue();
        	}
        	else
        	{
            	FlushQueue();
        	}
            
            cycles = 3;
        }
        #endregion
        
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
        
        void arm_dataproc()
		{
        	uint operand = 0;
        	uint rd = 0;
        	uint rn = 0;
        	
        	rd = (opcode >> 12) & 0x0F;
        	rn = (opcode >> 16) & 0x0F;
        		
        	if(BIT_N(opcode,25)){	/*Immediate Operand*/
				operand = imm_rotate(opcode);
			}else{
                if(BIT_N(opcode,4))
                {
				    operand = reg_shift(opcode);
                }
                else
                {
                    operand = imm_shift(opcode);
                }
			}
        	
        	bool registerShift = (opcode & (1 << 4)) == (1 << 4);
            if (rn == 15 && !BIT_N(opcode, 25) && registerShift)
            {
                rn = registers[rn] + 4;
            }
            else
            {
                rn = registers[rn];
            }
        	
        	switch((byte)((opcode >> 21) & 0x0F)){
				case 0x0: 
        			/*and*/
        			registers[rd] = rn & operand;
        			if(BIT_N(opcode,20)) set_dp_flags(registers[rd]);
        			break;
				case 0x1: 
        			/*eor*/ 
        			registers[rd] = rn ^ operand;
                    if (BIT_N(opcode, 20)) set_dp_flags(registers[rd]);
        			break;
				case 0x2: 
        			/*sub*/
        			registers[rd] = rn - operand;
                    if (BIT_N(opcode, 20)) set_sub_flag(rn, operand, registers[rd]);
        			break;
				case 0x3: 
        			/*rsb*/ 
        			registers[rd] = operand - rn;
                    if (BIT_N(opcode, 20)) set_sub_flag(rn, operand, registers[rd]);
        			break;
				case 0x4: 
        			/*add*/ 
        			registers[rd] = rn + operand;
                    if (BIT_N(opcode, 20)) set_add_flag(rn, operand, registers[rd]);
        			break;
				case 0x5: 
        			/*adc*/ 
        			registers[rd] = rn + operand + carry;
                    if (BIT_N(opcode, 20)) set_add_flag(rn, operand, registers[rd]);
        			break;
				case 0x6: 
        			/*sbc*/ 
        			registers[rd] = rn - operand + carry - 1;
                    if (BIT_N(opcode, 20)) set_sub_flag(rn, operand, registers[rd]);
        			break;
				case 0x7: 
        			/*rsc*/ 
        			registers[rd] = operand - rn + carry - 1;
                    if (BIT_N(opcode, 20)) set_sub_flag(rn, operand, registers[rd]);
        			break;
				case 0x8: 
        			/*tst*/ 
        			set_dp_flags(rn & operand);
        			break;
				case 0x9: 
        			/*teq*/ 
        			set_dp_flags(rn ^ operand);
        			break;
				case 0xA: 
        			/*cmp*/ 
        			//set_dp_flags(rn - operand);
        			set_sub_flag(rn, operand, rn - operand);
        			break;
				case 0xB: 
        			/*cmn*/
                    set_add_flag(rn, operand, rn + operand);
        			break;
				case 0xC: 
        			/*orr*/
        			registers[rd] = rn | operand;
                    if (BIT_N(opcode, 20)) set_dp_flags(registers[rd]);
        			break;
				case 0xD: 
        			/*mov*/
        			registers[rd] = operand;
                    if (BIT_N(opcode, 20)) set_dp_flags(registers[rd]);
        			break;
				case 0xE: 
        			/*bic*/
        			registers[rd] = rn & ~operand;
                    if (BIT_N(opcode, 20)) set_dp_flags(registers[rd]);
        			break;
				case 0xF: 
        			/*mvn*/ 
        			registers[rd] = 0xFFFFFFFF ^ operand;
                    if (BIT_N(opcode, 20)) set_dp_flags(registers[rd]);
        			break;
			}
            
            if (rd == 15)
            {
            	// Prevent writing if no SPSR exists (this will be true for USER or SYSTEM mode)
                //if (this.parent.SPSRExists) this.parent.WriteCpsr(this.parent.SPSR);
                UnpackFlags();

                // Check for branch back to Thumb Mode
                if ((registers[16] & Armcpu.T_MASK) == Armcpu.T_MASK)
                {
                        return;
                }

                // Otherwise, flush the instruction queue
                FlushQueue();
            }
            
            cycles = 1;
		}
        #endregion
        
        #region Section 6
        void arm_mrs()
		{
			uint rd = 0;
			
			rd = (opcode >> 12) & 0x0F;
		
			if(BIT_N(opcode,22)){
				switch((byte)(registers[16] & 0xF)){
				case 0x0:	/*USR*/
				case 0xF:	/*SYS*/
					registers[rd] = registers[16];
					break;
				case 0x1:	/*FIQ*/
					registers[rd] = myEngine.myCPU.bankedFIQ[0];
					break;
				case 0x2:	/*IRQ*/
					registers[rd] = myEngine.myCPU.bankedIRQ[0];
					break;
				case 0x3:	/*SVC*/
					registers[rd] = myEngine.myCPU.bankedSVC[0];
					break;
				case 0x7:	/*ABT*/
					registers[rd] = myEngine.myCPU.bankedABT[0];
					break;
				case 0xB:	/*UND*/
					registers[rd] = myEngine.myCPU.bankedUND[0];
					break;
				}
			}else{
				registers[rd] = registers[16];
			}
		
			cycles = 3;
		}
        
        void arm_msr()
		{
			uint rm = 0;
			
			rm = opcode & 0x0F;
		
			if(BIT_N(opcode,22)){
				switch((byte)(registers[16] & 0xF)){
				case 0x0:	/*USR*/
				case 0xF:	/*SYS*/
					registers[16] = registers[rm];
					break;
				case 0x1:	/*FIQ*/
					myEngine.myCPU.bankedFIQ[0] = registers[rm];
					myEngine.myCPU.SwapRegisters(Armcpu.FIQ);
					break;
				case 0x2:	/*IRQ*/
					myEngine.myCPU.bankedIRQ[0] = registers[rm];
					myEngine.myCPU.SwapRegisters(Armcpu.IRQ);
					break;
				case 0x3:	/*SVC*/
					myEngine.myCPU.bankedSVC[0] = registers[rm];
					myEngine.myCPU.SwapRegisters(Armcpu.SVC);
					break;
				case 0x7:	/*ABT*/
					myEngine.myCPU.bankedABT[0] = registers[rm];
					myEngine.myCPU.SwapRegisters(Armcpu.ABT);
					break;
				case 0xB:	/*UND*/
					myEngine.myCPU.bankedUND[0] = registers[rm];
					myEngine.myCPU.SwapRegisters(Armcpu.UND);
					break;
				}
			}else{
				registers[16] = registers[rm];
			}
			
			switch((byte)(registers[16] & 0xF)){
				case 0x0:	/*USR*/
				case 0xF:	/*SYS*/
					myEngine.myCPU.SwapRegisters(Armcpu.USR);
					break;
				case 0x1:	/*FIQ*/
					myEngine.myCPU.SwapRegisters(Armcpu.FIQ);
					break;
				case 0x2:	/*IRQ*/
					myEngine.myCPU.SwapRegisters(Armcpu.IRQ);
					break;
				case 0x3:	/*SVC*/
					myEngine.myCPU.SwapRegisters(Armcpu.SVC);
					break;
				case 0x7:	/*ABT*/
					myEngine.myCPU.SwapRegisters(Armcpu.ABT);
					break;
				case 0xB:	/*UND*/
					myEngine.myCPU.SwapRegisters(Armcpu.UND);
					break;
			}
			
		
			cycles = 3;
		}
        #endregion
        
        #region Section 7
        void arm_mla()
        {
            uint rm = opcode & 0x0F;
            uint rs = (opcode >> 8) & 0x0F;
            uint rn = (opcode >> 12) & 0x0F;
            uint rd = (opcode >> 16) & 0x0F;
        	
        	registers[rd] = registers[rm] * registers[rs] + registers[rn];

        	if(BIT_N(opcode,20)) set_dp_flags(registers[rd]);
        	
        	//Need to fix cycles
        	cycles = 3;
		}
        
        void arm_mul()
        {
            uint rm = opcode & 0x0F;
            uint rs = (opcode >> 8) & 0x0F;
            uint rn = (opcode >> 12) & 0x0F;
            uint rd = (opcode >> 16) & 0x0F;
        	
        	registers[rd] = registers[rm] * registers[rs];

            if (BIT_N(opcode, 20)) set_dp_flags(registers[rd]);
        	
        	//Need to fix cycles
        	cycles = 3;
		}
        
        //Long Variants
        void arm_mlal()
        {
            uint rm = opcode & 0x0F;
            uint rs = (opcode >> 8) & 0x0F;
            uint rdlo = (opcode >> 12) & 0x0F;
            uint rdhi = (opcode >> 16) & 0x0F;
        	
        	if (BIT_N(opcode,22)) {	//If it's signed...
				// SMLAL
                long accum = (((long)((int)registers[rdhi])) << 32) | registers[rdlo];
                long result = ((long)((int)registers[rm])) * ((long)((int)registers[rs]));
                result += accum;
                registers[rdhi] = (uint)(result >> 32);
                registers[rdlo] = (uint)(result & 0xFFFFFFFF);
    		}
        	else
        	{
        		// UMLAL
                ulong accum = (((ulong)registers[rdhi]) << 32) | registers[rdlo];
                ulong result = ((ulong)registers[rm]) * registers[rs];
                result += accum;
                registers[rdhi] = (uint)(result >> 32);
                registers[rdlo] = (uint)(result & 0xFFFFFFFF);
        	}
			
        	set_dp_flags(registers[rdhi]);
        	
        	//Need to fix cycles
        	cycles = 5;
		}
        
        void arm_mull()
        {
            uint rm = opcode & 0x0F;
            uint rs = (opcode >> 8) & 0x0F;
            uint rdlo = (opcode >> 12) & 0x0F;
            uint rdhi = (opcode >> 16) & 0x0F;
        	
        	if (BIT_N(opcode,22)) {	//If it's signed...
				// SMULL
                long result = ((long)((int)registers[rm])) * ((long)((int)registers[rs]));
                registers[rdhi] = (uint)(result >> 32);
                registers[rdlo] = (uint)(result & 0xFFFFFFFF);
    		}
        	else
        	{
        		// UMULL
                ulong result = ((ulong)registers[rm]) * registers[rs];
                registers[rdhi] = (uint)(result >> 32);
                registers[rdlo] = (uint)(result & 0xFFFFFFFF);
        	}
			
        	set_dp_flags(registers[rdhi]);
        	
        	//Need to fix cycles
        	cycles = 5;
		}
        #endregion
        
        #region Section 9
        void arm_ldr()
        {
        	uint rd = 0;
        	uint rn = 0;
        	uint rm = 0;
        	uint address = 0;
        	uint fimmed = 0;
        	uint fpreindex = 0;
        	uint faddbase = 0;
        	uint fbyte = 0;
        	uint fwriteback = 0;
        	
        	rd = (opcode >> 12) & 0x0F;
        	rn = (opcode >> 16) & 0x0F;
        	
        	fimmed = 	((opcode >> 25) & 1);
        	fpreindex = ((opcode >> 24) & 1);
        	faddbase = 	((opcode >> 23) & 1);
        	fbyte = 	((opcode >> 22) & 1);
        	fwriteback =((opcode >> 21) & 1);
        	
        	address = registers[rn];
        	
        	if(fimmed!=0)
        	{
				rm = imm_shift(opcode);
        	}
        	else
        	{
				rm = opcode & 0xFFF;
			}
        	
        	if(fpreindex!=0)
        	{
				if(faddbase!=0)
				{
					address += rm;
				}
				else
				{
					address -= rm;
				}
			}
        	
        	if(fbyte!=0)
        	{
        		registers[rd] = myEngine.myMemory.ReadByte(address);
        	}
        	else
        	{
        		registers[rd] = myEngine.myMemory.ReadWord(address);
        	}
        	
        	if (rd == 15)
            {
            	registers[15] &= ~3U;
            	FlushQueue();
            }
        	
        	if(fpreindex==0)
        	{
				if(faddbase!=0)
				{
					address += rm;
				}
				else
				{
					address -= rm;
				}
			}
        	
        	if((fwriteback==1) || (fpreindex==0)){	/*Write-back: ‘‚«ž‚Ýæ‚ÌƒAƒhƒŒƒX‚ðŽc‚µ‚Ä‚¨‚­*/
				registers[rn] = address;
			}
        	
        	cycles = 3;
		}
        
        void arm_str()
        {
        	uint rd = 0;
        	uint rn = 0;
        	uint rm = 0;
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
        	
        	if(fimmed!=0)
        	{
				rm = imm_shift(opcode);
        	}
        	else
        	{
				rm = opcode & 0xFFF;
			}
        	
        	if(fpreindex!=0)
        	{
				if(faddbase!=0)
				{
					address += rm;
				}
				else
				{
					address -= rm;
				}
			}
        	
        	amount = registers[rd];
            if (rd == 15) amount += 4;
        	
        	if(fbyte!=0)
        	{
        		myEngine.myMemory.WriteByte(address,(byte)(amount & 0xFF));
        	}
        	else
        	{
        		myEngine.myMemory.WriteWord(address,amount);
        	}
        	
        	if(fpreindex==0)
        	{
				if(faddbase!=0)
				{
					address += rm;
				}
				else
				{
					address -= rm;
				}
			}
        	
        	if((fwriteback==1) || (fpreindex==0)){	/*Write-back: ‘‚«ž‚Ýæ‚ÌƒAƒhƒŒƒX‚ðŽc‚µ‚Ä‚¨‚­*/
				registers[rn] = address;
			}
        	
        	cycles = 2;
		}
        #endregion

        #region Section 10
        void arm_ldrs()
        {
            uint rd = 0;
            uint rn = 0;
            uint rm = 0;
            uint address = 0;
            uint fimmed = 0;
            uint fpreindex = 0;
            uint faddbase = 0;
            uint fbyte = 0;
            uint fwriteback = 0;

            rd = (opcode >> 12) & 0x0F;
            rn = (opcode >> 16) & 0x0F;

            fimmed = ((opcode >> 25) & 1);
            fpreindex = ((opcode >> 24) & 1);
            faddbase = ((opcode >> 23) & 1);
            fbyte = ((opcode >> 22) & 1);
            fwriteback = ((opcode >> 21) & 1);

            address = registers[rn];

            if (fimmed != 0)
            {
                rm = opcode & 0xFFF;
            }
            else
            {
                rm = imm_shift(opcode);
            }

            if (fpreindex != 0)
            {
                if (faddbase != 0)
                {
                    address += rm;
                }
                else
                {
                    address -= rm;
                }
            }

            if (fbyte != 0)
            {
                registers[rd] = myEngine.myMemory.ReadByte(address);
            }
            else
            {
                registers[rd] = myEngine.myMemory.ReadWord(address);
            }

            if (rd == 15)
            {
                registers[15] &= ~3U;
                FlushQueue();
            }

            if (fpreindex == 0)
            {
                if (faddbase != 0)
                {
                    address += rm;
                }
                else
                {
                    address -= rm;
                }
            }

            if ((fwriteback == 1) || (fpreindex == 0))
            {	/*Write-back: ‘‚«ž‚Ýæ‚ÌƒAƒhƒŒƒX‚ðŽc‚µ‚Ä‚¨‚­*/
                registers[rn] = address;
            }

            cycles = 3;
        }
        void arm_ldrs_imm()
        {
            uint rd = 0;
            uint rn = 0;
            uint rm = 0;
            uint address = 0;
            uint fimmed = 0;
            uint fpreindex = 0;
            uint faddbase = 0;
            uint ftype = 0;
            uint fwriteback = 0;

            rd = (opcode >> 12) & 0x0F;
            rn = (opcode >> 16) & 0x0F;

            fimmed = ((opcode >> 22) & 1);
            fpreindex = ((opcode >> 24) & 1);
            faddbase = ((opcode >> 23) & 1);
            fwriteback = ((opcode >> 21) & 1);
            ftype = ((opcode >> 5) & 3);

            address = registers[rn];

            if (fimmed != 0)
            {
                rm = (opcode & 0xF) | (((opcode >> 8) & 0xF) << 4);
            }
            else
            {
                rm = imm_shift(opcode);
            }

            if (fpreindex != 0)
            {
                if (faddbase != 0)
                {
                    address += rm;
                }
                else
                {
                    address -= rm;
                }
            }

            if (ftype == 1)
            {
                registers[rd] = myEngine.myMemory.ReadShort(address);
            }
            else if (ftype == 2)
            {
                registers[rd] = myEngine.myMemory.ReadByte(address);
                if ((registers[rd] & 0x80) != 0)
                {
                    registers[rd] |= 0xFFFFFF00;
                }
            }
            else
            {
                registers[rd] = myEngine.myMemory.ReadShort(address);
                if ((registers[rd] & 0x8000) != 0)
                {
                    registers[rd] |= 0xFFFF0000;
                }
                //myEngine.myLog.WriteLog("Unhandled opcode 0x" + Convert.ToString(opcode));
                //registers[rd] = myEngine.myMemory.ReadWord(address);
            }

            if (rd == 15)
            {
                registers[15] &= ~3U;
                FlushQueue();
            }

            if (fpreindex == 0)
            {
                if (faddbase != 0)
                {
                    address += rm;
                }
                else
                {
                    address -= rm;
                }
            }

            if ((fwriteback == 1) || (fpreindex == 0))
            {	/*Write-back: ‘‚«ž‚Ýæ‚ÌƒAƒhƒŒƒX‚ðŽc‚µ‚Ä‚¨‚­*/
                registers[rn] = address;
            }

            cycles = 3;
        }
        void arm_strs_imm()
        {
            uint rd = 0;
            uint rn = 0;
            uint rm = 0;
            uint address = 0;
            uint fimmed = 0;
            uint fpreindex = 0;
            uint faddbase = 0;
            uint ftype = 0;
            uint fwriteback = 0;

            rd = (opcode >> 12) & 0x0F;
            rn = (opcode >> 16) & 0x0F;

            fimmed = ((opcode >> 22) & 1);
            fpreindex = ((opcode >> 24) & 1);
            faddbase = ((opcode >> 23) & 1);
            fwriteback = ((opcode >> 21) & 1);
            ftype = ((opcode >> 5) & 3);

            address = registers[rn];

            if (fimmed != 0)
            {
                rm = (opcode & 0xF) | (((opcode >> 8) & 0xF) << 4);
            }
            else
            {
                rm = imm_shift(opcode);
            }

            if (fpreindex != 0)
            {
                if (faddbase != 0)
                {
                    address += rm;
                }
                else
                {
                    address -= rm;
                }
            }

            if (ftype == 1)
            {
                myEngine.myMemory.WriteShort(address,(ushort)registers[rd]);
            }
            else if (ftype == 2)
            {
                registers[rd] = myEngine.myMemory.ReadWord(address);
                registers[rd+1] = myEngine.myMemory.ReadWord(address+4);
            }
            else//if (ftype == 3)
            {
                myEngine.myMemory.WriteWord(address, registers[rd]);
                myEngine.myMemory.WriteWord(address + 4, registers[rd + 1]);
            }

            if (rd == 15)
            {
                registers[15] &= ~3U;
                FlushQueue();
            }

            if (fpreindex == 0)
            {
                if (faddbase != 0)
                {
                    address += rm;
                }
                else
                {
                    address -= rm;
                }
            }

            if ((fwriteback == 1) || (fpreindex == 0))
            {	/*Write-back: ‘‚«ž‚Ýæ‚ÌƒAƒhƒŒƒX‚ðŽc‚µ‚Ä‚¨‚­*/
                registers[rn] = address;
            }

            cycles = 3;
        }
        #endregion

        #region Section 11
        void arm_ldm()
        {
            uint rn = 0;
            int lst = 0;
            uint done = 0;
            uint address = 0;
            uint fpreindex = 0;
            uint faddbase = 0;
            uint fbyte = 0;
            uint fwriteback = 0;

            rn = (opcode >> 16) & 0x0F;

            fpreindex = ((opcode >> 24) & 1);
            faddbase = ((opcode >> 23) & 1);
            fbyte = ((opcode >> 22) & 1);
            fwriteback = ((opcode >> 21) & 1);

            address = registers[rn];

            cycles = 3;

            if (BIT_N(opcode, 23)) lst = 0; else lst = 15;

            while (done==0)
            {
                if (BIT_N(opcode, lst))
                {
                    cycles++;
                    if (fpreindex != 0)
                    {
                        if (faddbase != 0)
                        {
                            address += 4;
                        }
                        else
                        {
                            address -= 4;
                        }
                    }

                    if (fbyte != 0)
                    {
                        registers[lst] = myEngine.myMemory.ReadByte(address);
                    }
                    else
                    {
                        registers[lst] = myEngine.myMemory.ReadWord(address);
                    }

                    if (lst == 15)
                    {
                        registers[lst] &= ~3U;
                        FlushQueue();
                    }

                    if (fpreindex == 0)
                    {
                        if (faddbase != 0)
                        {
                            address += 4;
                        }
                        else
                        {
                            address -= 4;
                        }
                    }
                }

                if (BIT_N(opcode, 23) && (lst >= 15)) done = 1;
                if ((!BIT_N(opcode, 23)) && (lst <= 0)) done = 1;
                if (BIT_N(opcode, 23)) lst++; else lst--; 
            }

            if ((fwriteback == 1)/* || (fpreindex == 0)*/)
            {	/*Write-back: ‘‚«ž‚Ýæ‚ÌƒAƒhƒŒƒX‚ðŽc‚µ‚Ä‚¨‚­*/
                registers[rn] = address;
            }

        }

        void arm_stm()
        {
            uint rn = 0;
            int lst = 0;
            uint done = 0;
            uint address = 0;
            uint fpreindex = 0;
            uint faddbase = 0;
            uint fbyte = 0;
            uint fwriteback = 0;

            rn = (opcode >> 16) & 0x0F;

            fpreindex = ((opcode >> 24) & 1);
            faddbase = ((opcode >> 23) & 1);
            fbyte = ((opcode >> 22) & 1);
            fwriteback = ((opcode >> 21) & 1);

            address = registers[rn];

            cycles = 3;

            if (BIT_N(opcode, 23)) lst = 0; else lst = 15;

            while (done == 0)
            {
                if (BIT_N(opcode, lst))
                {
                    cycles++;
                    if (fpreindex != 0)
                    {
                        if (faddbase != 0)
                        {
                            address += 4;
                        }
                        else
                        {
                            address -= 4;
                        }
                    }

                    if (fbyte != 0)
                    {
                        myEngine.myMemory.WriteByte(address, (byte)(registers[lst] & 0xFF));
                    }
                    else
                    {
                        myEngine.myMemory.WriteWord(address, registers[lst]);
                    }

                    if (fpreindex == 0)
                    {
                        if (faddbase != 0)
                        {
                            address += 4;
                        }
                        else
                        {
                            address -= 4;
                        }
                    }
                }

                if (BIT_N(opcode, 23) && (lst >= 15)) done = 1;
                if ((!BIT_N(opcode, 23)) && (lst <= 0)) done = 1;
                if (BIT_N(opcode, 23)) lst++; else lst--;
            }

            if ((fwriteback == 1) || (fpreindex == 0))
            {	/*Write-back: ‘‚«ž‚Ýæ‚ÌƒAƒhƒŒƒX‚ðŽc‚µ‚Ä‚¨‚­*/
                registers[rn] = address;
            }

        }
        #endregion

        #region Section 12
        void arm_swp()
		{
        	uint rd = 0;
        	uint rn = 0;
        	uint rm = 0;
            uint temp = 0;
        	
        	rm = (opcode & 0x0F);
        	rd = (opcode >> 12) & 0x0F;
        	rn = (opcode >> 16) & 0x0F;
        	
        	if(BIT_N(opcode,22))
        	{
                temp = (uint)myEngine.myMemory.ReadByte(registers[rn]);
        		myEngine.myMemory.WriteByte(registers[rn], (byte)registers[rm]);
                registers[rd] = temp;
        	}
        	else
        	{
                temp = myEngine.myMemory.ReadWord(registers[rn]);
				myEngine.myMemory.WriteWord(registers[rn], registers[rm]);
                registers[rd] = temp;
        	}
        	
        	cycles = 4;
        }
        #endregion
        
        #region Section 13
        void arm_swi()
		{
			uint swi = (uint)(opcode & 0x00FFFFFF);
		 	
			//Not done yet
			if(swi==0xFF) //Log
			{
				byte character = (byte)myEngine.myCPU.Registers[1];
                byte printChar = (byte)myEngine.myCPU.Registers[3];
				if(character == 0xFF) 
				{
					myEngine.myLog.WriteLog(Environment.NewLine);
				}
                else if (character == 0)
                {
                    myEngine.myLog.WriteLog(Environment.NewLine);
                }
                else
                {
                    //if(printChar == 3)
                    myEngine.myLog.WriteLog(ASCIIEncoding.ASCII.GetString(new byte[] { character }));
                }
			}
		 	
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
        
        private void set_sub_flag(uint a, uint b, uint c)
		{	 	
        	/*sub,rsb,rsc,cmp*/
        	if(c==0)
        	{
        		zero = 1; 
        	}
        	else
        	{
        		zero = 0;
        	}
        	
			if(((c>>31)&1)==1)
        	{
        		negative = 1; 
        	}
        	else
        	{
        		negative = 0;
        	}
        	
        	if(((((a & (~b)) | (a & (~c)) | ((~b) & (~c)))>>31)&1)==1)
        	{
        		carry = 1; 
        	}
        	else
        	{
        		carry = 0;
        	}
        	
        	if(((((a & ~(b | c)) | ((b & c) & (~a)))>>31)&1)==1)
        	{
        		overflow = 1; 
        	}
        	else
        	{
        		overflow = 0;
        	}
		}
		
		private void set_add_flag(uint a, uint b, uint c)
		{		
			/*add,adc,cmn*/
			if(c==0)
        	{
        		zero = 1; 
        	}
        	else
        	{
        		zero = 0;
        	}
			
			if(((c>>31)&1)==1)
        	{
        		negative = 1; 
        	}
        	else
        	{
        		negative = 0;
        	}
        	
        	if(((((a & b) | (a & (~c)) | (b & (~c)))>>31)&1)==1)
        	{
        		carry = 1; 
        	}
        	else
        	{
        		carry = 0;
        	}
        	
        	if(((((a & b & (~c)) | ((~a) & (~b) & c))>>31)&1)==1)
        	{
        		overflow = 1; 
        	}
        	else
        	{
        		overflow = 0;
        	}
		}
		
		private void set_dp_flags(uint a)	
		{	
			/*muls,mlas,and,eor,tst,teq,orr,mov,bic,mvn*/
			negative = a >> 31;
            zero = a == 0 ? 1U : 0U;
		}
		
		private bool BIT_N(uint a, int b)
		{
			uint test = ((a>>b)&1);
			return (bool)(test==1) ? true : false;
		}
        #endregion

        public int Emulate()
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
	            	case 0x0: /*0000*/
						if(((opcode >> 4) & 0x0F) == 0x09)
						{
							if(BIT_N(opcode,23))
							{	
								/*Multiply Long*/
								if(BIT_N(opcode,21))
								{	
									/*Accumelate*/
									arm_mlal();
									break;
								}
								else
								{
									arm_mull();
									break;
								}
							}
	            			else
	            			{					
	            				/*Multiply*/
	            				if(BIT_N(opcode,21))
								{	
									arm_mla();
									break;
								}
								else
								{
									arm_mul();
									break;
								}
							}
						}
                        /*Data Processing can be eitehr 0x1 or 0x3*/
                        //arm_dataproc();
                        goto jump1;
                    /*Data Processing*/
					case 0x1:	/*0001*/
jump1:
                        if (((opcode >> 4) & 0xFF) == 0x09)
                        {
                            /*00001001*/
                            arm_swp();	/*Single Data Swap*/
                            break;
                        }
                        if ((opcode & 0x0FFFFFF0) == 0x012FFF10)
                        {
                            arm_bx();	/*Branch and Exchange*/
                            break;
                        }
                        if ((opcode & 0x0FFF0000) == 0x016F0000)
                        {
                            //  arm_clz();	/*Count Leading Zeros*/
                            break;
                        }
                        if (BIT_N(opcode, 4) && BIT_N(opcode, 7))
                        {
                            /*Halfword data Transfer:*/
                            if (BIT_N(opcode, 20))
                            {
                                /*immdiate offset*/
                                if (BIT_N(opcode, 22))
                                {
                                    /*Load from memory*/
                                    arm_ldrs_imm();
                                    break;
                                }
                                else
                                {
                                    /*Load from memory*/
                                    arm_ldrs();
                                    break;
                                }
                            }
                            else
                            {
                                /*register offset*/
                                if (BIT_N(opcode, 22))
                                {
                                    /*Store to memory*/
                                    arm_strs_imm();
                                    break;
                                }
                                else
                                {
                                    /*Store to memory*/
                                    //arm_strs();
                                    break;
                                }
                            }
                        }
                        goto jump2;
                    case 0x2:	/*0010*/
                    case 0x3:	/*0011*/
jump2:
						if((opcode&0xFFF00000)==0xE1000000){
			            	arm_mrs();
			            	break;
			            }
			            if((opcode&0xFFF00000)==0xE1200000){
			            	arm_msr();
			            	break;
			            }
						
						/*Data Processing can be eitehr 0x1, 0x2 or 0x3*/
						arm_dataproc();
						break;
					
	            	/*Single Data Transfer*/
					case 0x4:	/*0100*/
					case 0x5:	/*0101*/
					case 0x6:	/*0110*/
					case 0x7:	/*0111*/
						if(BIT_N(opcode,20))
						{	
							/*Load from memory*/
							arm_ldr();
						}
						else
						{
							/*Store to memory*/
							arm_str();
						}
						break;
					/*Block Data Transfer*/
					case 0x8:	/*1000*/
					case 0x9:	/*1001*/
						if(BIT_N(opcode,20))
						{	
							/*Load from */
							arm_ldm();
						}
						else
						{
							arm_stm();
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
						arm_swi();
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

        public void FlushQueue()
        {
            opcodeQueue = myEngine.myMemory.ReadWord(registers[15]);
            registers[15] += 4;
        }
    }
}
