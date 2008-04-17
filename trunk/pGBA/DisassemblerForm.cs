/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/15/2008
 * Time: 8:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace pGBA
{
	/// <summary>
	/// Description of DisassemblerForm.
	/// </summary>
	public partial class DisassemblerForm : Form
	{
		private Engine myEngine;
		private Disassembler disasm;
		
		private uint curPC=0;
		
		public DisassemblerForm(Engine engine)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			timer.Start();
			myEngine = engine;
			
			disasm = new Disassembler(engine);
			
			curPC = myEngine.myCPU.Registers[15];
			
			RefreshRegisterList();
			
			if(myEngine.myMemory.romLoaded)
			{
				RefreshDisasmList();
			}
		}
		
		void RefreshDisasmList()
		{
			int i=0;
			uint tAdr=0;
			
			for(i=0; i<17; i++)
			{
				tAdr = (uint)(curPC+(i*2));
				disasmList.Items[i] = disasm.DisasmThumb(tAdr);
			}
		}
		
		void RefreshRegisterList()
		{
			int i=0;
			
			for(i=0; i<17; i++)
			{
				regList.Items[i] = "R"+i.ToString()+" = 0x"+Convert.ToString(myEngine.myCPU.Registers[i],16).ToUpper().PadLeft(8,'0');
			}
		
		}
		
		void TimerTick(object sender, EventArgs e)
		{			
			if(!autoUpdate.Checked) return;
			
			curPC = myEngine.myCPU.Registers[15];
			
			RefreshRegisterList();
			RefreshDisasmList();
		}
		
		void VScrollBar1Scroll(object sender, ScrollEventArgs e)
		{
			switch (e.Type)
            {
                case ScrollEventType.SmallIncrement:
					///if(thumb)
                    	curPC += 2;
					//else
					//	curPC += 4;
                    break;
                case ScrollEventType.SmallDecrement:
                    ///if(thumb)
                    	curPC -= 2;
					//else
					//	curPC -= 4;
                    break;
                case ScrollEventType.LargeIncrement:
                    curPC += 0x100;
                    break;
                case ScrollEventType.LargeDecrement:
                    curPC -= 0x100;
                    break;
                case ScrollEventType.ThumbTrack:
                    curPC = (uint)e.NewValue;
                    break;
            }
			
			RefreshRegisterList();
			RefreshDisasmList();
		}
		
		void CloseBtnClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void refreshBtnClick(object sender, EventArgs e)
		{
			curPC = myEngine.myCPU.Registers[15];
			
			RefreshRegisterList();
			RefreshDisasmList();
		}
		
		void NextBtnClick(object sender, EventArgs e)
		{			
			myEngine.myCPU.Step();
			
			curPC = myEngine.myCPU.Registers[15];	
			RefreshRegisterList();
			RefreshDisasmList();
		}
	}
}
