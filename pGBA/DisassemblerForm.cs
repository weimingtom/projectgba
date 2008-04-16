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
			
			RefreshRegisterList();
			RefreshDisasmList();
		}
		
		void RefreshDisasmList()
		{
			int i=0;
			
			for(i=0; i<17; i++)
			{
				disasmList.Items[i] = disasm.DisasmThumb(0);
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
			
			
			RefreshRegisterList();
		}
		
		void VScrollBar1Scroll(object sender, ScrollEventArgs e)
		{
			vScrollBar1.Value = 158;
		}
		
		void CloseBtnClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void refreshBtnClick(object sender, EventArgs e)
		{
			RefreshRegisterList();
		}
	}
}
