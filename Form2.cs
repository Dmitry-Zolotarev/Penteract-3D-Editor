using SharpGL.SceneGraph;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Penteract
{
    public partial class Form2 : Form
    {
        public string FilePath;
        private int Language = 0;
        private Color backColor = Color.Black, objectColor = Color.FromArgb(204, 255, 204);
        private Form1 Main;//Main window of this program
        
        public Form2() { }

        public Form2(Form1 main)
        {
            Main = main;
            KeyPreview = true;
            InitializeComponent();
            var folderPath = Path.Combine(Main.userFolder, "Documents", "Penteract");
            Directory.CreateDirectory(folderPath);
            FilePath = Path.Combine(folderPath, "Settings.xml");
            //Loading setting from the file
            try {
                var settings = XElement.Load(FilePath);
                Language = (int)settings.Attribute("Language");
                showAxes.Checked = (bool)settings.Attribute("showAxes");
                showGrid.Checked = (bool)settings.Attribute("showGrid");
                var xBackColor = settings.Element("BackColor");
                backColor = Color.FromArgb((int)xBackColor.Attribute("R"), (int)xBackColor.Attribute("G"), (int)xBackColor.Attribute("B"));
                var xObjectColor = settings.Element("ObjectColor");
                objectColor = Color.FromArgb((int)xObjectColor.Attribute("R"), (int)xObjectColor.Attribute("G"), (int)xObjectColor.Attribute("B"));
            }
            catch (Exception) { SaveSettings(); }
           
        }
        private void Form2_Shown(object sender, EventArgs e)
        {
            selectLanguage.SelectedIndex = Language;
            setObjectColor.BackColor = objectColor;
            setBackground.BackColor = backColor;
            SetLabels();
        }
        private void SetLabels()
        {
            langLabel.Text = Main.localizedLabels[85];
            backColorLabel.Text = Main.localizedLabels[87] + new Vertex(backColor.R, backColor.G, backColor.B).ToString();
            colorLabel.Text = Main.localizedLabels[88] + new Vertex(objectColor.R, objectColor.G, objectColor.B).ToString();
            showAxes.Text = Main.localizedLabels[36] + Main.localizedLabels[123];
            showGrid.Text = Main.localizedLabels[37] + Main.localizedLabels[123];
            this.Text = "Penteract – " + Main.localizedLabels[16].ToLower();
        }
        private void SaveSettings()
        {
            try {
                var settings = new XElement("Settings");
                settings.Add(new XAttribute("Language", Language));
                settings.Add(new XAttribute("showAxes", showAxes.Checked));
                settings.Add(new XAttribute("showGrid", showGrid.Checked));
                var xBackColor = new XElement("BackColor");
                xBackColor.Add(new XAttribute("R", backColor.R));
                xBackColor.Add(new XAttribute("G", backColor.G));
                xBackColor.Add(new XAttribute("B", backColor.B));
                var xObjectColor = new XElement("ObjectColor");
                xObjectColor.Add(new XAttribute("R", objectColor.R));
                xObjectColor.Add(new XAttribute("G", objectColor.G));
                xObjectColor.Add(new XAttribute("B", objectColor.B));
                settings.Add(xBackColor);
                settings.Add(xObjectColor);
                File.WriteAllText(FilePath, $"{settings}");
                Refresh();
            }
            catch(Exception) { }
            
        }
        public void UpdateLanguage(object sender, EventArgs e)
        {//Changing a language of program interface
            Language = selectLanguage.SelectedIndex;
            Main.PreviousLanguage = Main.Language;
            Main.Language = Language;
            SaveSettings();
            Main.SetLocale(Main);
            Main.selectObject();
            Main.UpdateInterface(sender, e);
            if (Main.editor.Visible) Main.editor.UpdateLists();
            SetLabels();
        }
        private void setBackground_Click(object sender, EventArgs e)
        {//Changing a default background color
            colorDialog1.Color = backColor;
            colorDialog1.ShowDialog();
            backColor = colorDialog1.Color;
            setBackground.BackColor = backColor;
            backColorLabel.Text = Main.localizedLabels[87] + new Vertex(backColor.R, backColor.G, backColor.B).ToString();
            SaveSettings();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e) => Main.Select();

        private void showAxes_CheckedChanged(object sender, EventArgs e) => SaveSettings();

        private void showGrid_CheckedChanged(object sender, EventArgs e) => SaveSettings();

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Close();
        }

        private void setObjectColor_Click(object sender, EventArgs e)
        {//Changing a default object color
            colorDialog1.Color = objectColor;
            colorDialog1.ShowDialog();
            objectColor = colorDialog1.Color;
            setObjectColor.BackColor = objectColor;
            colorLabel.Text = Main.localizedLabels[88] + new Vertex(objectColor.R, objectColor.G, objectColor.B).ToString();
            SaveSettings();
        }
    }
}
