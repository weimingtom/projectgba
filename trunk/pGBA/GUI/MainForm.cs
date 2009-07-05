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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace pGBA
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
        [StructLayout(LayoutKind.Sequential)]
        public struct PeekMsg
        {
            public IntPtr hWnd;
            public Message msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd,
                 uint messageFilterMin, uint messageFilterMax, uint flags);

		private Engine myEngine;
		private Thread executionThread = null;
		
		int vramCycles = 0;
			
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			myEngine = new Engine();
			
			//Monitor.Enter(this);
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			//executionThread = new Thread(RunEmulationLoop);
            //executionThread.Start();

            // Wait for the initialization to complete
            //Monitor.Wait(this);
		}
		
		public void UpdateScreen()
		{
            if (myEngine.myGfx.curLine <= 160)
            {
                scrnBox.Image = myEngine.Scrn;
            }
		}

        private void OnApplicationIdle(object sender, EventArgs e)
        {
            if (myEngine.myMemory.romLoaded == false || !Focused)
            {
                return;
            }
            PeekMsg msg;
            while (!PeekMessage(out msg, IntPtr.Zero, 0, 0, 0))
            {
                RunEmulationLoop();
                UpdateScreen();
            }
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
            Application.Idle += OnApplicationIdle;
		}
		
		void OpenGBAToolStripMenuItemClick(object sender, EventArgs e)
		{
            myEngine.emulate = false;
            timer.Stop();
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
                myEngine.Reset();
			}
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Close();
			
			//Monitor.Enter(this);
            //Monitor.Exit(this);
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
			
			//Monitor.Pulse(this);
            //Monitor.Exit(this);
		}
		
		void PauseToolStripMenuItemClick(object sender, EventArgs e)
		{
			myEngine.emulate = false;
			timer.Stop();
			
			//Monitor.Enter(this);
		}
		
		void TimerTick(object sender, EventArgs e)
		{
            RunEmulationLoop();

			//if(myEngine.myGfx.curLine < 160)
			//	UpdateScreen();
		}
		
		void RunEmulationLoop()
		{					
			//lock (this)
            {
                //Monitor.Pulse(this);
                //Monitor.Wait(this);
                
				if(myEngine.emulate)
				{   
					//for (int frame = 0; frame < 60; frame++)
					{
						const int numSteps = 2284;
		                const int cycleStep = 123;
		
		                for (int i = 0; i < numSteps; i++)
		                {
		                	if (vramCycles <= 0)
		                    {
                                if (myEngine.myGfx.inHblank == 1)
			                    {
			                    	vramCycles += 960;
			                    	//LeaveHBlank here
			                    	myEngine.myGfx.LeaveHBlank();
                                    myEngine.myGfx.inHblank = 0;
			                    }
			                    else
			                    {
			                    	vramCycles += 272;
			                    	//RenderLine here
			                    	//myEngine.myGfx.RenderLine();
			                    	//EnterHBlank here
			                    	myEngine.myGfx.EnterHBlank();

                                    myEngine.myGfx.inHblank = 1;
			                    }
		                	}
		                	
		                	myEngine.myCPU.Emulate(cycleStep);
							
							vramCycles -= cycleStep;
		                }

                        ushort vcount = myEngine.myMemory.ReadShort(0x04000006);
                        if (vcount > 227) vcount = 0; else vcount++;
                        myEngine.myMemory.WriteShort(0x04000006, vcount);
					}
				}
				//Monitor.Pulse(this);
			}
		}

        private void scrnBox_Click(object sender, EventArgs e)
        {

        }
	}
}
