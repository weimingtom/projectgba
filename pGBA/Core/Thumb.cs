/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/13/2008
 * Time: 5:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;

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
        private const int COND_LE = 13; 	// Z set OR (N<>V)
        
		private Engine myEngine;
		private ushort	opcode, opcodeQueue, cycles;
		private uint zero, carry, negative, overflow;
		private uint[] registers;
		
		public Thumb(Engine engine)
		{
			myEngine = engine;
		}
		
		#region Overflow Handlers
        private void OverflowCarryAdd(uint a, uint b, uint r)
        {
            overflow = ((a & b & ~r) | (~a & ~b & r)) >> 31;
            carry = ((a & b) | (a & ~r) | (b & ~r)) >> 31;
        }

        private void OverflowCarrySub(uint a, uint b, uint r)
        {
            overflow = ((a & ~b & ~r) | (~a & b & r)) >> 31;
            carry = ((a & ~b) | (a & ~r) | (~b & ~r)) >> 31;
        }
        #endregion
		
        #region Opcodes
        
        #region Section 1
		void thumb_lsl_imm()
		{
            // lsl rd, rm, #immed
			int rd = opcode & 0x07;
            int rs = (opcode >> 3) & 0x07;
            int immed = (opcode >> 6) & 0x1F;

            if (immed == 0)
            {
                registers[rd] = registers[rs];
            } else
            {
                carry = (registers[rs] >> (32 - immed)) & 0x01;
                registers[rd] = registers[rs] << immed;
            }

            negative = registers[rd] >> 31;
            zero = (registers[rd] == 0) ? 1U : 0U;

			cycles = 1;
		}
		
		void thumb_lsr_imm()
		{
            // lsr rd, rm, #immed
			int rd = opcode & 0x07;
            int rs = (opcode >> 3) & 0x07;
            int immed = (opcode >> 6) & 0x1F;

            if (immed == 0)
            {
                carry = registers[rs] >> 31;
                myEngine.myCPU.Registers[rd] = 0;
            } else {
                carry = (registers[rs] >> (immed - 1)) & 0x01;
                registers[rd] = registers[rs] >> immed;
            }

            negative = registers[rd] >> 31;
            zero = (registers[rd] == 0) ? 1U : 0U;

			cycles = 1;
		}
		
		void thumb_asr_imm()
		{
            // asr rd, rs, #immed
			int rd = opcode & 0x07;
            int rs = (opcode >> 3) & 0x07;
            int immed = (opcode >> 6) & 0x1F;

            if (immed == 0)
            {
                carry = registers[rs] >> 31;
                registers[rd] = 0;
            } else {
                carry = (registers[rs] >> (immed - 1)) & 0x01;
                registers[rd] = (uint)(((int)registers[rs]) >> immed);
            }

            negative = registers[rd] >> 31;
            zero = (registers[rd] == 0) ? 1U : 0U;

			cycles = 1;
		}
		#endregion
		
		#region Section 2
		void thumb_add_reg()
		{
			// add rd, rs, rn
			int rd = opcode & 0x07;
            int rs = (opcode >> 3) & 0x07;
            int rn = (opcode >> 6) & 0x07;

            registers[rd] = registers[rs] + registers[rn];
            
            OverflowCarryAdd(registers[rs],registers[rn],registers[rd]);
            
            negative = registers[rd] >> 31;
            zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_sub_reg()
		{
			// sub rd, rs, rn
			int rd = opcode & 0x07;
            int rs = (opcode >> 3) & 0x07;
            int rn = (opcode >> 6) & 0x07;
            
            registers[rd] = registers[rs] - registers[rn];
            
            OverflowCarrySub(registers[rs],registers[rn],registers[rd]);
            
            negative = registers[rd] >> 31;
            zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_add_imm()
		{
			// add rd, rs, #immed
			int rd = opcode & 0x07;
            int rs = (opcode >> 3) & 0x07;
            uint immed = (uint)((opcode >> 6) & 0x07);
            
            registers[rd] = registers[rs] + immed;
            
            OverflowCarryAdd(registers[rs],immed,registers[rd]);
            
            negative = registers[rd] >> 31;
            zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_sub_imm()
		{
			// sub rd, rs, #immed
			int rd = opcode & 0x07;
            int rs = (opcode >> 3) & 0x07;
            uint immed = (uint)((opcode >> 6) & 0x07);
            
            registers[rd] = registers[rs] - immed;
            
            OverflowCarrySub(registers[rs],immed,registers[rd]);
            
            negative = registers[rd] >> 31;
            zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		#endregion
		
		#region Section 3
		void thumb_mov()
		{
			// mov rd, #immed
			uint immed = (uint)(opcode & 0xFF);
            int rd = (opcode >> 8) & 0x07;
            
            registers[rd] = immed;
            
            negative = 0;
            zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_cmp()
		{
			// cmp rd, #immed
			uint immed = (uint)(opcode & 0xFF);
            int rd = (opcode >> 8) & 0x07;
            uint alu = registers[rd] - immed;

            OverflowCarrySub(registers[rd], immed, alu);
            
            negative = alu >> 31;
            zero = (alu == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_add()
		{
			// add rd, #immed
			uint immed = (uint)(opcode & 0xFF);
            int rd = (opcode >> 8) & 0x07;
            uint ord = registers[rd];

            registers[rd] += immed;
            
            OverflowCarryAdd(ord, immed, registers[rd]);
            
            negative = registers[rd] >> 31;
            zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_sub()
		{
			// sub rd, #immed
			uint immed = (uint)(opcode & 0xFF);
            int rd = (opcode >> 8) & 0x07;
            uint ord = registers[rd];

            registers[rd] -= immed;
            
            OverflowCarrySub(ord, immed, registers[rd]);
            
            negative = registers[rd] >> 31;
            zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		#endregion
		
		#region Section 4
		void thumb_alu_and()
		{
			//and rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			
			registers[rd] &= registers[rs];
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_eor()
		{
			//eor rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			
			registers[rd] ^= registers[rs];
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_lsl()
		{
			//lsl rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			int shiftAmt = (int)(registers[rs] & 0xFF);
			
			if (shiftAmt == 0)
            {
            	// Don't need to do anything
            }
            else if (shiftAmt < 32)
            {
            	carry = (registers[rd] >> (32 - shiftAmt)) & 0x1;
            	registers[rd] <<= shiftAmt;
            }
            else if (shiftAmt == 32)
            {
            	carry = registers[rd] & 0x1;
            	registers[rd] = 0;
            }
            else
            {
            	carry = 0;
                registers[rd] = 0;
            }
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            //Is it 1 or 2 here??
            cycles = 2;
		}
		
		void thumb_alu_lsr()
		{
			//lsr rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			int shiftAmt = (int)(registers[rs] & 0xFF);
			
			if (shiftAmt == 0)
            {
            	// Don't need to do anything
            }
            else if (shiftAmt < 32)
            {
            	carry = (registers[rd] >> (shiftAmt - 1)) & 0x1;
            	registers[rd] >>= shiftAmt;
            }
            else if (shiftAmt == 32)
            {
            	carry = (registers[rd] >> 31) & 0x1;
            	registers[rd] = 0;
            }
            else
            {
            	carry = 0;
                registers[rd] = 0;
            }
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            //Is it 1 or 2 here??
            cycles = 2;
		}
		
		void thumb_alu_asr()
		{
			//asr rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			int shiftAmt = (int)(registers[rs] & 0xFF);
			
			if (shiftAmt == 0)
            {
            	// Don't need to do anything
            }
            else if (shiftAmt < 32)
            {
            	carry = (registers[rd] >> (shiftAmt - 1)) & 0x1;
                registers[rd] = (uint)(((int)registers[rd]) >> shiftAmt);
            }
            else if (shiftAmt == 32)
            {
            	carry = (registers[rd] >> 31) & 0x1;
            	//May need some checks here unsure
            	registers[rd] = 0;
            }
            else
            {
            	carry = 0;
                registers[rd] = 0;
            }
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
			//Is it 1 or 2 here??
            cycles = 2;
		}
		
		void thumb_alu_adc()
		{
			//adc rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			uint temp = registers[rd];
			
			registers[rd] += registers[rs] + carry;
			
			this.OverflowCarryAdd(temp, registers[rs], registers[rd]);
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_sbc()
		{
			//sbc rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			uint temp = registers[rd];
			
			registers[rd] = (registers[rd] - registers[rs]) - (1U - carry);
			
			this.OverflowCarrySub(temp, registers[rs], registers[rd]);
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_ror()
		{
			//asr rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			int shiftAmt = (int)(registers[rs] & 0xFF);
			
			if (shiftAmt == 0)
            {
            	// Don't need to do anything
            }
            else if ((shiftAmt & 0x1F) == 0)
            {
            	carry = registers[rd] >> 31;
            }
            else
            {
            	shiftAmt &= 0x1F;
            	carry = (registers[rd] >> (shiftAmt - 1)) & 0x1;
            	registers[rd] = (registers[rd] >> shiftAmt) | (registers[rd] << (32 - shiftAmt));
            }
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
			//Is it 1 or 2 here??
            cycles = 2;
		}
		
		void thumb_alu_tst()
		{
			//tst rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			
			uint temp = registers[rd] & registers[rs];
			
			negative = temp >> 31;
			zero = (temp == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_neg()
		{
			//neg rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			
			registers[rd] = 0 - registers[rs];
			
			this.OverflowCarrySub(0, registers[rs], registers[rd]);
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_cmp()
		{
			//cmp rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			uint temp = registers[rd] - registers[rs];
			
			this.OverflowCarryAdd(registers[rd], registers[rs], temp);
			negative = temp >> 31;
			zero = (temp == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_cmn()
		{
			//cmn rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			uint temp = registers[rd] + registers[rs];
			
			this.OverflowCarrySub(registers[rd], registers[rs], temp);
			negative = temp >> 31;
			zero = (temp == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_orr()
		{
			//orr rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			
			registers[rd] |= registers[rs];
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_mul()
		{
			//mul rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			
			//Need to do mul cycle claculations here
			//hard coded to 3 like in armv5 atleast for now
			uint mulCycles = 3;
			
			registers[rd] *= registers[rs];
			
			myEngine.myCPU.cycles -= mulCycles;
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_bic()
		{
			//bic rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			
			registers[rd] &= ~registers[rs];
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		
		void thumb_alu_mvn()
		{
			//mvn rd,rs
			int rd = (opcode & 0x07);
			int rs = ((opcode>>3) & 0x07);
			
			registers[rd] = ~registers[rs];
			
			negative = registers[rd] >> 31;
			zero = (registers[rd] == 0) ? 1U : 0U;
            
            cycles = 1;
		}
		#endregion
		
		#region Section 5
		void thumb_add_hi()
		{
			//addhi rd,rs
			int rd = ((opcode & (1 << 7)) >> 4) | (opcode & 0x7);
            int rs = (opcode >> 3) & 0xF;
			
			registers[rd] += registers[rs];
			
			if (rd == 15)
            {
                registers[rd] &= ~1U;
                registers[15] += 2;
                cycles = 3;
            }
			else
			{
	           cycles = 1;
			}
		}
		
		void thumb_cmp_hi()
		{
			//cmphi rd,rs
			int rd = ((opcode & (1 << 7)) >> 4) | (opcode & 0x7);
            int rs = (opcode >> 3) & 0xF;
			uint temp = registers[rd] - registers[rs];

            negative = temp >> 31;
            zero = (temp == 0) ? 1U : 0U;
            OverflowCarrySub(registers[rd], registers[rs], temp);
            
	        cycles = 1;
		}
		
		void thumb_mov_hi()
		{
			//movhi rd,rs
			int rd = ((opcode & (1 << 7)) >> 4) | (opcode & 0x7);
            int rs = (opcode >> 3) & 0xF;

            registers[rd] = registers[rs];
            
            if (rd == 15)
            {
                registers[rd] &= ~1U;
                FlushQueue();
                cycles = 3;
            }
			else
			{
	           cycles = 1;
			}
		}
		
		void thumb_bx()
		{
			//bx rs
            int rs = (opcode >> 3) & 0xF;

            registers[16] &= ~Armcpu.T_MASK;
            registers[16] |= (registers[rs] & 1) << Armcpu.T_BIT;

            registers[15] = registers[rs] & (~1U);

            // Check for branch back to Arm Mode
            if ((registers[16] & Armcpu.T_MASK) != Armcpu.T_MASK)
            {
                return;
            }

            FlushQueue();
            
	        cycles = 3;
		}
		#endregion

		#region Section 6
		void thumb_ldr_pc()
		{
			//ldr rd,[pc,#immed]
			int rd = ((opcode>>8) & 0x7);
			uint immed = (uint)(opcode & 0xFF);
			
			registers[rd] = myEngine.myMemory.ReadWord((registers[15] & ~2U) + (immed * 4));
		
	        cycles = 3;
		}
		#endregion
		
		//Might be buggy needs testing
		#region Section 7
		void thumb_str()
		{
			//str rd,[rb,ro]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			int ro = ((opcode>>6) & 0x07);
			
			myEngine.myMemory.WriteWord(registers[rb] + registers[ro], registers[rd]);
            
            cycles = 2;
		}
		
		void thumb_strb()
		{
			//strb rd,[rb,ro]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			int ro = ((opcode>>6) & 0x07);
			
			myEngine.myMemory.WriteByte(registers[rb] + registers[ro], (byte)registers[rd]);
            
            cycles = 2;
		}
		
		void thumb_ldr()
		{
			//ldr rd,[rb,ro]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			int ro = ((opcode>>6) & 0x07);
			
			registers[rd] = myEngine.myMemory.ReadWord(registers[rb] + registers[ro]);
            
            cycles = 3;
		}
		
		void thumb_ldrb()
		{
			//ldrb rd,[rb,ro]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			int ro = ((opcode>>6) & 0x07);
			
			registers[rd] = (uint)myEngine.myMemory.ReadByte(registers[rb] + registers[ro]);
            
            cycles = 3;
		}
		#endregion
		
		//Might be buggy needs testing
		#region Section 8
		void thumb_strh()
		{
			//strh rd,[rb,ro]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			int ro = ((opcode>>6) & 0x07);
			
			myEngine.myMemory.WriteShort(registers[rb] + registers[ro], (ushort)registers[rd]);
            
            cycles = 2;
		}
		
		void thumb_ldsb()
		{
			//ldsb rd,[rb,ro]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			int ro = ((opcode>>6) & 0x07);
			
			registers[rd] = (uint)((sbyte)myEngine.myMemory.ReadByte(registers[rb] + registers[ro]));
            
            cycles = 3;
		}
		
		void thumb_ldrh()
		{
			//ldrh rd,[rb,ro]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			int ro = ((opcode>>6) & 0x07);
			
			registers[rd] = (uint)myEngine.myMemory.ReadShort(registers[rb] + registers[ro]);
            
            cycles = 3;
		}
		
		void thumb_ldsh()
		{
			//ldsh rd,[rb,ro]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			int ro = ((opcode>>6) & 0x07);
			
			registers[rd] = (uint)((short)myEngine.myMemory.ReadShort(registers[rb] + registers[ro]));
            
            cycles = 3;
		}
		#endregion
		
		//Might be buggy needs testing
		#region Section 9
		void thumb_str_imm()
		{
			//str rd,[rb,#immed]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			uint immed = (uint)(((opcode>>6) & 0x1F) * 4);
			
			myEngine.myMemory.WriteWord(registers[rb] + immed, registers[rd]);
            
            cycles = 2;
		}
		
		void thumb_ldr_imm()
		{
			//ldr rd,[rb,#immed]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			uint immed = (uint)(((opcode>>6) & 0x1F) * 4);
			
			registers[rd] = myEngine.myMemory.ReadWord(registers[rb] + immed);
            
            cycles = 3;
		}
		
		void thumb_strb_imm()
		{
			//strb rd,[rb,#immed]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			uint immed = (uint)((opcode>>6) & 0x1F);
			
			myEngine.myMemory.WriteByte(registers[rb] + immed, (byte)registers[rd]);
            
            cycles = 2;
		}
		
		void thumb_ldrb_imm()
		{
			//ldrb rd,[rb,#immed]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			uint immed = (uint)((opcode>>6) & 0x1F);

			registers[rd] = (uint)myEngine.myMemory.ReadByte(registers[rb] + immed);
            
            cycles = 2;
		}
		#endregion
		
		//Might be buggy needs testing
		#region Section 10
		void thumb_strh_imm()
		{
			//strh rd,[rb,#immed]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			uint immed = (uint)(((opcode>>6) & 0x1F) * 2);
			
			myEngine.myMemory.WriteShort(registers[rb] + immed, (ushort)registers[rd]);
            
            cycles = 2;
		}
		
		void thumb_ldrh_imm()
		{
			//ldrh rd,[rb,#immed]
			int rd = (opcode & 0x07);
			int rb = ((opcode>>3) & 0x07);
			uint immed = (uint)(((opcode>>6) & 0x1F) * 2);

			registers[rd] = (uint)myEngine.myMemory.ReadShort(registers[rb] + immed);
            
            cycles = 3;
		}
		#endregion
		
		//Might be buggy needs testing
		#region Section 11
		void thumb_str_sp()
		{
			//str rd,[sp,#immed]
			int rd = ((opcode>>8) & 0x07);
			uint immed = (uint)(((opcode>>6) & 0xFF) * 4);
			
			myEngine.myMemory.WriteWord(registers[13] + immed, registers[rd]);
            
            cycles = 2;
		}
		
		void thumb_ldr_sp()
		{
			//ldr rd,[sp,#immed]
			int rd = ((opcode>>8) & 0x07);
			uint immed = (uint)(((opcode>>6) & 0xFF) * 4);
			
			registers[rd] = myEngine.myMemory.ReadWord(registers[13] + immed);
            
            cycles = 3;
		}
		#endregion
		
		//Might be buggy needs testing
		#region Section 12
		void thumb_add_pc()
		{
			//add  rd,pc,#nn
			int rd = ((opcode>>8) & 0x07);
			uint immed = (uint)((opcode& 0xFF) * 4);
			
			registers[rd] = ((registers[15] & ~2U) + immed);
			
			cycles = 1;
		}
		
		void thumb_add_sp()
		{
			//add  rd,sp,#nn
			int rd = ((opcode>>8) & 0x07);
			uint immed = (uint)((opcode& 0xFF) * 4);
			
			registers[rd] = (registers[13] + immed);
			
			cycles = 1;
		}
		#endregion

		//Might be buggy needs testing
		#region Section 13
		void thumb_add_sp_imm()
		{
			//add  sp,#immed
			uint immed = (uint)((opcode& 0x7F) * 4);
			
			registers[13] += immed;
			
			cycles = 1;
		}
		
		void thumb_sub_sp_imm()
		{
			//add  sp,#-immed
			uint immed = (uint)((opcode& 0x7F) * 4);
			
			registers[13] -= immed;
			
			cycles = 1;
		}
		#endregion
		
		//Might be buggy needs testing (timing needs fixed)
		#region Section 14
		void thumb_push()
		{
			//push {Rlist}
			
			if(((opcode >> 8)&1) == 1)
			{
				registers[13] -= 4;
            	myEngine.myMemory.WriteWord(registers[13], registers[14]);
			}
			
			for (int i = 7; i >= 0; i--)
            {
                if (((opcode >> i) & 1) == 1)
                {
                    registers[13] -= 4;
                    myEngine.myMemory.WriteWord(registers[13], registers[i]);
                }
            }
			
			cycles = 1;
		}
		
		void thumb_pop()
		{
			//pop {Rlist}
			uint immed = (uint)((opcode& 0x7F) * 4);
			
			for (int i = 0; i < 8; i++)
            {
                if (((opcode >> i) & 1) == 1)
                {
                    registers[i] = myEngine.myMemory.ReadWord(registers[13]);
                    registers[13] += 4;
                }
            }
			
			if(((opcode >> 8)&1) == 1)
			{
				registers[15] = myEngine.myMemory.ReadWord(registers[13]) & (~1U);
            	registers[13] += 4;
            	this.FlushQueue();
			}
			
			cycles = 1;
		}
		#endregion
		
		//Might be buggy needs testing (timing needs fixed)
		#region Section 15
		void thumb_stmia()
		{
			//stmia rb!, {Rlist}
			int rb = ((opcode>>8) & 0x07);
			
			for (int i = 0; i < 8; i++)
            {
                if (((opcode >> i) & 1) == 1)
                {
                    myEngine.myMemory.WriteWord(registers[rb] & (~3U), registers[i]);
                    registers[rb] += 4;
                }
            }
			
			cycles = 3;
		}
		
		void thumb_ldmia()
		{
			//ldmia rb!, {Rlist}
			int rb = ((opcode>>8) & 0x07);
			uint adr = registers[rb];
			
			for (int i = 0; i < 8; i++)
            {
                if (((opcode >> i) & 1) == 1)
                {
                    registers[i] = myEngine.myMemory.ReadWord(adr & (~3U));
                    adr += 4;
                }
            }
			
			if (((opcode >> rb) & 1) == 0)
            {
                registers[rb] = adr;
            }
			
			cycles = 3;
		}
		#endregion
		
		//Might be buggy needs testing (timing needs fixed)
		#region Section 16
		void thumb_bxx()
		{
			uint cond = 0;
            switch ((opcode >> 8) & 0xF)
            {
            	case COND_EQ:
            		cond = zero;
            		break;
            	case COND_NE:
            		cond = 1 - zero;
            		break;
            	case COND_CS:
            		cond = carry;
            		break;
            	case COND_CC:
            		cond = 1 - carry;
            		break;
            	case COND_MI:
            		cond = negative;
            		break;
            	case COND_PL:
            		cond = 1 - negative;
            		break;
            	case COND_VS:
            		cond = overflow;
            		break;
            	case COND_VC:
            		cond = 1 - overflow;
            		break;
            	case COND_HI:
            		cond = carry & (1 - zero);
            		break;
            	case COND_LS:
            		cond = (1 - carry) | zero;
            		break;
            	case COND_GE:
            		cond = (1 - negative) ^ overflow;
            		break;
            	case COND_LT:
            		cond = negative ^ overflow;
            		break;
            	case COND_GT: 
            		cond = (1 - zero) & (negative ^ (1 - overflow)); 
            		break;
                case COND_LE: 
            		cond = (negative ^ overflow) | zero; 
            		break;
            }
            
            if (cond == 1)
            {
                uint offset = (uint)(opcode & 0xFF);
                if ((offset & (1 << 7)) != 0) offset |= 0xFFFFFF00;

                registers[15] += offset << 1;        

                FlushQueue();
                
                cycles = 3;
            }
            
            cycles = 1;
		}
		#endregion
		
		//Not done yet
		#region Section 17
		void thumb_swi()
		{
			uint swi = (uint)(opcode & 0xFF);
		 	
			//Not done yet
			if(swi==0xFF) //Log
			{
				byte character = (byte)myEngine.myCPU.Registers[1];
				if(character == 0xFF) 
				{
					myEngine.myLog.WriteLog(Environment.NewLine);
				}
				else
				{
					myEngine.myLog.WriteLog(ASCIIEncoding.ASCII.GetString(new byte[] {character}));
				}
			}
		 	
		 	cycles = 3;
		}
		#endregion
		
		//Might be buggy needs testing
		#region Section 18
		void thumb_b()
		{
			uint offset = (uint)(opcode & 0x7FF);
            if ((offset & (1 << 10)) != 0) offset |= 0xFFFFF800;

            registers[15] += offset << 1;

            FlushQueue();
            
            cycles = 3;
		}
		#endregion
		
		#region Section 19
		void thumb_bl1()
		{
			uint offset = (uint)(opcode & 0x7FF);
            if ((offset & (1 << 10)) != 0) offset |= 0xFFFFF800;

            registers[14] = registers[15] + (offset << 12);
            
            cycles = 1;
		}
		
		void thumb_bl2()
		{
			uint tmp = registers[15];
			uint offset = (uint)(opcode & 0x7FF);
			
            registers[15] = registers[14] + (offset << 1);
            registers[14] = (tmp - 2U) | 1;

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
        
        public void Begin()
        {
        	registers = myEngine.myCPU.Registers;
        	
        	//Should never be done
        	FlushQueue();
        }
		
		public uint Emulate()
		{
			byte	opcode_11_5, opcode_6_5, opcode_9_3;
			
			opcode 		= opcodeQueue;
			FlushQueue();
            
			opcode_11_5	= (byte)((opcode >> 11) & 0x1F);	/*11-15 5bit*/
			opcode_6_5	= (byte)((opcode >> 6) & 0x1F);
			opcode_9_3	= (byte)((opcode >> 9) & 0x07);
			
			UnpackFlags();
			
			switch(opcode_11_5){
			case 0x00:	/*00000*/
				thumb_lsl_imm();
				break;
			case 0x01:	/*00001*/
				thumb_lsr_imm();
				break;
			case 0x02:	/*00010*/
				thumb_asr_imm();
				break;
			case 0x03:	/*00011*/
				switch((opcode >> 9)&3)
				{
					case 0: 
						thumb_add_reg();
						break;
					case 1:
						thumb_sub_reg();
						break;
					case 2: 
						thumb_add_imm();
						break;
					case 3: 
						thumb_sub_imm();
						break;
				}
				break;
			case 0x04:	/*00100*/
				thumb_mov();
				break;
			case 0x05:	/*00101*/
				thumb_cmp();
				break;
			case 0x06:	/*00110*/
				thumb_add();
				break;
			case 0x07:	/*00111*/
				thumb_sub();
				break;
			case 0x08:	/*01000*/
				if(((opcode >> 10)&1)==0)
				{
					switch((opcode >> 6)&0x0F)
					{
						case 0x00:
							thumb_alu_and();
							break;
						case 0x01:
							thumb_alu_eor();
							break;
						case 0x02:
							thumb_alu_lsl();
							break;
						case 0x03:
							thumb_alu_lsr();
							break;
						case 0x04:
							thumb_alu_asr();
							break;
						case 0x05:
							thumb_alu_adc();
							break;
						case 0x06:
							thumb_alu_sbc();
							break;
						case 0x07:
							thumb_alu_ror();
							break;
						case 0x08:
							thumb_alu_tst();
							break;
						case 0x09:
							thumb_alu_neg();
							break;
						case 0x0A:
							thumb_alu_cmp();
							break;
						case 0x0B:
							thumb_alu_cmn();
							break;
						case 0x0C:
							thumb_alu_orr();
							break;
						case 0x0D:
							thumb_alu_mul();
							break;
						case 0x0E:
							thumb_alu_bic();
							break;
						case 0x0F:
							thumb_alu_mvn();
							break;
					}
				}
				else
				{
					switch(((opcode >> 8)&3))
					{
						case 0:
							thumb_add_hi();
							break;
						case 1:
							thumb_cmp_hi();
							break;
						case 2:
							thumb_mov_hi();
							break;
						case 3:
							thumb_bx();
							break;
					}
				}
				break;
			case 0x09:	/*01001*/
				thumb_ldr_pc();
				break;
			case 0x0A:	/*01010*/
			case 0x0B:	/*01011*/
				switch(opcode_9_3){
				case 0x0:	/*000*/
					thumb_str();
					break;
				case 0x2:	/*010*/
					thumb_strb();
					break;
				case 0x4:	/*100*/
					thumb_ldr();
					break;
				case 0x6:	/*110*/
					thumb_ldrb();
					break;
				case 0x1:	/*000*/
					thumb_strh();
					break;
				case 0x3:	/*010*/
					thumb_ldsb();
					break;
				case 0x5:	/*100*/
					thumb_ldrh();
					break;
				case 0x7:	/*110*/
					thumb_ldsh();
					break;
				}
				break;	
			case 0x0C:	/*01100*/
				thumb_str_imm();
				break;
			case 0x0D:	/*01101*/
				thumb_ldr_imm();
				break;
			case 0x0E:	/*01110*/
				thumb_strb_imm();
				break;
			case 0x0F:	/*01111*/
				thumb_ldrb_imm();
				break;
			case 0x10:	/*10000*/
				thumb_strh_imm();
				break;
			case 0x11:	/*10001*/
				thumb_ldrh_imm();
				break;
			case 0x12:	/*10010*/
				thumb_str_sp();
				break;
			case 0x13:	/*10011*/
				thumb_ldr_sp();
				break;
			case 0x14:	/*10100*/
				thumb_add_pc();
				break;
			case 0x15:	/*10101*/
				thumb_add_sp();
				break;
			case 0x16:	/*10110*/
			case 0x17:	/*10111*/
				if(((opcode>>10)&1)==1)
				{
					if(((opcode>>11)&1)==1)
					{
						thumb_pop();
					}
					else
					{
						thumb_push();
					}
				}
				else
				{
					if(((opcode >> 8) & 0x7)==0) /*000S*/
					{
						if(((opcode >> 7)&1) == 0)
						{
							thumb_add_sp_imm();
						}
						else
						{
							thumb_sub_sp_imm();
						}
					}
				}
				break;
			case 0x18:	/*11000*/
				thumb_stmia();
				break;
			case 0x19:	/*11001*/
				thumb_ldmia();
				break;
			case 0x1A:	/*11010*/
			case 0x1B:	/*11011*/
				if(((opcode >> 8) & 0xF) == 0xF){
					thumb_swi();
				}else{
					thumb_bxx();
				}
				break;
			case 0x1C:	/*11100*/
				thumb_b();
				break;
			case 0x1E:	/*11110*/
				thumb_bl1();
				break;
			case 0x1F:	/*11111*/
				thumb_bl2();
				break;
			default:
				cycles = 1;
				break;
			}
			
			PackFlags();
			
			return cycles;
		}
		
		private void FlushQueue()
        {
            opcodeQueue = myEngine.myMemory.ReadShort(registers[15]);
            registers[15] += 2;
        }
	}
}
