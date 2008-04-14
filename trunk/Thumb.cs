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
		private Engine myEngine;
		
		public Thumb(Engine engine)
		{
			myEngine = engine;
		}
		
		public uint Emulate()
		{
			ushort	opcode;
			byte	opcode_11_5, opcode_6_5, opcode_9_3;
			uint 	address=myEngine.myARM.Registers[15];
			
			opcode		= myEngine.myMemory.ReadShort(address);
			
			opcode_11_5	= (byte)((opcode >> 11) & 0x11);	/*11-15 5bit*/
			opcode_6_5	= (byte)((opcode >> 6) & 0x1F);
			opcode_9_3	= (byte)((opcode >> 9) & 0x07);
			
			return 1;
		}
	}
}
