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
			
			Nflag.Checked = (myEngine.myCPU.Registers[16] & Armcpu.N_MASK) == 0 ? false:true;
			Zflag.Checked = (myEngine.myCPU.Registers[16] & Armcpu.Z_MASK) == 0 ? false:true;
			Cflag.Checked = (myEngine.myCPU.Registers[16] & Armcpu.C_MASK) == 0 ? false:true;
			Vflag.Checked = (myEngine.myCPU.Registers[16] & Armcpu.V_MASK) == 0 ? false:true;
			Iflag.Checked = (myEngine.myCPU.Registers[16] & Armcpu.I_MASK) == 0 ? false:true;
			Fflag.Checked = (myEngine.myCPU.Registers[16] & Armcpu.F_MASK) == 0 ? false:true;
			Tflag.Checked = (myEngine.myCPU.Registers[16] & Armcpu.T_MASK) == 0 ? false:true;
		
			ModeValue.Text =  Convert.ToString((myEngine.myCPU.Registers[16] & 0xFF),16).ToUpper();
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
