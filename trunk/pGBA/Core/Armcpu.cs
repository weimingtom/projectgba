/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/13/2008
 * Time: 5:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace pGBA
{
	/// <summary>
	/// Description of Armcpu.
	/// </summary>
	public class Armcpu
	{	      
		//CPU Conditions
		public const int COND_EQ = 0;	    // Z set
        public const int COND_NE = 1;	    // Z clear
        public const int COND_CS = 2;	    // C set
        public const int COND_CC = 3;	    // C clear
        public const int COND_MI = 4;	    // N set
        public const int COND_PL = 5;	    // N clear
        public const int COND_VS = 6;	    // V set
        public const int COND_VC = 7;	    // V clear
        public const int COND_HI = 8;	    // C set and Z clear
        public const int COND_LS = 9;	    // C clear or Z set
        public const int COND_GE = 10;	    // N equals V
        public const int COND_LT = 11;	    // N not equal to V
        public const int COND_GT = 12; 	// Z clear AND (N equals V)
        public const int COND_LE = 13; 	// Z set OR (N<>V)
        
        // CPU mode definitions
        public const uint USR = 0x10;
        public const uint FIQ = 0x11;
        public const uint IRQ = 0x12;
        public const uint SVC = 0x13;
        public const uint ABT = 0x17;
        public const uint UND = 0x1B;
        public const uint SYS = 0x1F;

        // CPSR bit definitions
        public const int N_BIT = 31;
        public const int Z_BIT = 30;
        public const int C_BIT = 29;
        public const int V_BIT = 28;
        public const int I_BIT = 7;
        public const int F_BIT = 6;
        public const int T_BIT = 5;

        //CPSR bit masks
        public const uint N_MASK = (uint)(1U << N_BIT);
        public const uint Z_MASK = (uint)(1U << Z_BIT);
        public const uint C_MASK = (uint)(1U << C_BIT);
        public const uint V_MASK = (uint)(1U << V_BIT);
        public const uint I_MASK = (uint)(1U << I_BIT);
        public const uint F_MASK = (uint)(1U << F_BIT);
        public const uint T_MASK = (uint)(1U << T_BIT);
        
        // Standard registers
        public uint[] Registers =  new uint[17];
        
        // Banked registers
        public uint[] bankedFIQ = new uint[7];
        public uint[] bankedIRQ = new uint[2];
        public uint[] bankedSVC = new uint[2];
        public uint[] bankedABT = new uint[2];
        public uint[] bankedUND = new uint[2];
        
        // Saved CPSR's
        public uint spsrFIQ = 0;
        public uint spsrIRQ = 0;
        public uint spsrSVC = 0;
        public uint spsrABT = 0;
        public uint spsrUND = 0;

        public uint IRQ_VBLANK = 0x0001;
        public uint IRQ_HBLANK = 0x0002;
        public uint IRQ_VCOUNT = 0x0004;
        public uint IRQ_TIMER0 = 0x0008;
        public uint IRQ_TIMER1 = 0x0010;
        public uint IRQ_TIMER2 = 0x0020;
        public uint IRQ_TIMER3 = 0x0040;
        public uint IRQ_SERIAL = 0x0080;
        public uint IRQ_DMA0 = 0x0100;
        public uint IRQ_DMA1 = 0x0200;
        public uint IRQ_DMA2 = 0x0400;
        public uint IRQ_DMA3 = 0x0800;
        public uint IRQ_KEYPAD = 0x1000;
        public uint IRQ_GAMEPAK = 0x2000;
        
		private Engine myEngine;
		public Thumb myThumb;
		public Arm myArm;
		
		public int cycles;
		
		public Armcpu(Engine engine)
		{
			myEngine = engine;
			myArm = new Arm(myEngine);
			myThumb = new Thumb(myEngine);
		}
		
		public void Reset()
		{
			
			//Clear the log on each reset
			myEngine.myLog.ClearLog();
			
			//Reset all Registers to 0
			for(int i=0; i<16; i++)
				Registers[i] = 0x00000000;
			
			//Init Default Registers
			Registers[13] = 0x03007F00;
			Registers[15] = 0x08000000;
			
			bankedSVC[0] = 0x03007FE0;
            bankedIRQ[0] = 0x03007FA0;
			
			Registers[16] = 0x0000005F; //VBA sets 0x5F should default to 0x1F
			//Default to thumb mode (Cheating)
			//Registers[16] = 0x00000003F;	
			
			Begin();
		}
		
		public void Begin()
		{
			myThumb.Begin();
			myArm.Begin();
		}
		
		public void Emulate(int cpu_cycles)
		{
			cycles = cpu_cycles;
			while(cycles > 0)
			{
				if((Registers[16] & T_MASK)!=0) //Thumb State
					cycles -= myThumb.Emulate();
				else
					cycles -= myArm.Emulate();
			}
		}
		
		public void Step()
		{
			cycles = 20;
			
			if((Registers[16] & T_MASK)!=0)  //Thumb State
				cycles -= myThumb.Emulate();
			else
				cycles -= myArm.Emulate();
		}
		
		public void StepScanline()
		{
			Emulate(960);
			myEngine.myGfx.RenderLine();
			myEngine.myGfx.EnterHBlank();
			Emulate(272);
			myEngine.myGfx.LeaveHBlank();
		}
		
		private void SwapRegsHelper(uint[] swapRegs)
        {
            for (int i = 14; i > 14 - swapRegs.Length; i--)
            {
                uint tmp = Registers[i];
                Registers[i] = swapRegs[swapRegs.Length - (14 - i) - 1];
                swapRegs[swapRegs.Length - (14 - i) - 1] = tmp;
            }
        }

        public void SwapRegisters(uint bank)
        {
            switch (bank & 0x1F)
            {
                case FIQ:
                    SwapRegsHelper(bankedFIQ);
                    break;
                case SVC:
                    SwapRegsHelper(bankedSVC);
                    break;
                case ABT:
                    SwapRegsHelper(bankedABT);
                    break;
                case IRQ:
                    SwapRegsHelper(bankedIRQ);
                    break;
                case UND:
                    SwapRegsHelper(bankedUND);
                    break;
            }
        }

        public void Interrupt(uint irq)
        {
	        if((myEngine.myMemory.ReadWord(0x04000208) & 1) == 0)return;
            if ((myEngine.myMemory.ReadShort(0x04000200) & irq) == 0) return;

            myEngine.myMemory.WriteWord(myEngine.myCPU.Registers[13], myEngine.myCPU.Registers[15]);
            myEngine.myCPU.Registers[13] += 4;
            myEngine.myCPU.Registers[15] = myEngine.myMemory.ReadWord(0x03007F00);
            myArm.FlushQueue();
        }
	}
}
