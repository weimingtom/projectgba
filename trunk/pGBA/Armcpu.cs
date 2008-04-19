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
        
		private Engine myEngine;
		private Thumb myThumb;
		
		public uint[] Registers;
		public uint cycles;
		
		public Armcpu(Engine engine)
		{
			myEngine = engine;
			myThumb = new Thumb(myEngine);
			
			Registers = new uint[17];
			
			Reset();
		}
		
		public void Reset()
		{
			
			//Only here to test opcodes
			Registers[1] = 0x00000001;
			Registers[2] = 0x00000100;
			
			//Init Default Registers
			Registers[13] = 0x03007F00;
			Registers[15] = 0x08000000;
			Registers[16] = 0x0000001F;
		}
		
		public void Emulate(uint cpu_cycles)
		{
			cycles = cpu_cycles;
			while(cycles > 0)
			{
				//if(Registers[16] & 0x20) //Thumb State
					cycles -= myThumb.Emulate();
				//else
					//cycles -= myArm.Emulator();
			}
		}
		
		public void Step()
		{
			//if(Registers[16] & 0x20) //Thumb State
				myThumb.Emulate();
			//else
				//myArm.Emulator();
		}
	}
}
