﻿/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/13/2008
 * Time: 2:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace pGBA
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.scrnBox = new System.Windows.Forms.PictureBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGBAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disassemblerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memoryEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.FPS_Indicator = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.scrnBox)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrnBox
            // 
            this.scrnBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scrnBox.Location = new System.Drawing.Point(0, 24);
            this.scrnBox.Name = "scrnBox";
            this.scrnBox.Size = new System.Drawing.Size(240, 160);
            this.scrnBox.TabIndex = 0;
            this.scrnBox.TabStop = false;
            this.scrnBox.Click += new System.EventHandler(this.scrnBox_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.emulationToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(240, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGBAToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openGBAToolStripMenuItem
            // 
            this.openGBAToolStripMenuItem.Name = "openGBAToolStripMenuItem";
            this.openGBAToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openGBAToolStripMenuItem.Text = "&Open GBA";
            this.openGBAToolStripMenuItem.Click += new System.EventHandler(this.OpenGBAToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // emulationToolStripMenuItem
            // 
            this.emulationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.pauseToolStripMenuItem});
            this.emulationToolStripMenuItem.Name = "emulationToolStripMenuItem";
            this.emulationToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.emulationToolStripMenuItem.Text = "&Emulation";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.runToolStripMenuItem.Text = "&Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.RunToolStripMenuItemClick);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.pauseToolStripMenuItem.Text = "&Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.PauseToolStripMenuItemClick);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disassemblerToolStripMenuItem,
            this.memoryEditorToolStripMenuItem,
            this.logWindowToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // disassemblerToolStripMenuItem
            // 
            this.disassemblerToolStripMenuItem.Name = "disassemblerToolStripMenuItem";
            this.disassemblerToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.disassemblerToolStripMenuItem.Text = "&Disassembler";
            this.disassemblerToolStripMenuItem.Click += new System.EventHandler(this.DisassemblerToolStripMenuItemClick);
            // 
            // memoryEditorToolStripMenuItem
            // 
            this.memoryEditorToolStripMenuItem.Name = "memoryEditorToolStripMenuItem";
            this.memoryEditorToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.memoryEditorToolStripMenuItem.Text = "&Memory Editor";
            this.memoryEditorToolStripMenuItem.Click += new System.EventHandler(this.MemoryEditorToolStripMenuItemClick);
            // 
            // logWindowToolStripMenuItem
            // 
            this.logWindowToolStripMenuItem.Name = "logWindowToolStripMenuItem";
            this.logWindowToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.logWindowToolStripMenuItem.Text = "&Log Window";
            this.logWindowToolStripMenuItem.Click += new System.EventHandler(this.LogWindowToolStripMenuItemClick);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FPS_Indicator});
            this.statusStrip.Location = new System.Drawing.Point(0, 184);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(240, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            this.statusStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.statusStrip_ItemClicked);
            // 
            // FPS_Indicator
            // 
            this.FPS_Indicator.Name = "FPS_Indicator";
            this.FPS_Indicator.Size = new System.Drawing.Size(29, 17);
            this.FPS_Indicator.Text = "FPS:";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "GBA Rom files (*.gba)|*.gba";
            // 
            // timer
            // 
            this.timer.Interval = 20;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 206);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.scrnBox);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "pGBA";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.scrnBox)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem emulationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem logWindowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem memoryEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem disassemblerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem openGBAToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel FPS_Indicator;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.PictureBox scrnBox;
	}
}
