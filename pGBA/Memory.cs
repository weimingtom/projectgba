/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/13/2008
 * Time: 5:43 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;

namespace pGBA
{
	/// <summary>
	/// Description of Memory.
	/// </summary>
	public class Memory
	{
		public Memory()
		{
		}
		
		public byte ReadByte(uint adr)
		{
			//Needs to be implemented
			return (byte)0;
		}
		
		public ushort ReadShort(uint adr)
		{
			//Needs to be implemented
			//MessageBox.Show("ReadShort(0x"+Convert.ToString(adr,16)+");");
			return (ushort)0;
		}
		
		uint ReadWord(uint adr)
		{
			//Needs to be implemented
			return (uint)0;
		}
		
		void WriteByte(uint adr, byte value)
		{
			//Needs to be implemented
		}
		
		void WriteShort(uint adr, ushort value)
		{
			//Needs to be implemented
		}
		
		void WriteWord(uint adr, uint value)
		{
			//Needs to be implemented
		}
	}
}
