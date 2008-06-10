/*
 * Created by SharpDevelop.
 * User: user
 * Date: 6/10/2008
 * Time: 4:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace pGBA
{
	/// <summary>
	/// Description of LogWindow.
	/// </summary>
	public partial class LogWindow : Form
	{
		private string log;
		private Engine myEngine;
		
		public LogWindow(Engine engine)
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
		}
		
		private void TimerTick(object sender, EventArgs e)
		{
			log = myEngine.myLog.ReturnLog();
			logBox.Text = log;
		}
	}
}
