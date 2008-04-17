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
		
		public byte[] rom;
		public bool romLoaded=false;
		public uint romSize;
		
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
			if((adr >= 0x08000000) && (adr <(0x08000000+romSize)-1))
			{
				adr -= 0x08000000;
				return (ushort)(rom[adr]|rom[adr+1]<<8);
			}
			
			return (ushort)0;
		}
		
		public uint ReadWord(uint adr)
		{
			//Needs to be implemented
			return (uint)0;
		}
		
		public void WriteByte(uint adr, byte value)
		{
			//Needs to be implemented
		}
		
		public void WriteShort(uint adr, ushort value)
		{
			//Needs to be implemented
		}
		
		public void WriteWord(uint adr, uint value)
		{
			//Needs to be implemented
		}
		
		public void LoadRom(byte[] _rom)
		{
			//rom = new byte[file_size];
			rom = _rom;
			romLoaded =true;
		}
	}
}
