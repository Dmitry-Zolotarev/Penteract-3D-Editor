
using System.Windows.Forms;
using System.Collections.Generic;
using SharpGL.SceneGraph;
using SharpGL;
using System;

namespace Penteract
{//A window for editing vertices and polygons of all objects
    public partial class Form3 : Form
    {
        private Form1 Main;//Link to the main form
        private Timer updater = new Timer();
        public Form3()
        {
            InitializeComponent();
        }
        public Form3(Form1 main) {
            InitializeComponent();
            
            KeyPreview = true;
            Main = main;
            UpdateLists();
            updater.Interval = 50;
            updater.Tick += UpdateInterface; 
            updater.Start();
        }
        private void UpdateInterface(object sender, EventArgs e)
        {
            if (scaleRadio2.Checked) {
                xNumeric2.Value = 1; yNumeric2.Value = 1; zNumeric2.Value = 1;
            }
            else {
                xNumeric2.Value = 0; yNumeric2.Value = 0; zNumeric2.Value = 0;
            }
            shiftVertex.Enabled = listVertices.SelectedItems.Count > 0;
            polygonTransform.Enabled = listPolygons.SelectedItems.Count > 0;
        }
        public void UpdateLists()
        {//Updating a lists of vertices and polygons
            try
            {
                moveRadio2.Text = Main.localizedLabels[2];
                rotateRadio2.Text = Main.localizedLabels[3];
                scaleRadio2.Text = Main.localizedLabels[4];
                this.Text = "Penteract – " + Main.localizedLabels[106].ToLower();
                shiftVertex.Text = Main.localizedLabels[105];
                polygonTransform.Text = Main.localizedLabels[104];
                label4.Text = Main.localizedLabels[107];
                label13.Text = Main.localizedLabels[108];
                editVertices.Text = Main.localizedLabels[110];
                editPolygons.Text = Main.localizedLabels[109];
                divide.Text = Main.localizedLabels[111];
                extrude.Text = Main.localizedLabels[129];
                toolTip1.SetToolTip(divide, Main.localizedLabels[112]);
                toolTip1.SetToolTip(extrude, Main.localizedLabels[130]);
                listVertices.Items.Clear();
                listPolygons.Items.Clear();
                for (int i = 0; i < Main.allObjects.Count; i++)
                {
                    for (int j = 1; j <= Main.allObjects[i].Vertices.Count; j++)
                    {
                        listVertices.Items.Add(i + 1 + ". " + Main.allObjects[i].Name + " - " + Main.localizedLabels[69] + ' ' + j);
                    }
                    for (int j = 1; j <= Main.allObjects[i].Polygons.Count; j++)
                    {
                        listPolygons.Items.Add(i + 1 + ". " + Main.allObjects[i].Name + " - " + Main.localizedLabels[70] + ' ' + j);
                    }
                }
                if (scaleRadio2.Checked)  {
                    xNumeric2.Value = 1; yNumeric2.Value = 1; zNumeric2.Value = 1;
                }
                else  {
                    xNumeric2.Value = 0; yNumeric2.Value = 0; zNumeric2.Value = 0;
                }
            }
            catch(Exception)  {  }
        }
        private void xNumeric_ValueChanged(object sender, EventArgs e) => TransformVertices();
        private void yNumeric_ValueChanged(object sender, EventArgs e) => TransformVertices();
        private void zNumeric_ValueChanged(object sender, EventArgs e) => TransformVertices();
        private void TransformVertices()
        {//Transforming the selected vertices to the values ​​from the fields on the right
            try  {
                Vertex d = new Vertex((float)xNumeric.Value, (float)yNumeric.Value, (float)zNumeric.Value);
                xNumeric.Value = 0; yNumeric.Value = 0; zNumeric.Value = 0;//Reset field values
                var modifiedObjects = new HashSet<Object3D>();
                foreach (var item in listVertices.SelectedItems)
                {
                    var data = item.ToString().Split('.', ' ');
                    int i = int.Parse(data[0]) - 1, j = int.Parse(data[data.Length - 1]) - 1;
                    Main.allObjects[i].Vertices[j] += d;
                    modifiedObjects.Add(Main.allObjects[i]);
                }
            }
            catch (Exception) { } 
        }
        private void Transform2()
        {//Transforming the selected polygons to the values ​​from the fields on the right
            try
            {
                Vertex v = new Vertex((float)xNumeric2.Value, (float)yNumeric2.Value, (float)zNumeric2.Value);
                foreach (var item in listPolygons.SelectedItems)
                {
                    var data = item.ToString().Split('.', ' ');
                    int i = int.Parse(data[0]) - 1, j = int.Parse(data[data.Length - 1]) - 1;
                    if (moveRadio2.Checked && v.Magnitude() > 0)
                    {
                        Main.allObjects[i].Polygons[j].Move(v);
                    }
                    else if (rotateRadio2.Checked && v.Magnitude() > 0)
                    {
                        Main.allObjects[i].Polygons[j].Rotate(v);
                    }
                    else if (scaleRadio2.Checked)
                    {
                        Main.allObjects[i].Polygons[j].Scale(v);
                    }
                }
                Main.Backup();
            }
            catch(Exception) { }
            
        }
        public void RenderSelected(OpenGL gl)
        {//Render the selection of selected elements
            try
            {
                gl.Disable(OpenGL.GL_LIGHTING);
                if (tabControl1.SelectedTab == editVertices) {
                    gl.Color(0.9f, 0, 0);
                    gl.Begin(OpenGL.GL_POINTS);
                    foreach (var item in listVertices.SelectedItems)
                    {
                        var data = item.ToString().Split('.', ' ');
                        int i = int.Parse(data[0]) - 1, j = int.Parse(data[data.Length - 1]) - 1;
                        gl.Vertex(Main.allObjects[i].Vertices[j]);
                    }
                    gl.End();
                }
                gl.Begin(OpenGL.GL_TRIANGLES);
                if(tabControl1.SelectedTab == editPolygons) 
                {
                    foreach (var item in listPolygons.SelectedItems)
                    {
                        var data = item.ToString().Split('.', ' ');
                        int i = int.Parse(data[0]) - 1, j = int.Parse(data[data.Length - 1]) - 1;
                        gl.Color(Main.LightColor(Main.allObjects[i].color));
                        Main.allObjects[i].Polygons[j].Draw(OpenGL.GL_TRIANGLES, null);
                    }
                }   
            }
            catch(Exception) { }
        }
        private void xNumeric_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.Enter) Main.Backup();
        } 
        private void yNumeric_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) Main.Backup();
        }
        private void zNumeric_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) Main.Backup();
        }
        private void xNumeric2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) Transform2();
        }
        private void xNumeric2_Leave(object sender, EventArgs e) => Transform2();
        private void yNumeric2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) Transform2();
        }
        private void yNumeric2_Leave(object sender, EventArgs e) => Transform2();
        private void zNumeric2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) Transform2();
        }
        private void zNumeric2_Leave(object sender, EventArgs e) => Transform2();
        private void xNumeric_Leave(object sender, EventArgs e) => Main.Backup();
        private void yNumeric_Leave(object sender, EventArgs e) => Main.Backup();
        private void zNumeric_Leave(object sender, EventArgs e) => Main.Backup();
        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {//Control from keyboard
            try
            {
                if (e.Control) switch (e.KeyCode)
                    {
                        case Keys.A:
                            if (tabControl1.SelectedIndex == 0)
                                for (int i = 0; i < listVertices.Items.Count; i++) listVertices.SetSelected(i, true);
                            else
                                for (int i = 0; i < listPolygons.Items.Count; i++) listPolygons.SetSelected(i, true);
                            break;
                        case Keys.Z:
                            if (e.Shift) Main.Discard(1);//Redo
                            else Main.Discard(-1);//Undo
                            UpdateLists();
                            break;
                    }
            }
            catch(Exception) { }       
        }
        private void divide_Click(object sender, EventArgs e)
        {//Dividing n-angle polygons into n parts by a central vertex
            try
            {
                if (listPolygons.SelectedItems.Count > 0)
                {
                    var dividedPolygons = new List<Polygon>();

                    foreach (var item in listPolygons.SelectedItems)
                    {
                        var data = item.ToString().Split('.', ' ');
                        int i = int.Parse(data[0]) - 1, j = int.Parse(data[data.Length - 1]) - 1;
                        dividedPolygons.Add(Main.allObjects[i].Polygons[j]);
                        Main.allObjects[i].DividePolygon(j);
                    }
                    foreach (var polygon in dividedPolygons) polygon.Host.Polygons.Remove(polygon);
                    Main.Backup(); UpdateLists();
                    try
                    {
                        Main.selectObject();
                    }
                    catch (Exception) { };
                }
                else MessageBox.Show(Main.localizedLabels[113], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch(Exception) { } 
        }
        private void extrude_Click(object sender, EventArgs e)
        {
            try   {
                foreach (var item in listPolygons.SelectedItems)
                {
                    var data = item.ToString().Split('.', ' ');
                    int i = int.Parse(data[0]) - 1, j = int.Parse(data[data.Length - 1]) - 1;
                    Main.allObjects[i].ExtrudePolygon(j);
                }
                //Saving selection
                var selected = new HashSet<object>();
                
                foreach (var i in listPolygons.SelectedItems) selected.Add(i);
                Main.Backup(); UpdateLists();
                for(int i = 0; i < listPolygons.Items.Count; i++)
                    if (selected.Contains(listPolygons.Items[i])) listPolygons.SetSelected(i, true);
                try
                {
                    Main.selectObject();
                }
                catch (Exception) { };
            }
            catch (Exception) { }
        }
        private void listPolygons_DoubleClick(object sender, EventArgs e) => listPolygons.ClearSelected();
        private void listVertices_DoubleClick(object sender, EventArgs e) => listVertices.ClearSelected();
        //Update polygon selection
        private void listVertices_SelectedIndexChanged(object sender, EventArgs e) {
            Main.showSelection.Checked = true;
            Main.updated = true;
        }

        private void listPolygons_SelectedIndexChanged(object sender, EventArgs e) {
            Main.showSelection.Checked = true;
            Main.updated = true;
        }
        private void Form3_FormClosed(object sender, FormClosedEventArgs e) {
            Main.updated = true;
            Main.Select();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) => Main.updated = true;

        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Close();
            
        }
    }
}
