/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/13/2008
 * Time: 2:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
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
			Scrn = new Bitmap(240, 160, PixelFormat.Format32bppRgb);
			while(i++!=239) Scrn.SetPixel(i,0,Color.FromArgb(0,255,0));
			i=0;
			while(i++!=239) Scrn.SetPixel(i,159,Color.FromArgb(255,0,255));
			i=0;
			while(i++!=159) Scrn.SetPixel(0,i,Color.FromArgb(255,0,255));
			i=0;
			while(i++!=159) Scrn.SetPixel(239,i,Color.FromArgb(255,0,0));
			scrnBox.Image = Scrn;
			
			myEngine.myARM.Emulate(1);
		}
	}
}
