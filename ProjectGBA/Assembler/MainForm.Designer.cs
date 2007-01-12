/*
 * Created by SharpDevelop.
 * User: Spikeman
 * Date: 1/10/2007
 * Time: 7:08 PM
 * 
 */
namespace ProjectGBA
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
			this.tCode = new System.Windows.Forms.TextBox();
			this.bAssemble = new System.Windows.Forms.Button();
			this.bDebug = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tCode
			// 
			this.tCode.AcceptsTab = true;
			this.tCode.Location = new System.Drawing.Point(12, 12);
			this.tCode.MaxLength = 10000000;
			this.tCode.Multiline = true;
			this.tCode.Name = "tCode";
			this.tCode.Size = new System.Drawing.Size(268, 220);
			this.tCode.TabIndex = 0;
			// 
			// bAssemble
			// 
			this.bAssemble.Location = new System.Drawing.Point(12, 238);
			this.bAssemble.Name = "bAssemble";
			this.bAssemble.Size = new System.Drawing.Size(75, 23);
			this.bAssemble.TabIndex = 1;
			this.bAssemble.Text = "Assemble";
			this.bAssemble.UseVisualStyleBackColor = true;
			this.bAssemble.Click += new System.EventHandler(this.BAssembleClick);
			// 
			// bDebug
			// 
			this.bDebug.Location = new System.Drawing.Point(93, 238);
			this.bDebug.Name = "bDebug";
			this.bDebug.Size = new System.Drawing.Size(75, 23);
			this.bDebug.TabIndex = 2;
			this.bDebug.Text = "Debug";
			this.bDebug.UseVisualStyleBackColor = true;
			this.bDebug.Click += new System.EventHandler(this.BDebugClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.bDebug);
			this.Controls.Add(this.bAssemble);
			this.Controls.Add(this.tCode);
			this.Name = "MainForm";
			this.Text = "ProjectGBA";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button bDebug;
		private System.Windows.Forms.Button bAssemble;
		private System.Windows.Forms.TextBox tCode;
	}
}
