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
		//Memory
		public byte[] bios;
		public byte[] wram;
		public byte[] iwram;
		public byte[] io;
		public byte[] pal;
		public byte[] vram;
		public byte[] oam;
		public byte[] rom;
		public byte[] sram;
		
		//Defines
		public bool romLoaded=false;
		public uint romSize;
		
		public Memory()
		{
			bios 	= new byte[0x3FFF];
			wram 	= new byte[0x3FFFF];
			iwram 	= new byte[0x7FFF];
			io 		= new byte[0x3FF];
			pal 	= new byte[0x3FF];
			vram 	= new byte[0x17FFF];
			oam		= new byte[0x3FF];
			sram 	= new byte[0xFFFF];
		}
		
		public byte ReadByte(uint adr)
		{
			
			//Work RAM (256kb)
			if((adr >= 0x02000000) && (adr <= 0x0203FFFF))
			{
				adr -= 0x02000000;
				return (byte)(wram[adr]);
			}
			
			//IWRAM (32kb)
			if((adr >= 0x03000000) && (adr <= 0x03007FFF))
			{
				adr -= 0x03000000;
				return (byte)(iwram[adr]);
			}
			
			//I/O Registers (0x3FF)
			if((adr >= 0x04000000) && (adr <= 0x040003FF))
			{
				adr -= 0x04000000;
				return (byte)(io[adr]);
			}
			
			//Cartridge Rom
			if((adr >= 0x08000000) && (adr <(0x08000000+romSize)))
			{
				adr -= 0x08000000;
				return (byte)(rom[adr]);
			}
			
			return (byte)0;
		}
		
		public ushort ReadShort(uint adr)
		{
			//Work RAM (256kb)
			if((adr >= 0x02000000) && ((adr+1) <= 0x0203FFFF))
			{
				adr -= 0x02000000;
				return (ushort)(wram[adr]|wram[adr+1]<<8);
			}
			
			//IWRAM (32kb)
			if((adr >= 0x03000000) && ((adr+1) <= 0x03007FFF))
			{
				adr -= 0x03000000;
				return (ushort)(iwram[adr]|iwram[adr+1]<<8);
			}
			
			//I/O Registers (0x3FF)
			if((adr >= 0x04000000) && ((adr+1) <= 0x040003FF))
			{
				adr -= 0x04000000;
				return (ushort)(io[adr]|io[adr+1]<<8);
			}
			
			//VWRAM
			if((adr >= 0x06000000) && ((adr+1) <= 0x06017FFF))
			{
				adr -= 0x06000000;
				return (ushort)(vram[adr]|vram[adr+1]<<8);
			}
			
			//Cartridge Rom
			if((adr >= 0x08000000) && ((adr+1) <(0x08000000+romSize)))
			{
				adr -= 0x08000000;
				return (ushort)(rom[adr]|rom[adr+1]<<8);
			}
			
			return (ushort)0;
		}
		
		public uint ReadWord(uint adr)
		{
			//Work RAM (256kb)
			if((adr >= 0x02000000) && ((adr+3) <= 0x0203FFFF))
			{
				adr -= 0x02000000;
				return (uint)(wram[adr]|wram[adr+1]<<8|wram[adr+2]<<16|wram[adr+3]<<24);
			}
			
			//IWRAM (32kb)
			if((adr >= 0x03000000) && ((adr+3) <= 0x03007FFF))
			{
				adr -= 0x03000000;
				return (uint)(iwram[adr]|iwram[adr+1]<<8|iwram[adr+2]<<16|iwram[adr+3]<<24);
			}
			
			//I/O Registers (0x3FF)
			if((adr >= 0x04000000) && ((adr+3) <= 0x040003FF))
			{
				adr -= 0x04000000;
				return (uint)(io[adr]|io[adr+1]<<8|io[adr+2]<<16|io[adr+3]<<24);
			}
			
			//VWRAM
			if((adr >= 0x06000000) && ((adr+3) <= 0x06017FFF))
			{
				adr -= 0x06000000;
				return (uint)(vram[adr]|vram[adr+1]<<8|vram[adr+2]<<16|vram[adr+3]<<24);
			}
			
			//Cartridge Rom
			if((adr >= 0x08000000) && ((adr+3) <= (0x08000000+romSize)))
			{
				adr -= 0x08000000;
				return (uint)(rom[adr]|rom[adr+1]<<8|rom[adr+2]<<16|rom[adr+3]<<24);
			}
			return (uint)0;
		}
		
		public void WriteByte(uint adr, byte value)
		{
			//Just too make sure its within a byte
			value &= 0xFF;
			
			//Work RAM (256kb)
			if((adr >= 0x02000000) && (adr <= 0x0203FFFF))
			{
				adr -= 0x02000000;
				wram[adr] = value;
			}
			
			//IWRAM (32kb)
			if((adr >= 0x03000000) && (adr <= 0x03007FFF))
			{
				adr -= 0x03000000;
				iwram[adr] = value;
			}
		}
		
		public void WriteShort(uint adr, ushort value)
		{
			//Work RAM (256kb)
			if((adr >= 0x02000000) && ((adr+1) <= 0x0203FFFF))
			{
				adr -= 0x02000000;
				wram[adr] = (byte)(value & 0xFF);
				wram[adr+1] = (byte)((value >> 8) & 0xFF);
			}
			
			//IWRAM (32kb)
			if((adr >= 0x03000000) && ((adr+1) <= 0x03007FFF))
			{
				adr -= 0x03000000;
				iwram[adr] = (byte)(value & 0xFF);
				iwram[adr+1] = (byte)((value >> 8) & 0xFF);
			}
			
			//VRAM
			if((adr >= 0x06000000) && ((adr+1) <= 0x06017FFF))
			{
				adr -= 0x06000000;
				vram[adr] = (byte)(value & 0xFF);
				vram[adr+1] = (byte)((value >> 8) & 0xFF);
			}
		}
		
		public void WriteWord(uint adr, uint value)
		{
			//Work RAM (256kb)
			if((adr >= 0x02000000) && ((adr+3) <= 0x0203FFFF))
			{
				adr -= 0x02000000;
				wram[adr] = (byte)(value & 0xFF);
				wram[adr+1] = (byte)((value >> 8) & 0xFF);
				wram[adr+2] = (byte)((value >> 16) & 0xFF);
				wram[adr+3] = (byte)((value >> 24) & 0xFF);
			}
			
			//IWRAM (32kb)
			if((adr >= 0x03000000) && ((adr+3) <= 0x03007FFF))
			{
				adr -= 0x03000000;
				iwram[adr] = (byte)(value & 0xFF);
				iwram[adr+1] = (byte)((value >> 8) & 0xFF);
				iwram[adr+2] = (byte)((value >> 16) & 0xFF);
				iwram[adr+3] = (byte)((value >> 24) & 0xFF);
			}
			
			//VRAM
			if((adr >= 0x06000000) && ((adr+3) <= 0x06017FFF))
			{
				adr -= 0x06000000;
				vram[adr] = (byte)(value & 0xFF);
				vram[adr+1] = (byte)((value >> 8) & 0xFF);
				vram[adr+2] = (byte)((value >> 16) & 0xFF);
				vram[adr+3] = (byte)((value >> 24) & 0xFF);
			}

            //Cartridge Rom
            if ((adr >= 0x08000000) && ((adr + 3) <= (0x08000000 + romSize)))
            {
                adr -= 0x08000000;
                rom[adr] = (byte)(value & 0xFF);
                rom[adr + 1] = (byte)((value >> 8) & 0xFF);
                rom[adr + 2] = (byte)((value >> 16) & 0xFF);
                rom[adr + 3] = (byte)((value >> 24) & 0xFF);
            }
		}
		
		public void LoadRom(byte[] _rom)
		{
			//rom = new byte[file_size];
			rom = _rom;
			romLoaded =true;
		}
	}
}
