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
		private Engine myEngine;
		private Thumb myThumb;
		
		public uint[] Registers;
		public uint cycles;
		
		public Armcpu(Engine engine)
		{
			myEngine = engine;
			myThumb = new Thumb(myEngine);
			
			Registers = new uint[16];
			Registers[15] = 0x08000000;
		}
		
		public void Emulate(uint cpu_cycles)
		{
			cycles = cpu_cycles;
			while(cycles > 0)
			{
				//if(arm)
					//cycles -= myArm.Emulator();
				//else
				cycles -= myThumb.Emulate();
			}
		}
	}
}
