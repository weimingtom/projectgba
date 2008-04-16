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
			
			RefreshRegisterList();
		}
		
		void RefreshRegisterList()
		{
			int i=0;
			int padding=0;
			
			for(i=0; i<17; i++)
			{
				padding = 9-Convert.ToString(myEngine.myCPU.Registers[i],16).Length;
				regList.Items[i] = "R"+Convert.ToString(i)+" = 0x"+Convert.ToString(myEngine.myCPU.Registers[i],16).ToUpper().PadLeft(padding,'0');
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
