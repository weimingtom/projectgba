/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/14/2008
 * Time: 5:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace pGBA
{
	/// <summary>
	/// Description of Engine.
	/// </summary>
	public class Engine
	{
		//Universal Constants
		public Armcpu myCPU;
		public Memory myMemory;
		public Logging myLog;
		public Gfx myGfx;
		public bool emulate;
		public Bitmap Scrn;
		
		public Engine()
		{
			Scrn = new Bitmap(240, 160, PixelFormat.Format16bppRgb555);
			
			myCPU = new Armcpu(this);
            myMemory = new Memory(this);
			myLog = new Logging();
			myGfx = new Gfx(this);
			emulate=false;
		}

        public void Reset()
        {
            myGfx.Reset();
            myMemory.Reset();
            myCPU.Reset();
        }
	}
}
