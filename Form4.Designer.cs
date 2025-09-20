
namespace Penteract
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Bar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // Bar
            // 
            this.Bar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Bar.Location = new System.Drawing.Point(0, 0);
            this.Bar.Margin = new System.Windows.Forms.Padding(0);
            this.Bar.Name = "Bar";
            this.Bar.Size = new System.Drawing.Size(624, 81);
            this.Bar.TabIndex = 41;
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 81);
            this.Controls.Add(this.Bar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form5";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.ProgressBar Bar;
    }
}