/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/15/2008
 * Time: 8:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace pGBA
{
	partial class DisassemblerForm
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
			this.disasmList = new System.Windows.Forms.ListBox();
			this.regList = new System.Windows.Forms.ListBox();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.closeBtn = new System.Windows.Forms.Button();
			this.autoUpdate = new System.Windows.Forms.CheckBox();
			this.gotoPCBtn = new System.Windows.Forms.Button();
			this.refreshBtn = new System.Windows.Forms.Button();
			this.nextBtn = new System.Windows.Forms.Button();
			this.Nflag = new System.Windows.Forms.CheckBox();
			this.Zflag = new System.Windows.Forms.CheckBox();
			this.Cflag = new System.Windows.Forms.CheckBox();
			this.Vflag = new System.Windows.Forms.CheckBox();
			this.Tflag = new System.Windows.Forms.CheckBox();
			this.Fflag = new System.Windows.Forms.CheckBox();
			this.Iflag = new System.Windows.Forms.CheckBox();
			this.ModeTxt = new System.Windows.Forms.Label();
			this.ModeValue = new System.Windows.Forms.Label();
			this.editRegister = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// disasmList
			// 
			this.disasmList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.disasmList.FormattingEnabled = true;
			this.disasmList.Items.AddRange(new object[] {
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" "});
			this.disasmList.Location = new System.Drawing.Point(12, 34);
			this.disasmList.Name = "disasmList";
			this.disasmList.Size = new System.Drawing.Size(388, 264);
			this.disasmList.TabIndex = 0;
			// 
			// regList
			// 
			this.regList.FormattingEnabled = true;
			this.regList.Items.AddRange(new object[] {
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" ",
									" "});
			this.regList.Location = new System.Drawing.Point(445, 21);
			this.regList.Name = "regList";
			this.regList.Size = new System.Drawing.Size(136, 225);
			this.regList.TabIndex = 1;
			this.regList.DoubleClick += new System.EventHandler(this.regListDoubleClick);
			this.regList.Click += new System.EventHandler(this.regListClick);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.LargeChange = 34;
			this.vScrollBar1.Location = new System.Drawing.Point(399, 34);
			this.vScrollBar1.Maximum = 350;
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(18, 264);
			this.vScrollBar1.TabIndex = 2;
			this.vScrollBar1.Value = 158;
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VScrollBar1Scroll);
			// 
			// timer
			// 
			this.timer.Tick += new System.EventHandler(this.TimerTick);
			// 
			// closeBtn
			// 
			this.closeBtn.Location = new System.Drawing.Point(486, 324);
			this.closeBtn.Name = "closeBtn";
			this.closeBtn.Size = new System.Drawing.Size(95, 22);
			this.closeBtn.TabIndex = 3;
			this.closeBtn.Text = "Close";
			this.closeBtn.UseVisualStyleBackColor = true;
			this.closeBtn.Click += new System.EventHandler(this.CloseBtnClick);
			// 
			// autoUpdate
			// 
			this.autoUpdate.Location = new System.Drawing.Point(12, 304);
			this.autoUpdate.Name = "autoUpdate";
			this.autoUpdate.Size = new System.Drawing.Size(113, 18);
			this.autoUpdate.TabIndex = 4;
			this.autoUpdate.Text = "Automatic Update";
			this.autoUpdate.UseVisualStyleBackColor = true;
			// 
			// gotoPCBtn
			// 
			this.gotoPCBtn.Location = new System.Drawing.Point(12, 325);
			this.gotoPCBtn.Name = "gotoPCBtn";
			this.gotoPCBtn.Size = new System.Drawing.Size(95, 22);
			this.gotoPCBtn.TabIndex = 5;
			this.gotoPCBtn.Text = "Goto R15";
			this.gotoPCBtn.UseVisualStyleBackColor = true;
			// 
			// refreshBtn
			// 
			this.refreshBtn.Location = new System.Drawing.Point(166, 324);
			this.refreshBtn.Name = "refreshBtn";
			this.refreshBtn.Size = new System.Drawing.Size(95, 22);
			this.refreshBtn.TabIndex = 6;
			this.refreshBtn.Text = "Refresh";
			this.refreshBtn.UseVisualStyleBackColor = true;
			this.refreshBtn.Click += new System.EventHandler(this.refreshBtnClick);
			// 
			// nextBtn
			// 
			this.nextBtn.Location = new System.Drawing.Point(322, 325);
			this.nextBtn.Name = "nextBtn";
			this.nextBtn.Size = new System.Drawing.Size(95, 22);
			this.nextBtn.TabIndex = 7;
			this.nextBtn.Text = "Next";
			this.nextBtn.UseVisualStyleBackColor = true;
			this.nextBtn.Click += new System.EventHandler(this.NextBtnClick);
			// 
			// Nflag
			// 
			this.Nflag.Enabled = false;
			this.Nflag.Location = new System.Drawing.Point(445, 252);
			this.Nflag.Name = "Nflag";
			this.Nflag.Size = new System.Drawing.Size(41, 18);
			this.Nflag.TabIndex = 8;
			this.Nflag.Text = "N";
			this.Nflag.UseVisualStyleBackColor = true;
			// 
			// Zflag
			// 
			this.Zflag.Enabled = false;
			this.Zflag.Location = new System.Drawing.Point(445, 268);
			this.Zflag.Name = "Zflag";
			this.Zflag.Size = new System.Drawing.Size(41, 18);
			this.Zflag.TabIndex = 9;
			this.Zflag.Text = "Z";
			this.Zflag.UseVisualStyleBackColor = true;
			// 
			// Cflag
			// 
			this.Cflag.Enabled = false;
			this.Cflag.Location = new System.Drawing.Point(445, 283);
			this.Cflag.Name = "Cflag";
			this.Cflag.Size = new System.Drawing.Size(41, 18);
			this.Cflag.TabIndex = 10;
			this.Cflag.Text = "C";
			this.Cflag.UseVisualStyleBackColor = true;
			// 
			// Vflag
			// 
			this.Vflag.Enabled = false;
			this.Vflag.Location = new System.Drawing.Point(445, 299);
			this.Vflag.Name = "Vflag";
			this.Vflag.Size = new System.Drawing.Size(41, 18);
			this.Vflag.TabIndex = 11;
			this.Vflag.Text = "V";
			this.Vflag.UseVisualStyleBackColor = true;
			// 
			// Tflag
			// 
			this.Tflag.Enabled = false;
			this.Tflag.Location = new System.Drawing.Point(502, 283);
			this.Tflag.Name = "Tflag";
			this.Tflag.Size = new System.Drawing.Size(41, 18);
			this.Tflag.TabIndex = 14;
			this.Tflag.Text = "T";
			this.Tflag.UseVisualStyleBackColor = true;
			// 
			// Fflag
			// 
			this.Fflag.Enabled = false;
			this.Fflag.Location = new System.Drawing.Point(502, 268);
			this.Fflag.Name = "Fflag";
			this.Fflag.Size = new System.Drawing.Size(41, 18);
			this.Fflag.TabIndex = 13;
			this.Fflag.Text = "F";
			this.Fflag.UseVisualStyleBackColor = true;
			// 
			// Iflag
			// 
			this.Iflag.Enabled = false;
			this.Iflag.Location = new System.Drawing.Point(502, 252);
			this.Iflag.Name = "Iflag";
			this.Iflag.Size = new System.Drawing.Size(41, 18);
			this.Iflag.TabIndex = 12;
			this.Iflag.Text = "I";
			this.Iflag.UseVisualStyleBackColor = true;
			// 
			// ModeTxt
			// 
			this.ModeTxt.Location = new System.Drawing.Point(499, 301);
			this.ModeTxt.Name = "ModeTxt";
			this.ModeTxt.Size = new System.Drawing.Size(41, 16);
			this.ModeTxt.TabIndex = 15;
			this.ModeTxt.Text = "Mode: ";
			// 
			// ModeValue
			// 
			this.ModeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ModeValue.Location = new System.Drawing.Point(545, 301);
			this.ModeValue.Name = "ModeValue";
			this.ModeValue.Size = new System.Drawing.Size(26, 16);
			this.ModeValue.TabIndex = 16;
			this.ModeValue.Text = "1F";
			// 
			// editRegister
			// 
			this.editRegister.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.editRegister.Location = new System.Drawing.Point(492, -5);
			this.editRegister.MaxLength = 8;
			this.editRegister.Name = "editRegister";
			this.editRegister.Size = new System.Drawing.Size(92, 20);
			this.editRegister.TabIndex = 17;
			this.editRegister.Visible = false;
			this.editRegister.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.editRegister_KeyPress);
			// 
			// DisassemblerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(593, 350);
			this.Controls.Add(this.editRegister);
			this.Controls.Add(this.ModeValue);
			this.Controls.Add(this.ModeTxt);
			this.Controls.Add(this.Tflag);
			this.Controls.Add(this.Fflag);
			this.Controls.Add(this.Iflag);
			this.Controls.Add(this.Vflag);
			this.Controls.Add(this.Cflag);
			this.Controls.Add(this.Zflag);
			this.Controls.Add(this.Nflag);
			this.Controls.Add(this.nextBtn);
			this.Controls.Add(this.refreshBtn);
			this.Controls.Add(this.gotoPCBtn);
			this.Controls.Add(this.autoUpdate);
			this.Controls.Add(this.closeBtn);
			this.Controls.Add(this.vScrollBar1);
			this.Controls.Add(this.regList);
			this.Controls.Add(this.disasmList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "DisassemblerForm";
			this.Text = "Disassembler";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox editRegister;
		private System.Windows.Forms.CheckBox Nflag;
		private System.Windows.Forms.CheckBox Zflag;
		private System.Windows.Forms.CheckBox Cflag;
		private System.Windows.Forms.CheckBox Vflag;
		private System.Windows.Forms.Label ModeValue;
		private System.Windows.Forms.Label ModeTxt;
		private System.Windows.Forms.CheckBox Iflag;
		private System.Windows.Forms.CheckBox Fflag;
		private System.Windows.Forms.CheckBox Tflag;
		private System.Windows.Forms.Button gotoPCBtn;
		private System.Windows.Forms.Button refreshBtn;
		private System.Windows.Forms.Button nextBtn;
		private System.Windows.Forms.CheckBox autoUpdate;
		private System.Windows.Forms.Button closeBtn;
		private System.Windows.Forms.ListBox disasmList;
		private System.Windows.Forms.ListBox regList;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.VScrollBar vScrollBar1;
	}
}
