/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/23/2008
 * Time: 12:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace pGBA
{
	partial class MemoryEditor
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
			this.MemList = new System.Windows.Forms.ComboBox();
			this.closeBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.memText = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// MemList
			// 
			this.MemList.FormattingEnabled = true;
			this.MemList.Location = new System.Drawing.Point(74, 12);
			this.MemList.Name = "MemList";
			this.MemList.Size = new System.Drawing.Size(121, 21);
			this.MemList.TabIndex = 0;
			this.MemList.SelectedIndexChanged += new System.EventHandler(this.MemListSelectedIndexChanged);
			// 
			// closeBtn
			// 
			this.closeBtn.Location = new System.Drawing.Point(480, 309);
			this.closeBtn.Name = "closeBtn";
			this.closeBtn.Size = new System.Drawing.Size(94, 27);
			this.closeBtn.TabIndex = 2;
			this.closeBtn.Text = "&Close";
			this.closeBtn.UseVisualStyleBackColor = true;
			this.closeBtn.Click += new System.EventHandler(this.CloseBtnClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Memory";
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.LargeChange = 0;
			this.vScrollBar1.Location = new System.Drawing.Point(551, 39);
			this.vScrollBar1.Maximum = 400;
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(20, 264);
			this.vScrollBar1.SmallChange = 0;
			this.vScrollBar1.TabIndex = 4;
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VScrollBar1Scroll);
			// 
			// memText
			// 
			this.memText.BackColor = System.Drawing.SystemColors.ControlDark;
			this.memText.Location = new System.Drawing.Point(12, 39);
			this.memText.Name = "memText";
			this.memText.Size = new System.Drawing.Size(539, 264);
			this.memText.TabIndex = 5;
			this.memText.Paint += new System.Windows.Forms.PaintEventHandler(this.MemTextPaint);
			this.memText.MouseDown += new System.Windows.Forms.MouseEventHandler(this.memText_MouseDown);
			this.memText.MouseUp += new System.Windows.Forms.MouseEventHandler(this.memText_MouseUp);
			// 
			// MemoryEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(586, 345);
			this.Controls.Add(this.memText);
			this.Controls.Add(this.vScrollBar1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.closeBtn);
			this.Controls.Add(this.MemList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MemoryEditor";
			this.Text = "MemoryEditor";
			this.Load += new System.EventHandler(this.MemoryEditorLoad);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ComboBox MemList;
		private System.Windows.Forms.Panel memText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button closeBtn;
	}
}
