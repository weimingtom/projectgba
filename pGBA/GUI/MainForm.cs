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
using System.Threading;

namespace pGBA
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private Engine myEngine;
		private Thread executionThread = null;
		
		int vramCycles = 0;
        bool inHblank = false;
			
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			myEngine = new Engine();
			
			Monitor.Enter(this);
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			executionThread = new Thread(RunEmulationLoop);
            executionThread.Start();

            // Wait for the initialization to complete
            Monitor.Wait(this);
		}
		
		public void UpdateScreen()
		{
			if(myEngine.myGfx.curLine <= 160)
				scrnBox.Image = myEngine.Scrn;
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			int i=0;
			UpdateScreen();
			while(i++!=239) myEngine.Scrn.SetPixel(i,0,Color.FromArgb(0,255,0));
			i=0;
			while(i++!=239) myEngine.Scrn.SetPixel(i,159,Color.FromArgb(255,0,255));
			i=0;
			while(i++!=159) myEngine.Scrn.SetPixel(0,i,Color.FromArgb(255,255,255));
			i=0;
			while(i++!=159) myEngine.Scrn.SetPixel(239,i,Color.FromArgb(0,255,255));
			//scrnBox.Image = myEngine.Scrn;
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
			
			Monitor.Enter(this);
            Monitor.Exit(this);
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
		
		void MemoryEditorToolStripMenuItemClick(object sender, EventArgs e)
		{
			MemoryEditor form;
			
			form = new MemoryEditor(myEngine);
			
			form.Show();
		}
		
		void LogWindowToolStripMenuItemClick(object sender, EventArgs e)
		{
			LogWindow form;
			
			form = new LogWindow(myEngine);
			
			form.Show();
		}
		
		void RunToolStripMenuItemClick(object sender, EventArgs e)
		{
			myEngine.emulate = true;
			timer.Start();
			
			Monitor.Pulse(this);
            Monitor.Exit(this);
		}
		
		void PauseToolStripMenuItemClick(object sender, EventArgs e)
		{
			myEngine.emulate = false;
			timer.Stop();
			
			Monitor.Enter(this);
		}
		
		void TimerTick(object sender, EventArgs e)
		{
			if(myEngine.myGfx.curLine < 160)
				UpdateScreen();
		}
		
		void RunEmulationLoop()
		{			
			UpdateScreen();
			
			lock (this)
            {
                Monitor.Pulse(this);
                Monitor.Wait(this);
                
				if(myEngine.emulate)
				{   
					for (int frame = 0; frame < 60; frame++)
					{
						const int numSteps = 2284;
		                const int cycleStep = 123;
		
		                for (int i = 0; i < numSteps; i++)
		                {
		                	if (vramCycles <= 0)
		                    {
			                	if (inHblank)
			                    {
			                    	vramCycles += 960;
			                    	//LeaveHBlank here
			                    	myEngine.myGfx.LeaveHBlank();
			                        inHblank = false;
			                    }
			                    else
			                    {
			                    	vramCycles += 272;
			                    	//RenderLine here
			                    	myEngine.myGfx.RenderLine();
			                    	//EnterHBlank here
			                    	myEngine.myGfx.EnterHBlank();
			                    	
			                        inHblank = true;
			                    }
		                	}
		                	
		                	myEngine.myCPU.Emulate(cycleStep);
							
							vramCycles -= cycleStep;
		                }
					}
				}
				Monitor.Pulse(this);
			}
		}
	}
}
