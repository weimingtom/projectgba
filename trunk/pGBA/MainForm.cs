/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/13/2008
 * Time: 2:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace pGBA
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private Engine myEngine;
			
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			myEngine = new Engine();
		}
		private Bitmap Scrn = null;
		
		void MainFormLoad(object sender, EventArgs e)
		{
			int i=0;
			Scrn = new Bitmap(240, 160, PixelFormat.Format16bppRgb555);
			while(i++!=239) Scrn.SetPixel(i,0,Color.FromArgb(0,255,0));
			i=0;
			while(i++!=239) Scrn.SetPixel(i,159,Color.FromArgb(255,0,255));
			i=0;
			while(i++!=159) Scrn.SetPixel(0,i,Color.FromArgb(255,255,255));
			i=0;
			while(i++!=159) Scrn.SetPixel(239,i,Color.FromArgb(0,255,255));
			scrnBox.Image = Scrn;
		}
		
		void OpenGBAToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
				using (Stream stream = openFileDialog.OpenFile())
                {
					uint romSize = (uint)stream.Length;
                    
                    byte[] rom = new byte[romSize];
                    stream.Read(rom, 0, (int)stream.Length);
                    
                    myEngine.myMemory.romSize=romSize;

                    myEngine.myMemory.LoadRom(rom);
                }
				myEngine.myCPU.Reset();
			}
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void DisassemblerToolStripMenuItemClick(object sender, EventArgs e)
		{
			DisassemblerForm form;
			
			form = new DisassemblerForm(myEngine);
			
			form.Show();
		}
		
		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			MessageBox.Show("pGBA v0.1 alpha \n(C) Normmatt 2008","About");
		}
	}
}
