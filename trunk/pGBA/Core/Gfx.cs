/*
 * Created by SharpDevelop.
 * User: user
 * Date: 30/08/2008
 * Time: 11:53 p.m.
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
	/// Description of Graphics.
	/// </summary>
	public class Gfx
	{
		private Engine myEngine;
		public int curLine = 0;
        public int inHblank = 0;
        public int inVblank = 0;
		
		public Gfx(Engine engine)
		{
			myEngine = engine;
		}

        public void Reset()
        {
            for (int y = 0; y < 160; y++)
            {
                for (int x = 0; x < 240; x++)
                {
                    myEngine.Scrn.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }
        }

		private void EnterVBlank()
        {
			RenderFrame();
            inVblank = 1;
        }

        private void LeaveVBlank()
        {
            inVblank = 0;
        }
        
		public void EnterHBlank()
        {
            if (curLine < 160)
            {
            }
        }
		
		public void LeaveHBlank()
        {
            // Move to the next line
            curLine++;

            if (curLine >= 228)
            {
                // Start again at the beginning
                curLine = 0;
            }

            // Check for vblank
            if (curLine == 160)
            {
                EnterVBlank();
            }
            else if (curLine == 0)
            {
                LeaveVBlank();
            }
        }
		
		public void RenderLine()
		{
			if (curLine < 160)
            {
                RenderMode3Line(curLine);
            }
		}
		
		public void RenderFrame()
		{
			if (curLine == 160)
            {
                RenderMode3();
            }
		}
		
		public void RenderMode3()
		{
            uint offset = (uint)(0x06000000);
			for (int y = 0; y < 160; y++)
            {
             	for (int x = 0; x < 240; x++)
	            {
                    //uint offset = (uint)(0x06000000 + ((x*2) + (y*2) * 240));
                    //myEngine.myLog.WriteLog("pixel from 0x" + Convert.ToString((uint)(0x06000000 + (x + y * 160)),16));
                    int pixel = myEngine.myMemory.ReadShort(offset);
                    int r = ((pixel >> 0) & 31) * 8;
                    int g = ((pixel >> 5) & 31) * 8;
                    int b = ((pixel >> 10) & 31) * 8;
                    myEngine.Scrn.SetPixel(x, y, Color.FromArgb(r, g, b));
                    offset += 2;
	        	}
        	}
		}
		
		public void RenderMode3Line(int line)
		{
			for (int xx = 0; xx < 240; xx++)
	        {
				uint adr = (uint)(0x06000000+((xx*2)+line*160));
				ushort pixel = myEngine.myMemory.ReadShort(adr);
				int r = ((pixel >> 0) & 31) * 8;
             	int g = ((pixel >> 5) & 31) * 8;
             	int b = ((pixel >> 10) & 31) * 8;
             	myEngine.Scrn.SetPixel(xx,line,Color.FromArgb(0,r,g,b));
	        }
		}
	}
}
