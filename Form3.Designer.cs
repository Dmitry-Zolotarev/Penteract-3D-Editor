
namespace Penteract
{
    partial class Form3
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.editVertices = new System.Windows.Forms.TabPage();
            this.shiftVertex = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.zNumeric = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.yNumeric = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.xNumeric = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.listVertices = new System.Windows.Forms.ListBox();
            this.editPolygons = new System.Windows.Forms.TabPage();
            this.polygonTransform = new System.Windows.Forms.GroupBox();
            this.extrude = new System.Windows.Forms.Button();
            this.divide = new System.Windows.Forms.Button();
            this.moveRadio2 = new System.Windows.Forms.RadioButton();
            this.rotateRadio2 = new System.Windows.Forms.RadioButton();
            this.scaleRadio2 = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.zNumeric2 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.yNumeric2 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.xNumeric2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.listPolygons = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1.SuspendLayout();
            this.editVertices.SuspendLayout();
            this.shiftVertex.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xNumeric)).BeginInit();
            this.editPolygons.SuspendLayout();
            this.polygonTransform.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zNumeric2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNumeric2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xNumeric2)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.editVertices);
            this.tabControl1.Controls.Add(this.editPolygons);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(624, 321);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabControl1_KeyDown);
            // 
            // editVertices
            // 
            this.editVertices.BackColor = System.Drawing.Color.WhiteSmoke;
            this.editVertices.Controls.Add(this.shiftVertex);
            this.editVertices.Controls.Add(this.label13);
            this.editVertices.Controls.Add(this.listVertices);
            this.editVertices.Location = new System.Drawing.Point(4, 22);
            this.editVertices.Name = "editVertices";
            this.editVertices.Padding = new System.Windows.Forms.Padding(3);
            this.editVertices.Size = new System.Drawing.Size(616, 295);
            this.editVertices.TabIndex = 0;
            this.editVertices.Text = "Изменение вершин";
            // 
            // shiftVertex
            // 
            this.shiftVertex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.shiftVertex.Controls.Add(this.panel4);
            this.shiftVertex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.shiftVertex.Location = new System.Drawing.Point(437, 14);
            this.shiftVertex.Name = "shiftVertex";
            this.shiftVertex.Size = new System.Drawing.Size(176, 264);
            this.shiftVertex.TabIndex = 30;
            this.shiftVertex.TabStop = false;
            this.shiftVertex.Text = " Сдвиг выбранных вершин";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.label14);
            this.panel4.Controls.Add(this.zNumeric);
            this.panel4.Controls.Add(this.label15);
            this.panel4.Controls.Add(this.yNumeric);
            this.panel4.Controls.Add(this.label16);
            this.panel4.Controls.Add(this.xNumeric);
            this.panel4.Location = new System.Drawing.Point(3, 21);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(165, 79);
            this.panel4.TabIndex = 14;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label14.ForeColor = System.Drawing.Color.Blue;
            this.label14.Location = new System.Drawing.Point(-2, 57);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(24, 16);
            this.label14.TabIndex = 14;
            this.label14.Text = "dZ";
            // 
            // zNumeric
            // 
            this.zNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zNumeric.DecimalPlaces = 3;
            this.zNumeric.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.zNumeric.Location = new System.Drawing.Point(29, 55);
            this.zNumeric.Maximum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.zNumeric.Minimum = new decimal(new int[] {
            8000,
            0,
            0,
            -2147483648});
            this.zNumeric.Name = "zNumeric";
            this.zNumeric.Size = new System.Drawing.Size(135, 21);
            this.zNumeric.TabIndex = 13;
            this.zNumeric.ValueChanged += new System.EventHandler(this.zNumeric_ValueChanged);
            this.zNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.zNumeric_KeyDown);
            this.zNumeric.Leave += new System.EventHandler(this.zNumeric_Leave);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label15.ForeColor = System.Drawing.Color.ForestGreen;
            this.label15.Location = new System.Drawing.Point(-2, 31);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(25, 16);
            this.label15.TabIndex = 12;
            this.label15.Text = "dY";
            // 
            // yNumeric
            // 
            this.yNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.yNumeric.DecimalPlaces = 3;
            this.yNumeric.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.yNumeric.Location = new System.Drawing.Point(28, 29);
            this.yNumeric.Maximum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.yNumeric.Minimum = new decimal(new int[] {
            8000,
            0,
            0,
            -2147483648});
            this.yNumeric.Name = "yNumeric";
            this.yNumeric.Size = new System.Drawing.Size(135, 21);
            this.yNumeric.TabIndex = 11;
            this.yNumeric.ValueChanged += new System.EventHandler(this.yNumeric_ValueChanged);
            this.yNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.yNumeric_KeyDown);
            this.yNumeric.Leave += new System.EventHandler(this.yNumeric_Leave);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.ForeColor = System.Drawing.Color.Red;
            this.label16.Location = new System.Drawing.Point(-2, 5);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(24, 16);
            this.label16.TabIndex = 10;
            this.label16.Text = "dX";
            // 
            // xNumeric
            // 
            this.xNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.xNumeric.DecimalPlaces = 3;
            this.xNumeric.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.xNumeric.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.xNumeric.Location = new System.Drawing.Point(28, 3);
            this.xNumeric.Maximum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.xNumeric.Minimum = new decimal(new int[] {
            8000,
            0,
            0,
            -2147483648});
            this.xNumeric.Name = "xNumeric";
            this.xNumeric.Size = new System.Drawing.Size(135, 21);
            this.xNumeric.TabIndex = 9;
            this.xNumeric.ValueChanged += new System.EventHandler(this.xNumeric_ValueChanged);
            this.xNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.xNumeric_KeyDown);
            this.xNumeric.Leave += new System.EventHandler(this.xNumeric_Leave);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new System.Drawing.Point(8, 3);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(189, 13);
            this.label13.TabIndex = 11;
            this.label13.Text = "Список вершин всех объектов";
            // 
            // listVertices
            // 
            this.listVertices.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listVertices.FormattingEnabled = true;
            this.listVertices.ItemHeight = 15;
            this.listVertices.Location = new System.Drawing.Point(11, 19);
            this.listVertices.Name = "listVertices";
            this.listVertices.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listVertices.Size = new System.Drawing.Size(420, 259);
            this.listVertices.TabIndex = 0;
            this.listVertices.SelectedIndexChanged += new System.EventHandler(this.listVertices_SelectedIndexChanged);
            this.listVertices.DoubleClick += new System.EventHandler(this.listVertices_DoubleClick);
            // 
            // editPolygons
            // 
            this.editPolygons.BackColor = System.Drawing.Color.WhiteSmoke;
            this.editPolygons.Controls.Add(this.polygonTransform);
            this.editPolygons.Controls.Add(this.label4);
            this.editPolygons.Controls.Add(this.listPolygons);
            this.editPolygons.Location = new System.Drawing.Point(4, 22);
            this.editPolygons.Name = "editPolygons";
            this.editPolygons.Padding = new System.Windows.Forms.Padding(3);
            this.editPolygons.Size = new System.Drawing.Size(616, 295);
            this.editPolygons.TabIndex = 1;
            this.editPolygons.Text = "Изменение полигонов";
            // 
            // polygonTransform
            // 
            this.polygonTransform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.polygonTransform.Controls.Add(this.extrude);
            this.polygonTransform.Controls.Add(this.divide);
            this.polygonTransform.Controls.Add(this.moveRadio2);
            this.polygonTransform.Controls.Add(this.rotateRadio2);
            this.polygonTransform.Controls.Add(this.scaleRadio2);
            this.polygonTransform.Controls.Add(this.panel1);
            this.polygonTransform.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.polygonTransform.Location = new System.Drawing.Point(437, 16);
            this.polygonTransform.Name = "polygonTransform";
            this.polygonTransform.Size = new System.Drawing.Size(176, 266);
            this.polygonTransform.TabIndex = 35;
            this.polygonTransform.TabStop = false;
            this.polygonTransform.Text = "Трансформирование выбранных полигонов";
            // 
            // extrude
            // 
            this.extrude.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.extrude.Location = new System.Drawing.Point(7, 228);
            this.extrude.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.extrude.Name = "extrude";
            this.extrude.Size = new System.Drawing.Size(159, 30);
            this.extrude.TabIndex = 37;
            this.extrude.Text = "Экструдировать";
            this.extrude.UseVisualStyleBackColor = true;
            this.extrude.Click += new System.EventHandler(this.extrude_Click);
            // 
            // divide
            // 
            this.divide.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.divide.Location = new System.Drawing.Point(7, 184);
            this.divide.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.divide.Name = "divide";
            this.divide.Size = new System.Drawing.Size(159, 40);
            this.divide.TabIndex = 36;
            this.divide.Text = "Разделить на равные части ";
            this.divide.UseVisualStyleBackColor = true;
            this.divide.Click += new System.EventHandler(this.divide_Click);
            // 
            // moveRadio2
            // 
            this.moveRadio2.AutoSize = true;
            this.moveRadio2.Checked = true;
            this.moveRadio2.Location = new System.Drawing.Point(9, 34);
            this.moveRadio2.Name = "moveRadio2";
            this.moveRadio2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.moveRadio2.Size = new System.Drawing.Size(108, 19);
            this.moveRadio2.TabIndex = 2;
            this.moveRadio2.TabStop = true;
            this.moveRadio2.Text = "Перемещение";
            this.moveRadio2.UseVisualStyleBackColor = true;
            // 
            // rotateRadio2
            // 
            this.rotateRadio2.AutoSize = true;
            this.rotateRadio2.Location = new System.Drawing.Point(9, 57);
            this.rotateRadio2.Name = "rotateRadio2";
            this.rotateRadio2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rotateRadio2.Size = new System.Drawing.Size(139, 19);
            this.rotateRadio2.TabIndex = 1;
            this.rotateRadio2.Text = "Поворот в градусах";
            this.rotateRadio2.UseVisualStyleBackColor = true;
            // 
            // scaleRadio2
            // 
            this.scaleRadio2.AutoSize = true;
            this.scaleRadio2.Location = new System.Drawing.Point(9, 80);
            this.scaleRadio2.Name = "scaleRadio2";
            this.scaleRadio2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.scaleRadio2.Size = new System.Drawing.Size(135, 19);
            this.scaleRadio2.TabIndex = 0;
            this.scaleRadio2.Text = "Масштабирование";
            this.scaleRadio2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.zNumeric2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.yNumeric2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.xNumeric2);
            this.panel1.Location = new System.Drawing.Point(6, 101);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(160, 79);
            this.panel1.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(-2, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 16);
            this.label1.TabIndex = 14;
            this.label1.Text = "dZ";
            // 
            // zNumeric2
            // 
            this.zNumeric2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zNumeric2.DecimalPlaces = 3;
            this.zNumeric2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.zNumeric2.Location = new System.Drawing.Point(24, 55);
            this.zNumeric2.Maximum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.zNumeric2.Minimum = new decimal(new int[] {
            8000,
            0,
            0,
            -2147483648});
            this.zNumeric2.Name = "zNumeric2";
            this.zNumeric2.Size = new System.Drawing.Size(135, 21);
            this.zNumeric2.TabIndex = 13;
            this.zNumeric2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.zNumeric2_KeyDown);
            this.zNumeric2.Leave += new System.EventHandler(this.zNumeric2_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.ForestGreen;
            this.label2.Location = new System.Drawing.Point(-2, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 16);
            this.label2.TabIndex = 12;
            this.label2.Text = "dY";
            // 
            // yNumeric2
            // 
            this.yNumeric2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.yNumeric2.DecimalPlaces = 3;
            this.yNumeric2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.yNumeric2.Location = new System.Drawing.Point(23, 29);
            this.yNumeric2.Maximum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.yNumeric2.Minimum = new decimal(new int[] {
            8000,
            0,
            0,
            -2147483648});
            this.yNumeric2.Name = "yNumeric2";
            this.yNumeric2.Size = new System.Drawing.Size(135, 21);
            this.yNumeric2.TabIndex = 11;
            this.yNumeric2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.yNumeric2_KeyDown);
            this.yNumeric2.Leave += new System.EventHandler(this.yNumeric2_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(-2, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "dX";
            // 
            // xNumeric2
            // 
            this.xNumeric2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.xNumeric2.DecimalPlaces = 3;
            this.xNumeric2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.xNumeric2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.xNumeric2.Location = new System.Drawing.Point(23, 3);
            this.xNumeric2.Maximum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.xNumeric2.Minimum = new decimal(new int[] {
            8000,
            0,
            0,
            -2147483648});
            this.xNumeric2.Name = "xNumeric2";
            this.xNumeric2.Size = new System.Drawing.Size(135, 21);
            this.xNumeric2.TabIndex = 9;
            this.xNumeric2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.xNumeric2_KeyDown);
            this.xNumeric2.Leave += new System.EventHandler(this.xNumeric2_Leave);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(8, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(207, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Список полигонов всех объектов";
            // 
            // listPolygons
            // 
            this.listPolygons.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listPolygons.FormattingEnabled = true;
            this.listPolygons.ItemHeight = 15;
            this.listPolygons.Location = new System.Drawing.Point(9, 26);
            this.listPolygons.Name = "listPolygons";
            this.listPolygons.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listPolygons.Size = new System.Drawing.Size(420, 259);
            this.listPolygons.TabIndex = 31;
            this.listPolygons.SelectedIndexChanged += new System.EventHandler(this.listPolygons_SelectedIndexChanged);
            this.listPolygons.DoubleClick += new System.EventHandler(this.listPolygons_DoubleClick);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 3000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 321);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form3";
            this.Opacity = 0.92D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Редактирование вершин и полигонов";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form3_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form3_KeyDown);
            this.tabControl1.ResumeLayout(false);
            this.editVertices.ResumeLayout(false);
            this.editVertices.PerformLayout();
            this.shiftVertex.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xNumeric)).EndInit();
            this.editPolygons.ResumeLayout(false);
            this.editPolygons.PerformLayout();
            this.polygonTransform.ResumeLayout(false);
            this.polygonTransform.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zNumeric2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNumeric2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xNumeric2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage editVertices;
        private System.Windows.Forms.TabPage editPolygons;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox shiftVertex;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown xNumeric;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown zNumeric;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown yNumeric;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox polygonTransform;
        private System.Windows.Forms.RadioButton moveRadio2;
        public System.Windows.Forms.RadioButton rotateRadio2;
        private System.Windows.Forms.RadioButton scaleRadio2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown zNumeric2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown yNumeric2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown xNumeric2;
        private System.Windows.Forms.Button divide;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.ListBox listVertices;
        public System.Windows.Forms.ListBox listPolygons;
        private System.Windows.Forms.Button extrude;
    }
}