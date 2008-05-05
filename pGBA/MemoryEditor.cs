/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/23/2008
 * Time: 12:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
	
namespace pGBA
{
	/// <summary>
	/// Description of MemoryEditor.
	/// </summary>
	public partial class MemoryEditor : Form
	{
		Engine myEngine;
		string[] List = new string[9];
		uint[] BaseList = new uint[9];
		uint[] SizeList = new uint[9];
		
		uint curPos=0;
		
		public MemoryEditor(Engine engine)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			myEngine = engine;
			
			timer1.Start();
		}
		
		void CloseBtnClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void RefreshMem()
		{
			uint i,j;
			uint value;
			byte[] buffer = new byte[16];
			uint baseAdr=curPos;
			uint size=SizeList[MemList.SelectedIndex];
			
			memText.Clear();
			
			for(i=0; i<18; i++)
			{
				memText.Text += Convert.ToString(baseAdr+(i*16),16).PadLeft(8,'0').PadRight(4);
				for(j=0; j<16; j++)
				{
					value = myEngine.myMemory.ReadByte(baseAdr+(i*16)+j);
					buffer[j] = (byte)(value);
					memText.Text += " " + Convert.ToString(value,16).PadLeft(2,'0');
				}
				memText.Text += " ";
				memText.Text += Encoding.ASCII.GetString(buffer);
				if(i<17)memText.Text += Environment.NewLine;
			}
		}
		
		void MemoryEditorLoad(object sender, EventArgs e)
		{
			List[0] = "BIOS";
			BaseList[0] = 0x00000000;
			SizeList[0] = 0x00003FFF;
			List[1] = "WRAM";
			BaseList[1] = 0x02000000;
			SizeList[1] = 0x0003FFFF;
			List[2] = "IWRAM";
			BaseList[2] = 0x03000000;
			SizeList[2] = 0x00007FFF;
			List[3] = "IO";
			BaseList[3] = 0x04000000;
			SizeList[3] = 0x000003FE;
			List[4] = "PAL";
			BaseList[4] = 0x05000000;
			SizeList[4] = 0x000003FF;
			List[5] = "VRAM";
			BaseList[5] = 0x06000000;
			SizeList[5] = 0x00017FFF;
			List[6] = "OAM";
			BaseList[6] = 0x07000000;
			SizeList[6] = 0x000003FF;
			List[7] = "ROM";
			BaseList[7] = 0x08000000;
			SizeList[7] = myEngine.myMemory.romSize;
			List[8] = "OAM";
			BaseList[8] = 0x0E000000;
			SizeList[8] = 0x0000FFFF;
			
			for(int i=0; i<9; i++) MemList.Items.Add(List[i]);
			
			MemList.SelectedIndex = 3;
			curPos = BaseList[MemList.SelectedIndex];
			vScrollBar1.Minimum = 0;
			vScrollBar1.Maximum = (int)SizeList[MemList.SelectedIndex]/(16);
			vScrollBar1.SmallChange = 16;
			vScrollBar1.LargeChange = 16*17;
			
			RefreshMem();
		}
		
		void Timer1Tick(object sender, EventArgs e)
		{
			
		}
		
		void MemListSelectedIndexChanged(object sender, EventArgs e)
		{
			curPos = BaseList[MemList.SelectedIndex];
			
			vScrollBar1.Minimum = 0;
			vScrollBar1.Maximum = (int)SizeList[MemList.SelectedIndex]/16;
			vScrollBar1.SmallChange = 16;
			vScrollBar1.LargeChange = (int)(SizeList[MemList.SelectedIndex])/32;
			
			RefreshMem();
		}
		
		void VScrollBar1Scroll(object sender, ScrollEventArgs e)
		{
			switch (e.Type)
            {
                case ScrollEventType.SmallIncrement:
                    curPos += 16;
                    break;
                case ScrollEventType.SmallDecrement:
                    curPos -= 16;
                    break;
                case ScrollEventType.LargeIncrement:
                    curPos += 16*17;
                    break;
                case ScrollEventType.LargeDecrement:
                    curPos -= 16*17;
                    break;
                case ScrollEventType.ThumbTrack:
                    curPos = (uint)e.NewValue;
                    break;
            }
			uint temp = curPos;
			uint max = SizeList[MemList.SelectedIndex];
			if(temp>max)
			{
			   	curPos = max;
			}
			
			curPos += BaseList[MemList.SelectedIndex];
				
			RefreshMem();
		}
	}
}
