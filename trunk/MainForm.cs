/*
 * Created by SharpDevelop.
 * User: Spikeman
 * Date: 1/10/2007
 * Time: 7:08 PM
 * 
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectGBA
{

	public partial class MainForm : Form
	{
		
		#region Defines
		
		char[] whitespace = new char[] {' ', '	'};
		char[] comment = new char[] {';'};
		
		#endregion
		
		#region Generated Stuff
		
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		public MainForm()
		{

			InitializeComponent();
			
		}
		
		#endregion
		
		void BAssembleClick(object sender, EventArgs e)
		{
			string[] lines = tCode.Text.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
			foreach(string line in lines) {
				string tmp = line;
				int c = tmp.IndexOfAny(comment);		//only accepts chars, so can't do C-style comments like this
				if(c != -1)
					tmp = tmp.Substring(0, c);		//strip comments
				tmp = tmp.Trim(whitespace);
				System.Diagnostics.Trace.WriteLine(tmp);
			}
		}
		
	}
}
