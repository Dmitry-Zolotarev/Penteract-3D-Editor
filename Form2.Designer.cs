
namespace Penteract
{
    partial class Form2
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
            this.langLabel = new System.Windows.Forms.Label();
            this.selectLanguage = new System.Windows.Forms.ComboBox();
            this.backColorLabel = new System.Windows.Forms.Label();
            this.setBackground = new System.Windows.Forms.Button();
            this.colorLabel = new System.Windows.Forms.Label();
            this.setObjectColor = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.showAxes = new System.Windows.Forms.CheckBox();
            this.showGrid = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // langLabel
            // 
            this.langLabel.AutoSize = true;
            this.langLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.langLabel.Location = new System.Drawing.Point(10, 8);
            this.langLabel.Name = "langLabel";
            this.langLabel.Size = new System.Drawing.Size(126, 16);
            this.langLabel.TabIndex = 0;
            this.langLabel.Text = "Язык интерфейса";
            this.langLabel.UseWaitCursor = true;
            // 
            // selectLanguage
            // 
            this.selectLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.selectLanguage.FormattingEnabled = true;
            this.selectLanguage.Items.AddRange(new object[] {
            "Русский",
            "English"});
            this.selectLanguage.Location = new System.Drawing.Point(12, 26);
            this.selectLanguage.Name = "selectLanguage";
            this.selectLanguage.Size = new System.Drawing.Size(320, 24);
            this.selectLanguage.TabIndex = 1;
            this.selectLanguage.UseWaitCursor = true;
            this.selectLanguage.SelectedIndexChanged += new System.EventHandler(this.UpdateLanguage);
            // 
            // backColorLabel
            // 
            this.backColorLabel.AutoSize = true;
            this.backColorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.backColorLabel.Location = new System.Drawing.Point(12, 124);
            this.backColorLabel.Name = "backColorLabel";
            this.backColorLabel.Size = new System.Drawing.Size(181, 16);
            this.backColorLabel.TabIndex = 48;
            this.backColorLabel.Text = "Цвет фона по умолчанию: ";
            this.backColorLabel.UseWaitCursor = true;
            // 
            // setBackground
            // 
            this.setBackground.BackColor = System.Drawing.Color.Black;
            this.setBackground.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.setBackground.Location = new System.Drawing.Point(12, 142);
            this.setBackground.Name = "setBackground";
            this.setBackground.Size = new System.Drawing.Size(320, 23);
            this.setBackground.TabIndex = 49;
            this.setBackground.UseVisualStyleBackColor = false;
            this.setBackground.UseWaitCursor = true;
            this.setBackground.Click += new System.EventHandler(this.setBackground_Click);
            // 
            // colorLabel
            // 
            this.colorLabel.AutoSize = true;
            this.colorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.colorLabel.Location = new System.Drawing.Point(12, 168);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(209, 16);
            this.colorLabel.TabIndex = 51;
            this.colorLabel.Text = "Цвет объектов по умолчанию: ";
            this.colorLabel.UseWaitCursor = true;
            // 
            // setObjectColor
            // 
            this.setObjectColor.BackColor = System.Drawing.Color.White;
            this.setObjectColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.setObjectColor.Location = new System.Drawing.Point(12, 186);
            this.setObjectColor.Name = "setObjectColor";
            this.setObjectColor.Size = new System.Drawing.Size(320, 25);
            this.setObjectColor.TabIndex = 50;
            this.setObjectColor.UseVisualStyleBackColor = false;
            this.setObjectColor.UseWaitCursor = true;
            this.setObjectColor.Click += new System.EventHandler(this.setObjectColor_Click);
            // 
            // showAxes
            // 
            this.showAxes.AutoSize = true;
            this.showAxes.Checked = true;
            this.showAxes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showAxes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.showAxes.Location = new System.Drawing.Point(12, 67);
            this.showAxes.Name = "showAxes";
            this.showAxes.Size = new System.Drawing.Size(290, 20);
            this.showAxes.TabIndex = 53;
            this.showAxes.Text = "Отображать оси координат при запуске";
            this.showAxes.UseVisualStyleBackColor = true;
            this.showAxes.CheckedChanged += new System.EventHandler(this.showAxes_CheckedChanged);
            // 
            // showGrid
            // 
            this.showGrid.AutoSize = true;
            this.showGrid.Checked = true;
            this.showGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.showGrid.Location = new System.Drawing.Point(12, 90);
            this.showGrid.Name = "showGrid";
            this.showGrid.Size = new System.Drawing.Size(231, 20);
            this.showGrid.TabIndex = 52;
            this.showGrid.Text = "Отображать сетку при запуске";
            this.showGrid.UseVisualStyleBackColor = true;
            this.showGrid.CheckedChanged += new System.EventHandler(this.showGrid_CheckedChanged);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 221);
            this.Controls.Add(this.showAxes);
            this.Controls.Add(this.showGrid);
            this.Controls.Add(this.colorLabel);
            this.Controls.Add(this.setObjectColor);
            this.Controls.Add(this.backColorLabel);
            this.Controls.Add(this.setBackground);
            this.Controls.Add(this.selectLanguage);
            this.Controls.Add(this.langLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.Opacity = 0.96D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Параметры";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
            this.Shown += new System.EventHandler(this.Form2_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form2_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label langLabel;
        private System.Windows.Forms.ComboBox selectLanguage;
        private System.Windows.Forms.Label backColorLabel;
        private System.Windows.Forms.Button setBackground;
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.Button setObjectColor;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.CheckBox showAxes;
        private System.Windows.Forms.CheckBox showGrid;
    }
}