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
        public int inVCount = 0;
        private uint dispCnt = 0;
        private ushort dispStat = 0;
        private int[] bgx = new int[2], bgy = new int[2];
		
		public Gfx(Engine engine)
		{
			myEngine = engine;
		}

        public void Reset()
        {
            dispCnt = 0;
            inVblank = 0;
            inHblank = 0;
            curLine = 0;

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
			//RenderFrame();
            inVblank = 1;

            dispStat = myEngine.myMemory.ReadShort(0x04000004);
            dispStat &= 0xFFFF;
            dispStat |= 1;
            myEngine.myMemory.WriteShort(0x04000004, dispStat);

            //Check V-Blank IRQ is Enabled
            if ((dispStat & (1 << 3)) != 0)
            {
                myEngine.myCPU.Interrupt(myEngine.myCPU.IRQ_VBLANK);
            }
        }

        private void LeaveVBlank()
        {
            inVblank = 0;

            dispStat = myEngine.myMemory.ReadShort(0x04000004);
            dispStat &= 0xFFFF;
            myEngine.myMemory.WriteShort(0x04000004, dispStat);

            bgx[0] = (int)myEngine.myMemory.ReadWord(0x04000028);
            bgx[1] = (int)myEngine.myMemory.ReadWord(0x04000038);
            bgy[0] = (int)myEngine.myMemory.ReadWord(0x0400002C);
            bgy[1] = (int)myEngine.myMemory.ReadWord(0x0400003C);
        }
        
		public void EnterHBlank()
        {
            dispStat = myEngine.myMemory.ReadShort(0x04000004);
            dispStat &= 0xFFFE; //Clear old flag is there is one
            dispStat |= (1<<1);
            myEngine.myMemory.WriteShort(0x04000004, dispStat);

            // Advance the bgx registers
            for (int bg = 0; bg <= 1; bg++)
            {
                short dmx = (short)myEngine.myMemory.ReadShort(0x04000022 + (uint)bg * 0x10);
                short dmy = (short)myEngine.myMemory.ReadShort(0x04000026 + (uint)bg * 0x10);
                bgx[bg] += dmx;
                bgy[bg] += dmy;
            }

            if (curLine < 160)
            {
                //Check H-Blank IRQ is Enabled
                if ((dispStat & (1 << 4)) != 0)
                {
                    myEngine.myCPU.Interrupt(myEngine.myCPU.IRQ_HBLANK);
                }
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

            //Best update vcount
            myEngine.myMemory.WriteShort(0x04000006, (ushort)curLine);

            // Check for vblank
            if (curLine == 160)
            {
                EnterVBlank();
            }
            else if (curLine == 0)
            {
                LeaveVBlank();
            }

            dispStat = myEngine.myMemory.ReadShort(0x04000004);
            if ((dispStat >> 8) == curLine)
            {
                dispStat &= 0xFFFB;
                dispStat |= (1 << 2);
                myEngine.myMemory.WriteShort(0x04000004, dispStat);
                if ((dispStat & (1 << 5)) != 0)
                {
                    myEngine.myCPU.Interrupt(myEngine.myCPU.IRQ_VCOUNT);
                    inVCount = 1;
                }
                else
                    inVCount = 0;
            }
            else
            {
                dispStat &= 0xFFFB;
                myEngine.myMemory.WriteShort(0x04000004, dispStat);
            }

            dispStat &= 0xFFFE;
            myEngine.myMemory.WriteShort(0x04000004, dispStat);
        }
		
		public void RenderLine()
		{
			if (curLine < 160)
            {
                dispCnt = myEngine.myMemory.ReadWord(0x04000000);

                if ((dispCnt & (1 << 7)) != 0)
                {
                    ForceBlank();
                }
                else
                {
                    switch (this.dispCnt & 0x7)
                    {
                        case 0:
                            RenderMode0Line(); 
                            //myEngine.myLog.WriteLog("IMPLEMENT MODE 0\n");
                            break;
                        case 1:
                            //RenderMode1Line(); 
                            myEngine.myLog.WriteLog("IMPLEMENT MODE 1\n");
                            break;
                        case 2:
                            //RenderMode2Line(); 
                            myEngine.myLog.WriteLog("IMPLEMENT MODE 2\n");
                            break;
                        case 3:
                            RenderMode3Line();
                            break;
                        case 4:
                            RenderMode4Line(); 
                            //myEngine.myLog.WriteLog("IMPLEMENT MODE 4\n");
                            break;
                        case 5:
                            RenderMode5Line(); 
                            //myEngine.myLog.WriteLog("IMPLEMENT MODE 5\n");
                            break;
                    }
                }
            }
		}
		
		public void RenderFrame()
		{
			if (curLine == 160)
            {
                dispCnt = myEngine.myMemory.ReadWord(0x04000000);

                if ((dispCnt & (1 << 7)) != 0)
                {
                    ForceBlank();
                }
                else
                {
                    //RenderMode3();
                }
            }
		}

        public void ForceBlank()
        {
            uint offset = (uint)(0x06000000);
            for (int y = 0; y < 160; y++)
            {
                for (int x = 0; x < 240; x++)
                {
                    myEngine.Scrn.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    offset += 2;
                }
            }
        }

        public void RenderMode0Line()
        {
            for (int pri = 3; pri >= 0; pri--)
            {
                for (int i = 3; i >= 0; i--)
                {
                    if ((dispCnt & (1 << (8 + i))) != 0)
                    {
                        ushort bgcnt = myEngine.myMemory.ReadShort(0x04000008 + 0x2 * (uint)i);

                        if ((bgcnt & 0x3) == pri)
                        {
                            //RenderTextBg(i);
                            agb_draw_mode0_bg(i);
                        }
                    }
                }

                //DrawSprites(pri);
            }
        }

		public void RenderMode3Line()
		{
            if ((dispCnt & (1 << 10)) != 0)
            {
                for (int xx = 0; xx < 240; xx++)
                {
                    uint adr = (uint)(0x06000000 + ((xx * 2) + curLine * 240));
                    ushort pixel = myEngine.myMemory.ReadShort(adr);
                    int r = ((pixel >> 0) & 31) * 8;
                    int g = ((pixel >> 5) & 31) * 8;
                    int b = ((pixel >> 10) & 31) * 8;
                    myEngine.Scrn.SetPixel(xx, curLine, Color.FromArgb(0, r, g, b));
                }
            }
		}

        public void RenderMode4Line()
        {
            if ((dispCnt & (1 << 10)) != 0)
            {
                uint baseIdx = 0;
                if ((dispCnt & (1 << 4)) == 1 << 4) baseIdx = 0xA000;

                int x = bgx[0];
                int y = bgy[0];
                short dx = (short)myEngine.myMemory.ReadShort(0x04000020);
                short dy = (short)myEngine.myMemory.ReadShort(0x04000024);
                for (int i = 0; i < 240; i++)
                {
                    int ax = ((int)x) >> 8;
                    int ay = ((int)y) >> 8;
                    if (ax >= 0 && ax < 240 && ay >= 0 && ay < 160)
                    {
                        uint adr = (uint)(0x06000000 + baseIdx + (ay * 240) + ax);
                        uint palAdr = (uint)(0x05000000 + (myEngine.myMemory.ReadByte(adr) * 2));
                        ushort palette = myEngine.myMemory.ReadShort(palAdr);
                        int r = ((palette >> 0) & 31) * 8;
                        int g = ((palette >> 5) & 31) * 8;
                        int b = ((palette >> 10) & 31) * 8;
                        myEngine.Scrn.SetPixel(ax, ay, Color.FromArgb(0, r, g, b));
                    }
                    x += dx;
                    y += dy;
                }
            }
        }

        public void RenderMode5Line()
        {
            if ((dispCnt & (1 << 10)) != 0)
            {
                int baseIdx = 0;
                if ((dispCnt & (1 << 4)) == 1 << 4) baseIdx += 160 * 128 * 2;

                for (int xx = 0; xx < 160; xx++)
                {
                    uint adr = (uint)(0x06000000 + baseIdx + ((xx * 2) + curLine * 160));
                    ushort pixel = myEngine.myMemory.ReadShort(adr);
                    int r = ((pixel >> 0) & 31) * 8;
                    int g = ((pixel >> 5) & 31) * 8;
                    int b = ((pixel >> 10) & 31) * 8;
                    myEngine.Scrn.SetPixel(xx, curLine, Color.FromArgb(0, r, g, b));
                }
            }
        }

        public void agb_draw_mode0_bg(int bg)
        {	//mode 0 タイル方式
            int map_x, map_y, tile_x, tile_y;
            int base_char, base_map;
            byte char_ram;
            int map_offset;

            short bgcnt = (short)myEngine.myMemory.ReadShort((uint)(0x04000008 + 2*bg));
            uint palAdr = (uint)(0x05000000);
            ushort palette = myEngine.myMemory.ReadShort(palAdr);

            base_char = ((bgcnt >> 2) & 3) << 14;	//16KB単位
            base_map = ((bgcnt >> 8) & 0x1F) << 11;	//2KB単位
            for (map_y = 0; map_y < 20; map_y++)
            {
                for (map_x = 0; map_x < 30; map_x++)
                {
                    map_offset = myEngine.myMemory.ReadByte((uint)(0x06000000+base_map + (map_x + map_y * (((bgcnt & (1<<14))!=0) ? 64 : 32)) * 2));
                    char_ram = myEngine.myMemory.ReadByte((uint)(0x06000000+map_offset * 64 + base_char));
                    for (tile_y = 0; tile_y < 8; tile_y++)
                    {
                        for (tile_x = 0; tile_x < 8; tile_x++)
                        {
                            if (char_ram!=0)
                            {
                                palette = myEngine.myMemory.ReadShort((uint)(palAdr++ + char_ram*2));
                                int r = ((palette >> 0) & 31) * 8;
                                int g = ((palette >> 5) & 31) * 8;
                                int b = ((palette >> 10) & 31) * 8;
                                myEngine.Scrn.SetPixel(tile_x + map_x * 8, (tile_y + map_y * 8), Color.FromArgb(0, r, g, b));
                            }
                        }
                    }
                }
            }
        }
    }
}
