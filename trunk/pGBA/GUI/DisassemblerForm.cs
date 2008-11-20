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
		private uint inArm=0;
		
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
			
			inArm = 0;
			if((myEngine.myCPU.Registers[16] & Armcpu.T_MASK)==0) 
				inArm=1;
			
			RefreshRegisterList();
			UpdateDisassembly();
		}
		
		private void UpdateDisassembly()
        {
            if (myEngine.myCPU != null && myEngine.myMemory != null)
            {
                if ((myEngine.myCPU.Registers[16] & Armcpu.T_MASK)!=0)
                {
                    uint pc = (uint)(myEngine.myCPU.Registers[15] - 0x2U);
                    int numItems = disasmList.Height / disasmList.ItemHeight;

                    if (pc > curPC && pc < curPC + numItems * 2 - 2 && /*!prevInArm &&*/ disasmList.Items.Count > 0)
                    {
                        disasmList.SelectedIndex = (int)(pc - curPC) / 2;
                        return;
                    }

                    curPC = pc - 0x2U;

                    RefreshDisasmList();

                    disasmList.SelectedIndex = (int)(pc - curPC) / 2;
                }
                else
                {
                    uint pc = (uint)(myEngine.myCPU.Registers[15] - 0x4U);
                    int numItems = disasmList.Height / disasmList.ItemHeight;

                    if (pc > curPC && pc < curPC + numItems * 4 - 4 /*&& this.prevInArm*/ && disasmList.Items.Count > 0)
                    {
                        disasmList.SelectedIndex = (int)(pc - curPC) / 4;
                        return;
                    }

                    curPC = pc - 0x4U;

                    RefreshDisasmList();

                    disasmList.SelectedIndex = (int)(pc - curPC) / 4;
                }
            }
        }
		
		private void RefreshDisasmList()
		{
			int i=0;
			uint tAdr=0;

			for(i=0; i<18; i++)
			{
				if((myEngine.myCPU.Registers[16] & Armcpu.T_MASK)!=0) 
				{
					tAdr = (uint)(curPC+(i*2));
					disasmList.Items[i] = disasm.DisasmThumb(tAdr);
				}
				else
				{
					tAdr = (uint)(curPC+(i*4));
					disasmList.Items[i] = disasm.DisasmArm(tAdr);
				}
			}
		}
		
		private void RefreshRegisterList()
		{
			int i=0;
			string temp;
			
			for(i=0; i<17; i++)
			{
				temp=i.ToString();
				regList.Items[i] = "R"+temp.PadRight(5-temp.Length,' ')+"= 0x"+Convert.ToString(myEngine.myCPU.Registers[i],16).ToUpper().PadLeft(8,'0');
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
		
		private void TimerTick(object sender, EventArgs e)
		{			
			if(!autoUpdate.Checked) 
				return;
			
			RefreshRegisterList();
			UpdateDisassembly();
		}
		
		private void VScrollBar1ValueChanged(object sender, EventArgs e)
		{
			vScrollBar1.Value = 158;
		}
		
		private void VScrollBar1Scroll(object sender, ScrollEventArgs e)
		{
			switch (e.Type)
            {
                case ScrollEventType.SmallIncrement:
					if((myEngine.myCPU.Registers[16] & Armcpu.T_MASK)!=0) 
                    	curPC += 2;
					else
						curPC += 4;
                    break;
                case ScrollEventType.SmallDecrement:
                    if((myEngine.myCPU.Registers[16] & Armcpu.T_MASK)!=0) 
                    	curPC -= 2;
					else
						curPC -= 4;
                    break;
                case ScrollEventType.LargeIncrement:
                    curPC += 0x100;
                    break;
                case ScrollEventType.LargeDecrement:
                    curPC -= 0x100;
                    break;
                case ScrollEventType.ThumbTrack:
                    //curPC = (uint)e.NewValue;
                    break;
            }
				
			RefreshRegisterList();
			RefreshDisasmList();
		}
		
		private void CloseBtnClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		private void refreshBtnClick(object sender, EventArgs e)
		{
			//SelectCurPC();
			RefreshRegisterList();
			UpdateDisassembly();
		}
		
		private void NextBtnClick(object sender, EventArgs e)
		{	
			if(myEngine.myMemory.romLoaded)
			{
				int ii=1;
				while(ii!=0)
				{
					myEngine.myCPU.Step();
					RefreshRegisterList();
					UpdateDisassembly();
					ii-=1;
				}
			}
		}
		
		private void regListDoubleClick(object sender, EventArgs e)
		{
			int itemSelected=regList.SelectedIndex;
			string itemText=Convert.ToString(myEngine.myCPU.Registers[itemSelected],16).PadLeft(8,'0');
															 
			Rectangle r=regList.GetItemRectangle(itemSelected);
			
			editRegister.Location=new System.Drawing.Point(editRegister.Location.X,regList.Location.Y+r.Y);
			editRegister.Size=new System.Drawing.Size(editRegister.Size.Width, r.Height-10);
			editRegister.Visible=true;
			editRegister.Text=itemText;
			editRegister.Focus();
			editRegister.SelectAll();
			
		}
		
		private void regListClick(object sender, EventArgs e)
		{
			editRegister.Visible=false;	
		}
		
		private void editRegister_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar==13)
			{
				myEngine.myCPU.Registers[regList.SelectedIndex] = Convert.ToUInt32(editRegister.Text,16);
				RefreshRegisterList();
				editRegister.Visible=false;
			}
			if (e.KeyChar==27)
				editRegister.Visible=false;
		}
		
		private void SelectCurPC()
		{
			if (inArm==0)
            {
				uint pc = (uint)(myEngine.myCPU.Registers[15] - 0x2U);
				disasmList.SelectedIndex = (int)(pc - curPC) / 2;
			}
			else
			{
				uint pc = (uint)(myEngine.myCPU.Registers[15] - 0x4U);
				disasmList.SelectedIndex = (int)(pc - curPC) / 4;
			}
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			if(myEngine.myMemory.romLoaded)
			{
				int ii=1;
				while(ii!=0)
				{
					myEngine.myCPU.StepScanline();
					RefreshRegisterList();
					UpdateDisassembly();
					ii-=1;
				}
			}
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			if(myEngine.myMemory.romLoaded)
			{
				int ii=0;
				while(ii<160)
				{
					myEngine.myCPU.StepScanline();
					RefreshRegisterList();
					UpdateDisassembly();
					ii++;
				}
			}
		}
	}
}
