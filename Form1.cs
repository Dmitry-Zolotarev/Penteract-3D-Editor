using Microsoft.VisualBasic;
using SharpGL;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Drawing.Imaging;
using System.Globalization;
using System.Threading.Tasks;

namespace Penteract
{
    //https://github.com/mtosity/hcmus-cs/tree/main - openGLControl was taken from this folder;
    
    public partial class Form1 : Form
    {
        Camera cam; //Main camera
        public string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //Folder of a current user
        public List<string> languages = new List<string>();
        public List<Object3D> allObjects = new List<Object3D>();//List of all objects in the 3D scene
        
        List<Object3D> clipBoard = new List<Object3D>();//Clipboard for copying and pasting
        List<State> ctrlZ = new List<State>();//Archive of all states of objects in the 3D scene for Undo and Redo
        
        private Timer updater = new Timer();//Timer for automatic interface update
        Point previousCursorPosition;
        public LightSource lightSource = new LightSource(new Vertex(10, 10, 10));//Light source
        public Color backroundColor = Color.Black, objectColor = Color.FromArgb(204, 255, 204);//Background color
        public Form2 settings;
        public Form3 editor = new Form3();
        public List<string> defaultLabels = null, localizedLabels = null;
        public bool ctrlPressed = false, changesSaved = true, updated = true;
        const int undo = -1, redo = 1, maxVertices = 100;
        int drawLatency = 0, undoRedo = 0;
        public int Language = 1, PreviousLanguage = 0;
        Vertex oldPosition, oldRotation;
        public string file_Path = "", exePath = AppDomain.CurrentDomain.BaseDirectory;
        float camMoveCount;
        Keys previousKey;
        public Form1()
        {
            languages.Add(exePath + "Locales\\russian.txt");
            languages.Add(exePath + "Locales\\english.txt");
            //Form initialization
            InitializeComponent();
            //Camera initialization
            Vertex eyeVertex = new Vertex(3, 3, 3), lookAt = new Vertex(0, 0, 0);
            cam = new Camera(screen3D.OpenGL, eyeVertex, lookAt);
            settings = new Form2(this);
            camMoveCount = 10f / screen3D.FrameRate;
            Language = SystemLanguage();
            ApplySettings();
            screen3D.ContextMenuStrip = contextMenuStrip2;
            currentObject.ContextMenuStrip = contextMenuStrip2;
            listShape.ContextMenuStrip = contextMenuStrip3;
            previousCursorPosition = Cursor.Position;
            screen3D.OpenGL.Enable(OpenGL.GL_TEXTURE_2D);
            screen3D.OpenGL.Enable(OpenGL.GL_NORMALIZE);
            lightX.Value = (decimal)lightSource.Position.X;
            lightY.Value = (decimal)lightSource.Position.Y;
            lightZ.Value = (decimal)lightSource.Position.Z;

            SetLocale(this); 

            Backup(); Discard(undo);
            changesSaved = true;
            updater.Interval = 60; // Interval in milliseconds
            updater.Tick += UpdateInterface;
            updater.Start();
            Refresh();
        }
        private int SystemLanguage()
        {//Reading a system language
            if (CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "ru") return 0;
            else return 1;
        }
        private void ApplySettings()
        {
            try {//Apply settings from file
                var xSettings = XElement.Load(settings.FilePath);
                Language = (int)xSettings.Attribute("Language");
                showAxes.Checked = (bool)xSettings.Attribute("showAxes");
                showGrid.Checked = (bool)xSettings.Attribute("showGrid");
                var xBackColor = xSettings.Element("BackColor");
                backroundColor = Color.FromArgb((int)xBackColor.Attribute("R"), (int)xBackColor.Attribute("G"), (int)xBackColor.Attribute("B"));
                var xObjectColor = xSettings.Element("ObjectColor");
                objectColor = Color.FromArgb((int)xObjectColor.Attribute("R"), (int)xObjectColor.Attribute("G"), (int)xObjectColor.Attribute("B"));
            }
            catch (Exception)
            {//If file don't exist, create it with default settings
                var xSettings = new XElement("Settings");
                xSettings.Add(new XAttribute("Language", Language));
                xSettings.Add(new XAttribute("showAxes", showAxes.Checked));
                xSettings.Add(new XAttribute("showGrid", showGrid.Checked));
                var xBackColor = new XElement("BackColor");
                xBackColor.Add(new XAttribute("R", backroundColor.R));
                xBackColor.Add(new XAttribute("G", backroundColor.G));
                xBackColor.Add(new XAttribute("B", backroundColor.B));
                var xObjectColor = new XElement("ObjectColor");
                xObjectColor.Add(new XAttribute("R", objectColor.R));
                xObjectColor.Add(new XAttribute("G", objectColor.G));
                xObjectColor.Add(new XAttribute("B", objectColor.B));
                xSettings.Add(xBackColor);
                xSettings.Add(xObjectColor);
                File.WriteAllText(settings.FilePath, $"{xSettings}");
            }
        }
        public void SetLocale(Control thisControl)
        {
            try
            {
                localizedLabels = File.ReadAllLines(languages[Language]).ToList();
                defaultLabels = File.ReadAllLines(languages[PreviousLanguage]).ToList();

                renderMode.Items.Clear();
                renderMode.Items.Add(localizedLabels[62]);
                renderMode.Items.Add(localizedLabels[63]);
                renderMode.Items.Add(localizedLabels[89]);

                verticesCount.Text = localizedLabels[124] + "\n" + totalVertices();
                polygonsCount.Text = localizedLabels[125] + "\n" + totalPolygons();
                int view = camViews.SelectedIndex;

                camViews.Items.Clear();
                for (int i = 0; i < 6; i++) camViews.Items.Add(localizedLabels[90 + i]);

                if (view >= 0) camViews.SelectedIndex = view;
                SetToolTips();//Creating tooltips
                LocalizeMenuItems();//Localizing menu items
                if (Language == PreviousLanguage) return;
                foreach (Control control in thisControl.Controls)
                {//Recursive localization of the containers
                    if (control.Text.Length > 2)
                        for (int i = 0; i < defaultLabels.Count; i++)
                            if (control.Text == defaultLabels[i])
                            {
                                control.Text = localizedLabels[i];
                                break;
                            }
                    if (control.HasChildren) SetLocale(control);
                    if (control is MenuStrip menuStrip)
                        foreach (ToolStripMenuItem item in menuStrip.Items) LocalizeUpperMenuItem(item);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void LocalizeUpperMenuItem(ToolStripMenuItem menuItem)
        {
            try
            {//Localize menuBar
                if (Language == PreviousLanguage) return;
                for (int i = 0; i < defaultLabels.Count; i++)
                    if (menuItem.Text == defaultLabels[i])
                    {
                        menuItem.Text = localizedLabels[i];
                        break;
                    }
                foreach (ToolStripMenuItem subItem in menuItem.DropDownItems) LocalizeUpperMenuItem(subItem);
            }
            catch (Exception) { }
        }
        public void LocalizeMenuItems()
        {
            try
            {
                autoSave.Text = save.Text.Split()[0] + " (Ctrl + S)";
                toolStripMenuItem2.Text = localizedLabels[28];//Add a figure
                toolStripMenuItem3.Text = localizedLabels[29];//Cube
                toolStripMenuItem4.Text = localizedLabels[30];//Square
                toolStripMenuItem5.Text = localizedLabels[31];//Triangle
                toolStripMenuItem16.Text = localizedLabels[35];//Polygon (circle)
                toolStripMenuItem6.Text = localizedLabels[32];//Pyramid (cone)
                toolStripMenuItem7.Text = localizedLabels[33];//Prism (cylinder)
                toolStripMenuItem15.Text = localizedLabels[34];//Polyhedron (sphere)
                toolStripMenuItem20.Text = localizedLabels[77];//Ring (torus)
                contextMenuUndo.Text = localizedLabels[11];//Undo (Ctrl + Z)
                contextMenuRedo.Text = localizedLabels[12];//Redo (Ctrl + Shift + Z)
                toolStripMenuItem10.Text = localizedLabels[13];//Draw your object (Alt + N)
                rename2.Text = localizedLabels[41];//Rename (F2)
                rename3.Text = localizedLabels[41];//Rename (F2)
                modify3.Text = localizedLabels[103];//Shifting vertices and polygons (Alt + V)
                delete2.Text = localizedLabels[42];//Delete (Del) 
                deleteItem3.Text = localizedLabels[42];//Delete (Del)
                mergeItem.Text = localizedLabels[43];//Merge (Alt + M)
                copyItem.Text = localizedLabels[44];//Copy (Ctrl + C)
                pasteItem.Text = localizedLabels[45];//Paste (Ctrl + V)
                flipItem.Text = localizedLabels[134].Split()[0];//Flip Objects
                fillObjects.Text = localizedLabels[99];//Set color to objects
                selectTextureItem.Text = localizedLabels[24];//Set texture to objects(Alt + t)
                deleteTexture.Text = localizedLabels[25];//Delete texture (Alt + D)
                ItemFlipX.Text = localizedLabels[134] + 'X';
                ItemFlipY.Text = localizedLabels[134] + 'Y';
                ItemFlipZ.Text = localizedLabels[134] + 'Z';
            }
            catch (Exception) { }
        }
        public void UpdateInterface(object sender, EventArgs e)
        {//Updating interface elements
            try
            {
                if (comboShapes.Items.Count != allObjects.Count)
                {
                    comboShapes.Items.Clear();
                    if (editor.Visible) editor.UpdateLists();
                    for (int I = 0; I < allObjects.Count; I++) comboShapes.Items.Add((I + 1) + ". " + allObjects[I].Name);
                }
                if (listShape.Items.Count != allObjects.Count)
                {
                    listShape.Items.Clear();
                    if (editor.Visible) editor.UpdateLists();
                    for (int I = 0; I < allObjects.Count; I++) listShape.Items.Add((I + 1) + ". " + allObjects[I].Name);
                }
                this.Enabled = !settings.Visible;
                if (vertexPanel.ContainsFocus) updated = true;
                if (polygonPanel.ContainsFocus) updated = true;
                helpMenu.Enabled = !settings.Visible && !editor.Visible;
                settingsMenu.Enabled = !editor.Visible;
                if (allObjects.Count == 0) editor.Dispose();
                int i = comboShapes.SelectedIndex, j = comboVertices.SelectedIndex, k = comboPolygons.SelectedIndex;
                
                relativeTransform.Enabled = listShape.SelectedIndices.Count > 0;
                fillObjects.Enabled = listShape.SelectedIndices.Count > 0;
                bool objectSelected = allObjects.Count > 0;
                vertexPanel.Enabled = objectSelected;
                polygonPanel.Enabled = objectSelected;
                contextMenuStrip2.Enabled = objectSelected;
                transformation.Enabled = objectSelected;
                currentObject.Enabled = objectSelected;
                renderMode.Enabled = objectSelected;
                label17.Enabled = objectSelected;
                colorLabel.Visible = objectSelected;
                setObjectColor.Visible = objectSelected;
                screen3D.ContextMenuStrip = contextMenuStrip1;
                drawPanel.Visible = objectSelected && allObjects[i].drawMode;
                menuBarUndo.Enabled = undoRedo > 0;
                menuBarRedo.Enabled = undoRedo < ctrlZ.Count - 1;
                contextMenuUndo.Enabled = undoRedo > 0;
                contextMenuRedo.Enabled = undoRedo < ctrlZ.Count - 1;
                if (listShape.SelectedIndices.Count > 0) 
                {
                    listShape.ContextMenuStrip = contextMenuStrip3;
                }
                else
                {
                    listShape.ContextMenuStrip = null;
                    toolTip2.Hide(listShape);
                }
                showSelection.Enabled = allObjects.Count > 0;
                if (objectSelected) setObjectColor.BackColor = allObjects[i].GetColor();
                deleteTexture.Enabled = false;
                pasteItem.Enabled = clipBoard.Count > 0;
                foreach (int I in listShape.SelectedIndices)
                {
                    if (allObjects[I].TextureFile.Length > 1)
                    {
                        deleteTexture.Enabled = true;
                        break;
                    }
                }
                modify.Enabled = allObjects.Count > 0;
                modify2.Visible = allObjects.Count > 0;
                modify3.Visible = allObjects.Count > 0;
                if (objectSelected)
                {
                    //currentObject.Text = comboShapes.Text;
                    //Update coordinates of the selected vertex
                    if (j >= 0)
                    {
                        X1.Value = (decimal)(float)allObjects[i].Vertices[j].X;
                        Y1.Value = (decimal)(float)allObjects[i].Vertices[j].Y;
                        Z1.Value = (decimal)(float)allObjects[i].Vertices[j].Z;
                    }
                    //Update coordinates of the selected polygon
                    if (k >= 0)
                    {
                        Vertex center = allObjects[i].Polygons[k].Center();
                        X2.Value = (decimal)center.X;
                        Y2.Value = (decimal)center.Y;
                        Z2.Value = (decimal)center.Z;
                    }
                    updateNumericFields(i);
                }
                else
                {
                    currentObject.Text = localizedLabels[21];
                    X1.Value = 0; Y1.Value = 0; Z1.Value = 0;
                    X2.Value = 0; Y2.Value = 0; Z2.Value = 0;
                    updateNumericFields(-1);
                }
                if (moveRadio2.Checked)
                {
                    xNumeric2.Value = 0; yNumeric2.Value = 0; zNumeric2.Value = 0;
                }
                else if (rotateRadio2.Checked)
                {
                    xNumeric2.Value = 0; yNumeric2.Value = 0; zNumeric2.Value = 0;
                }
                else if (scaleRadio2.Checked)
                {
                    xNumeric2.Value = 1; yNumeric2.Value = 1; zNumeric2.Value = 1;
                }
            }
            catch (Exception ex) { }
        }
        private void SetToolTips()
        {//Seting a hints
            toolTip3.SetToolTip(screen3D, localizedLabels[101]);
            toolTip2.SetToolTip(lightSwitch, localizedLabels[39] + " (Alt + L)");
            toolTip2.SetToolTip(setBackground, localizedLabels[54]);
            toolTip2.SetToolTip(setLightColor, localizedLabels[55]);
            toolTip2.SetToolTip(setObjectColor, localizedLabels[56]);
            toolTip2.SetToolTip(listShape, localizedLabels[57]);
            toolTip2.SetToolTip(drawPanel, localizedLabels[60]);
            toolTip2.SetToolTip(renderMode, localizedLabels[61]);
            
        }
        public void Backup()
        {//Writing the current state of the 3D scene to the backup list used in Undo and Redo
            changesSaved = false;//Parameter for checking whether changes are saved before closing the program
            try
            {//Cleaning up so the backup list doesn't get full
                
                ctrlZ.Add(new State(this));
                verticesCount.Text = localizedLabels[124] + "\n" + totalVertices();
                polygonsCount.Text = localizedLabels[125] + "\n" + totalPolygons();
                undoRedo++; updated = true;
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }
        public void Discard(int direction)
        {
            try
            {//If direction = -1 then undo, else if direction = 1 then redo
                if (undoRedo + direction < 0 || undoRedo + direction >= ctrlZ.Count) return;
                undoRedo += direction;
                ClearAll();
                foreach (var obj in ctrlZ[undoRedo].allObjects)
                {
                    allObjects.Add(new Object3D(obj));//×òåíèå îáúåêòîâ èç ðåçåðâíîé êîïèè
                    comboShapes.Items.Add(allObjects.Count + ". " + obj.Name);
                }
                lightSource.lightColor = ctrlZ[undoRedo].lightColor;//Öâåò è ÿðêîñòü îñâåùåíèÿ
                Vertex backColor = ctrlZ[undoRedo].backgroundColor;//Öâåò çàäíåãî ôîíà
                backroundColor = Color.FromArgb((int)(backColor.X * 255f), (int)(backColor.Y * 255f), (int)(backColor.Z * 255f));
                setBackground.BackColor = backroundColor;
                LightButtonColor(lightSource.GetColor());
                try {
                    comboShapes.SelectedIndex = ctrlZ[undoRedo].selectedIndex;
                }
                catch {
                    comboShapes.SelectedIndex = comboShapes.Items.Count - 1;
                }
                if (editor.Visible) editor.UpdateLists();
                updated = true;
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        private void Copy(int i)
        {//Copy an object to the clipboard
            try {
                clipBoard.Add(new Object3D(allObjects[i]));
            }
            catch (Exception) { }
        }
        private void Paste()
        {
            foreach (var obj in clipBoard) addFigure(obj, obj.Position, false);
            Backup();
        }

        //Adding a figures:
        private void Cube_Click(object sender, EventArgs e) {
            var cube = new Prism(screen3D.OpenGL, 4, objectColor).ToCube();
            cube.Name = localizedLabels[29];
            addFigure(cube, cam.LookAt, true);
        }
        private void Square_Click(object sender, EventArgs e) => addFigure(new Square(screen3D.OpenGL, objectColor, localizedLabels[30]), cam.LookAt, true);

        private void Triangle_Click(object sender, EventArgs e) => addFigure(new Triangle(screen3D.OpenGL, objectColor, localizedLabels[31]), cam.LookAt, true);
        private void Pyramid_Click(object sender, EventArgs e)
        {
            try
            {
                int n = int.Parse(Interaction.InputBox(localizedLabels[75] + localizedLabels[116], "Penteract", "4"));
                if (n < 3)
                {
                    MessageBox.Show(localizedLabels[76], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Pyramid_Click(sender, e);
                }
                else if (n > maxVertices)
                {
                    MaxVerticesMessage(maxVertices);
                    Pyramid_Click(sender, e);
                }
                else addFigure(new Pyramid(screen3D.OpenGL, n, objectColor, localizedLabels[68], localizedLabels[66], localizedLabels[32]), cam.LookAt, true);
            }
            catch (Exception) { }
        }
        private void Prism_Click(object sender, EventArgs e)
        {
            try
            {
                int n = int.Parse(Interaction.InputBox(localizedLabels[75] + localizedLabels[117], "Penteract", "8"));
                if (n < 3)
                {
                    MessageBox.Show(localizedLabels[78], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Prism_Click(sender, e);
                }
                else if (n > maxVertices)
                {
                    MaxVerticesMessage(maxVertices);
                    Prism_Click(sender, e);
                }
                else addFigure(new Prism(screen3D.OpenGL, n, objectColor, localizedLabels[66], localizedLabels[33]), cam.LookAt, true);
            }
            catch (Exception) { }
        }
        private void Circle_Click(object sender, EventArgs e)
        {
            try {
                int n = int.Parse(Interaction.InputBox(localizedLabels[81], "Penteract", "100"));
                if (n < 3) {
                    MessageBox.Show(localizedLabels[82], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Circle_Click(sender, e);
                }
                else if (n > maxVertices)
                {
                    MaxVerticesMessage(maxVertices);
                    Circle_Click(sender, e);
                }
                else addFigure(new Circle(screen3D.OpenGL, n, objectColor, localizedLabels[67]), cam.LookAt, true);
            }
            catch (Exception) { }
        }
        private void Sphere_Click(object sender, EventArgs e)
        {
            try
            {
                int n = int.Parse(Interaction.InputBox(localizedLabels[79] + localizedLabels[118], "Penteract", "1000"));
                if (n < 9)
                {
                    MessageBox.Show(localizedLabels[80], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Sphere_Click(sender, e);
                }
                else if (n > maxVertices * maxVertices)
                {
                    MaxVerticesMessage(maxVertices * maxVertices);
                    Sphere_Click(sender, e);
                }
                else addFigure(new Sphere(screen3D.OpenGL, n, objectColor, localizedLabels[65]), cam.LookAt, true);
            }
            catch (Exception) { }
        }
        private void Torus_Click(object sender, EventArgs e)
        {
            try
            {
                int n = int.Parse(Interaction.InputBox(localizedLabels[79] + localizedLabels[119], "Penteract", "1000"));
                float inner = float.Parse(Interaction.InputBox(localizedLabels[115], "Penteract", "1"), CultureInfo.InvariantCulture);
                if (n < 9)
                {
                    MessageBox.Show(localizedLabels[80], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Torus_Click(sender, e);
                }
                else if (n > maxVertices * maxVertices)
                {
                    MaxVerticesMessage(maxVertices * maxVertices);
                    Torus_Click(sender, e);
                }
                else if (inner >= 0 && inner <= 2) addFigure(new Torus(screen3D.OpenGL, n, inner + 1, objectColor, localizedLabels[120]), cam.LookAt, true);
            }
            catch (Exception) { }
        }
        private void MaxVerticesMessage(int n)
        {
            var message = localizedLabels[102].Split().ToList();
            message.Insert(message.Count - 1, $"{n}");
            MessageBox.Show(string.Join(" ", message), "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void addFigure(Object3D figure, Vertex position, bool backup)
        {//Add new object to a scene
            try {
                var t = undoRedo;
                figure.Move(position);
                allObjects.Add(figure);
                comboShapes.Items.Add(allObjects.Count + ". " + figure.Name);

                comboShapes.SelectedIndex = allObjects.Count - 1;
                if (backup) {
                    undoRedo = t;
                    Backup();
                }
            }
            catch (Exception) { }
        }
        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs e)
        {//Update a 3D-scene every frame
            
            if (!updated) return;
            else updated = false;
            var gl = screen3D.OpenGL;
            var backColor = new Vertex(backroundColor.R / 255f, backroundColor.G / 255f, backroundColor.B / 255f);

            gl.ClearColor(backColor.X, backColor.Y, backColor.Z, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            cam.AspectRatio = screen3D.Width / (float)screen3D.Height;
            cam.Look();
            //Render coordinate axes
            if (showAxes.Checked) new Axes(cam.far * 4).Render(gl);
            //Render a grid surface
            if (showGrid.Checked) new PlaneGridRenderer(cam.far * 4).Render(gl, cam);
            if (allObjects.Count == 0) return;
            var width = 10 / cam.LookDistance();
            if (width > 2) width = 2;
            gl.LineWidth(width);//Width of objects lines

            var light = lightSource;
            if (!lightSwitch.Checked) light = null;
            var size = 25 / cam.LookDistance();
            if (size > 5) size = 5;
            gl.PointSize(size);//Size of the selection points
                               //Rendering objects
            for (int j = 0; j < allObjects.Count; j++) allObjects[j].Render(light);
            int i = comboShapes.SelectedIndex;
            if (showSelection.Checked)
            {
                var selection = new Object3D(OpenGL.GL_POINTS, gl, Color.White);
                foreach (int j in listShape.SelectedIndices)
                    if (j != i)
                    {//Selection for objects
                        selection.Polygons = allObjects[j].Polygons;
                        selection.Vertices = allObjects[j].Vertices;
                        selection.Render(null);
                    }
                if(i >= 0)
                {
                    selection.Polygons = allObjects[i].Polygons;
                    selection.Vertices = allObjects[i].Vertices;
                    selection.Render(null);
                }
            }
            gl.PointSize(size * 1.5f);//For selected vertices
            if (editor.Visible) editor.RenderSelected(gl);
            else if (vertexPanel.ContainsFocus)
            {
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Color(0.9f, 0, 0);
                var v = allObjects[i].Vertices[comboVertices.SelectedIndex];
                gl.Vertex(v);
                gl.Enable(OpenGL.GL_LIGHTING);
            }
            else if (polygonPanel.ContainsFocus)
            {
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.LineWidth(width * 2);
                gl.Begin(OpenGL.GL_TRIANGLES);//For selected polygons
                gl.Color(LightColor(allObjects[i].color));
                allObjects[i].Polygons[comboPolygons.SelectedIndex].Draw(OpenGL.GL_TRIANGLES, null);
                gl.Enable(OpenGL.GL_LIGHTING);
            }
            gl.End();
            gl.Flush();  
        }
        public Vertex LightColor(Vertex color) {
            var v = (new Vertex(1, 1, 1) + color) / 2;
            if (color.X + color.Y + color.Z > 2.9f) v /= 2;
            return v;
        }
        private void openGLControl1_KeyUp(object sender, KeyEventArgs e) {
            try
            {
                int i = comboShapes.SelectedIndex;
                camMoveCount = 10f / screen3D.FrameRate;
                ctrlPressed = false;
                bool moved = oldPosition.X != allObjects[i].Position.X || oldPosition.Y != allObjects[i].Position.Y || oldPosition.Z != allObjects[i].Position.Z;
                bool rotated = oldRotation.X != allObjects[i].Rotation.X || oldRotation.Y != allObjects[i].Rotation.Y || oldRotation.Z != allObjects[i].Rotation.Z;
                if (i >= 0 && (moved || rotated))
                {//If an Object was shifted or rotated, backup it
                    oldPosition = allObjects[i].Position;
                    Backup();
                }
            }
            catch (Exception) { }
        }
        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        { //Rotation the camera with the mouse around the selected object
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    float dx = Cursor.Position.X - previousCursorPosition.X;
                    float dy = Cursor.Position.Y - previousCursorPosition.Y;
                    previousCursorPosition = Cursor.Position;
                    cam.CenterRotateX(dx);
                    cam.CenterRotateY(dy);
                    camViews.SelectedIndex = -1;
                    int i = comboShapes.SelectedIndex;
                    if (i >= 0 && allObjects[i].drawMode && ctrlPressed) Draw();
                    updated = true;
                }
            }
            catch (Exception) { }
        }
        private void Draw()
        {//Drawing an arbitrary object mode 
            try {
                int i = comboShapes.SelectedIndex;
                
                if (drawLatency == 30 / allObjects[i].drawFrequency && i >= 0)
                {
                    drawLatency = 1;
                    if (allObjects[i].Polygons.Count < 65536)
                    {
                        var vertex = new Vertex(cam.Position - cam.LookAt) / 3;
                        if (moveRadio.Checked) allObjects[i].FindPosition();
                        else if (scaleRadio.Checked) allObjects[i].FindSize();
                        var vertices = allObjects[i].Polygons[allObjects[i].Polygons.Count - 1].VerticesID;
                        allObjects[i].Vertices.Add(vertex);
                        vertices.Add(allObjects[i].Vertices.Count - 1);
                        if (vertices.Count > 2) addPolygon();
                        comboVertices.Items.Add(localizedLabels[69] + ' ' + allObjects[i].Vertices.Count);
                        comboVertices.SelectedIndex = allObjects[i].Vertices.Count - 1;
                        verticesCount.Text = localizedLabels[124] + "\n" + totalVertices();      
                    }
                    else
                    {
                        MessageBox.Show(localizedLabels[114], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else drawLatency++;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void addPolygon()
        {//Adding a new polygon to an object
            try
            {
                int i = comboShapes.SelectedIndex;
                var poly = new Polygon(allObjects[i]);
                poly.VerticesID.Add(allObjects[i].Vertices.Count - 1);
                var polygons = allObjects[i].Polygons;
                polygons.Add(poly);
                polygonsCount.Text = localizedLabels[125] + "\n" + totalPolygons();
                comboPolygons.Items.Add(localizedLabels[70] + ' ' + polygons.Count);
                comboPolygons.SelectedIndex = polygons.Count - 1;  
            }
            catch (Exception) { }
        }
        int totalVertices(int i = 0)
        {
            foreach (var obj in allObjects) i += obj.Vertices.Count;
            return i;
        }
        int totalPolygons(int i = 0)
        {
            foreach (var obj in allObjects) i += obj.Polygons.Count;
            return i;
        }
        private void openGLControl1_KeyDown(object sender, KeyEventArgs e)
        {// Control by pressing or by a combination of keys
            try
            {
                int i = comboShapes.SelectedIndex, moved = 0, selectedCount = 0;
                Vertex direction = cam.Position - cam.LookAt, commonCenter = new Vertex(0, 0, 0);
                if (i >= 0) {
                    oldPosition = allObjects[i].Position;
                    oldRotation = allObjects[i].Rotation;
                    commonCenter += allObjects[i].Position;
                    selectedCount++;
                }
                foreach (int j in listShape.SelectedIndices)
                    if (j != i) {
                        commonCenter += allObjects[j].Position;
                        selectedCount++;
                    }
                commonCenter /= selectedCount;
                direction.Y = 0;
                direction.Normalize();
                direction.X *= cam.MoveSpeed;
                direction.Z *= cam.MoveSpeed;
                if (e.KeyCode != previousKey) camMoveCount = 10f / screen3D.FrameRate;
                camMoveCount += 1f / screen3D.FrameRate;
                previousKey = e.KeyCode;
                ctrlPressed = e.Control;
                Vertex delta = new Vertex(0, 0, 0);
                switch (e.KeyCode)
                {
                    case Keys.W:
                        delta = direction * camMoveCount * -1f;
                        moved = 1; break;
                    case Keys.S:
                        if (e.Control) QuickSave(sender, null);
                        else if (e.Alt) saveScreenShotDialog(sender, null);
                        else
                        {
                            delta += direction * camMoveCount;
                            moved = 1;
                        }
                        break;
                    case Keys.A:
                        //Move Left
                        if(e.Control) for (int j = 0; j < listShape.Items.Count; j++) listShape.SetSelected(j, true);
                        else {
                            delta -= cam.Rotate(direction, Math.PI / 2) * camMoveCount;
                            moved = 1; 
                        }
                        break;
                    case Keys.D:
                        //Move Right
                        if (e.Alt) DeleteTexture(sender, null);
                        else {
                            delta += cam.Rotate(direction, Math.PI / 2) * camMoveCount;
                            moved = 1;
                        }
                        break;
                    case Keys.Space:
                        //Move Up
                        delta.Y += cam.MoveSpeed * camMoveCount * 2;
                        moved = 1; break;
                    case Keys.C:
                        if (e.Control)
                        {
                            clipBoard.Clear();// Ctrl + C
                            foreach (int j in listShape.SelectedIndices) if (j != i) Copy(j);
                            Copy(i);
                        }
                        else
                        {//Move Down
                            delta.Y -= cam.MoveSpeed * camMoveCount * 2;
                        }
                        moved = 1; break;
                    case Keys.R://Change a render mode
                        if (i >= 0 && e.Control) {
                            int next = renderMode.SelectedIndex + 1, n = renderMode.Items.Count;
                            renderMode.SelectedIndex = next % n;
                        }
                        break;
                    case Keys.V:
                        if (e.Control) Paste(); // Ctrl + V
                        if (e.Alt && i >= 0) modify_Click(sender, null);
                        break;
                    case Keys.Q:
                        cam.CenterRotateX(-camMoveCount * 4.5f); // Clockwise rotation around an object
                        if (e.Shift && i >= 0)
                        {
                            Vertex d = new Vertex(0, camMoveCount * 1.5f, 0);
                            foreach (int j in listShape.SelectedIndices)
                            {
                                if (j != i) allObjects[j].Rotate(d, commonCenter);
                            }
                            allObjects[i].Rotate(d, commonCenter);
                        }
                        break;
                    case Keys.E:
                        cam.CenterRotateX(camMoveCount * 4.5f); // Counter-clockwise rotation around an object
                        if (e.Shift && i >= 0)
                        {
                            Vertex d = new Vertex(0, -camMoveCount * 1.5f, 0);
                            foreach (int j in listShape.SelectedIndices)
                            {
                                if (j != i) allObjects[j].Rotate(d, commonCenter);
                            }
                            allObjects[i].Rotate(d, commonCenter);
                        }
                        break;
                    case Keys.Delete:
                        if (i >= 0) DeleteSelected();
                        break;
                    case Keys.F2:
                        if (i >= 0) Rename(i);
                        break;
                    case Keys.Z:
                        if (e.Control)
                        {
                            if (e.Shift) Discard(redo);
                            else Discard(undo);
                        }
                        break;
                }
                if(e.Alt)
                {
                    if (e.KeyCode == Keys.N) createNew(sender, null);
                    else if (e.KeyCode == Keys.L) lightSwitch.Checked = !lightSwitch.Checked;
                    else if (e.KeyCode == Keys.X) completeEdit();
                    else if (e.KeyCode == Keys.M && listShape.SelectedItems.Count > 1) Merge(sender, null);
                    else if (e.KeyCode == Keys.T && i >= 0) SelectTexture(sender, null);
                }
                if (e.KeyCode == Keys.Escape) listShape.ClearSelected();
                if (e.KeyCode == Keys.F && e.Control && allObjects.Count > 0) showSelection.Checked = !showSelection.Checked;
                cam.Position += delta; cam.LookAt += delta;
                if (cam.LookAt.X > 1000) cam.LookAt.X = -1000;
                if (cam.LookAt.X < -1000) cam.LookAt.X = 1000;
                if (cam.LookAt.Y > 1000) cam.LookAt.Y = -1000;
                if (cam.LookAt.Y < -1000) cam.LookAt.Y = 1000;
                if (cam.LookAt.Z > 1000) cam.LookAt.Z = 1000;
                if (cam.LookAt.Z < -1000) cam.LookAt.Z = -1000;
                //Move the current object to the camera while holding Shift
                if (e.Shift && moved > 0 && i >= 0)
                {
                    foreach (int j in listShape.SelectedIndices)
                    {
                        var distance = allObjects[j].Position - commonCenter;
                        if (j != i) allObjects[j].Move(cam.LookAt + distance);
                    }
                    var distance2 = allObjects[i].Position - commonCenter;
                    allObjects[i].Move(cam.LookAt + distance2);
                }
                updated = true;
            }
            catch(Exception) { }
        }
        private void Transform()
        {//Changing the parameters of the selected object via the right menu
            try {
                int i = comboShapes.SelectedIndex;
                Vertex v = new Vertex((float)xNumeric.Value, (float)yNumeric.Value, (float)zNumeric.Value);
                if (moveRadio.Checked) {
                    allObjects[i].Move(v);
                } 
                else if (rotateRadio.Checked) allObjects[i].Rotate(v - allObjects[i].Rotation, allObjects[i].Position);
                else if (scaleRadio.Checked) allObjects[i].Scale(v, allObjects[i].Position);
                Backup();
            }
            catch(Exception) { }  
        }
        float zoomCount = 0, previous_delta;
        private void openGLControl1_MouseWheel(object sender, MouseEventArgs e)
        {//Zoom via mouse wheel
            if (!screen3D.DrawFPS) {
                if (e.Delta == -previous_delta) zoomCount = 0;
                previous_delta = e.Delta;
                zoomCount += cam.MoveSpeed;
                cam.Zoom(-zoomCount * e.Delta / 1000);
            }
            updated = true;
        }
        private void updateNumericFields(int i)
        {//Updating a numeric fields
            try {
                if (i < 0)
                {
                    xNumeric.Value = 0; yNumeric.Value = 0; zNumeric.Value = 0;
                }
                else  {
                    if (moveRadio.Checked)
                    {
                        xNumeric.Value = (decimal)allObjects[i].Position.X;
                        yNumeric.Value = (decimal)allObjects[i].Position.Y;
                        zNumeric.Value = (decimal)allObjects[i].Position.Z;
                    }
                    else if (rotateRadio.Checked)
                    {
                        xNumeric.Value = (decimal)allObjects[i].Rotation.X;
                        yNumeric.Value = (decimal)allObjects[i].Rotation.Y;
                        zNumeric.Value = (decimal)allObjects[i].Rotation.Z;
                    }
                    else if (scaleRadio.Checked)
                    {
                        xNumeric.Value = (decimal)allObjects[i].Size.X;
                        yNumeric.Value = (decimal)allObjects[i].Size.Y;
                        zNumeric.Value = (decimal)allObjects[i].Size.Z;
                    }
                }
            }
            catch { }  
        }
        private void comboShapes_SelectedIndexChanged(object sender, EventArgs e) => selectObject();
        public async void selectObject()
        {//Selection an object with updating numeric fields in the right menu
            try
            {   
                int i = comboShapes.SelectedIndex;
                if (i == -1) return;
                if(allObjects[i].drawMode) toolTip3.SetToolTip(screen3D, localizedLabels[53]);
                else toolTip3.SetToolTip(screen3D, localizedLabels[101]);

                var verticesList = new string[allObjects[i].Vertices.Count];
                var polygonsList = new string[allObjects[i].Polygons.Count];
                //Updating lists of vertices and polygons
                var tasks = new Task[2];
                tasks[0] = Task.Run(() =>  {
                    for (int j = 0; j < allObjects[i].Vertices.Count; j++) verticesList[j] = localizedLabels[69] + ' ' + (j + 1);
                });
                tasks[1] = Task.Run(() => {
                    for (int j = 0; j < allObjects[i].Polygons.Count; j++) polygonsList[j] = localizedLabels[70] + ' ' + (j + 1);
                });
                await Task.WhenAll(tasks);
                comboVertices.Items.Clear();
                comboPolygons.Items.Clear();
                comboVertices.Items.AddRange(verticesList);
                comboPolygons.Items.AddRange(polygonsList);
                comboVertices.SelectedIndex = 0;
                comboPolygons.SelectedIndex = 0;
                switch (allObjects[i].RenderMode)
                {
                    case OpenGL.GL_LINES:
                        renderMode.SelectedIndex = 0;
                        break;
                    case OpenGL.GL_TRIANGLES:
                        renderMode.SelectedIndex = 1;
                        break;
                    case OpenGL.GLU_NONE:
                        renderMode.SelectedIndex = 2;
                        break;
                }
                currentObject.Text = comboShapes.Text;
                setObjectColor.BackColor = allObjects[i].GetColor();
                allObjects[i].FindPosition();
                drawSpeedField.Value = (decimal)(allObjects[i].drawFrequency);
                if (scaleRadio.Checked) allObjects[i].FindSize();

                updated = true;
            }
            catch (Exception) { }
        }
        private void moveRadio_CheckedChanged(object sender, EventArgs e) => allObjects[comboShapes.SelectedIndex].FindPosition();

        private void scaleRadio_CheckedChanged(object sender, EventArgs e) => allObjects[comboShapes.SelectedIndex].FindSize();

        private void panel2_Enter(object sender, EventArgs e) => showSelection.Checked = true;

        private void panel3_Enter(object sender, EventArgs e) => showSelection.Checked = true;
        private void changeVertex()
        {//Changing a coordinates of selected vertex
            int i = comboShapes.SelectedIndex, j = comboVertices.SelectedIndex;
            allObjects[i].Vertices[j] = new Vertex((float)X1.Value, (float)Y1.Value, (float)Z1.Value);
            if (moveRadio.Checked) allObjects[i].FindPosition();
            else if (scaleRadio.Checked) allObjects[i].FindSize();
            Backup();
        }
        private void changePolygon()
        {//Changing a coordinates of selected polygon      
            int i = comboShapes.SelectedIndex, j = comboPolygons.SelectedIndex;
            Vertex d = new Vertex((float)X2.Value, (float)Y2.Value, (float)Z2.Value) - allObjects[i].Polygons[j].Center();
            allObjects[i].Polygons[j].Move(d);
            if (moveRadio.Checked) allObjects[i].FindPosition();
            if (scaleRadio.Checked) allObjects[i].FindSize();
            Backup(); 
        }
        private void ClearAll()
        {//Deleting the all objects
            try {
                allObjects.Clear();
                listShape.Items.Clear();
                comboShapes.Items.Clear();
                comboVertices.Items.Clear();
                comboPolygons.Items.Clear();
                comboShapes.Text = localizedLabels[21];
                currentObject.Text = localizedLabels[21];
                if (editor.Visible) editor.UpdateLists();
                updated = true;
            }
            catch(Exception) { }   
        }
        private void QuickSave(object sender, EventArgs e) => Save3dFile(file_Path);
        private void Save3dFile(string filePath)
        {//Call the window for saving a 3D scene to a file
            if (filePath.Length < 1) {
                editor.Close();
                save3dFile.FileName = Path.GetFileName(file_Path);
                if (save3dFile.ShowDialog() == DialogResult.OK)
                {
                    if (save3dFile.FileName.ToLower().EndsWith(".obj"))
                        to_OBJ_format(save3dFile.FileName);
                    else if (save3dFile.FileName.ToLower().EndsWith(".prct"))
                        to_PRCT_format(save3dFile.FileName);
                    else if (save3dFile.FileName.ToLower().EndsWith(".stl"))
                        to_STL_format(save3dFile.FileName);
                    else MessageBox.Show(localizedLabels[73] + localizedLabels[96], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    file_Path = save3dFile.FileName;
                }
            }
            else  {
                if (filePath.EndsWith(".obj")) to_OBJ_format(filePath);
                else if (filePath.EndsWith(".prct")) to_PRCT_format(filePath);
                else if (filePath.EndsWith(".stl")) to_STL_format(filePath);
                else MessageBox.Show(localizedLabels[73] + localizedLabels[96], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Load3dFileDialog(object sender, EventArgs e)
        {//Call the window for loading 3D-objects from files
            editor.Close();
            open3dFile.FileName = Path.GetFileName(file_Path);
            if (open3dFile.ShowDialog() == DialogResult.OK) Load3dFile(open3dFile.FileName);
        }
        public void Load3dFile(string filePath)
        {
            try {
                listShape.Items.Clear();
                if (filePath.ToLower().EndsWith(".obj")) from_OBJ_format(filePath);
                else if (filePath.ToLower().EndsWith(".prct")) from_PRCT_format(filePath);
                else if (filePath.ToLower().EndsWith(".stl")) from_STL_format(filePath);
            }
            catch(Exception ) {  }
        }
        private void createNew(object sender, EventArgs e)
        {//Mode of creation and drawing a new object 
            try
            {
                drawLabel.Text = localizedLabels[133];
                toolTip3.SetToolTip(screen3D, localizedLabels[53]);
                var label = "Penteract - " + localizedLabels[13].Split('(')[0].ToLower();
                string input = Interaction.InputBox(localizedLabels[97], label, localizedLabels[64]);
                var obj = new Object3D(screen3D.OpenGL, objectColor);
                if (input != null && input.Length > 0) obj.Name = input;
                else return;
                m:
                input = Interaction.InputBox(localizedLabels[131], label, "1");
                //Entering a frequency of spawn new polygons
                obj.drawFrequency = float.Parse(input, CultureInfo.InvariantCulture);
                if (obj.drawFrequency >= 0.5 && obj.drawFrequency <= 5.0)
                {
                    toolTip3.Show(localizedLabels[53], screen3D, 100, 100, 2000);
                    obj.Polygons.Add(new Polygon(obj));
                    obj.Vertices.Add(cam.LookAt);
                    obj.Polygons[0].VerticesID.Add(0);
                    obj.drawMode = true;
                    allObjects.Add(obj);
                    comboShapes.Items.Add(allObjects.Count + ". " + obj.Name);
                    comboShapes.SelectedIndex = allObjects.Count - 1;
                    drawSpeedField.Value = (decimal)obj.drawFrequency;
                    Backup();
                }
                else
                {
                    MessageBox.Show(localizedLabels[132], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    goto m;   
                }
            }
            catch (Exception ex) {
            }
        }
        private void exitCreate_Click(object sender, EventArgs e) => completeEdit();
        private void completeEdit()
        {//Turning off a draw mode
            toolTip3.SetToolTip(screen3D, localizedLabels[101]);
            try
            {
                allObjects[comboShapes.SelectedIndex].drawMode = false;
                setObjectColor.Visible = true;
                selectTextureItem.Visible = true;
            }
            catch(Exception) { }
        }
        private void SelectTexture(object sender, EventArgs e)
        {//Call a texture selection window for the selected object
            try
            {
                editor.Close();
                if (openTexture.ShowDialog() == DialogResult.OK) {
                    openTexture.OpenFile();
                    foreach (int i in listShape.SelectedIndices) allObjects[i].setTexture(openTexture.FileName);
                    var texturePath = Path.GetTempPath() + Guid.NewGuid() + ".prct";
                    to_PRCT_format(texturePath, false, false);//Updating 3D scene via temp file
                    ClearAll();
                    from_PRCT_format(texturePath, false);
                };     
            }
            catch (Exception) { }    
        }
        private void DeleteTexture(object sender, EventArgs e)
        {//Delete a texture from selected object 
            if(listShape.SelectedItems.Count > 0) {
                foreach (int i in listShape.SelectedIndices) allObjects[i].setTexture("");
                Backup();
            }
        }
        private void selectColor_Click(object sender, EventArgs e)
        {//Call a color selection window for the selected object
            var shape = allObjects[comboShapes.SelectedIndex];
            colorDialog1.Color = shape.GetColor();
            colorDialog1.ShowDialog();
            setObjectColor.BackColor = colorDialog1.Color;
            shape.setColor(colorDialog1.Color);
            Backup();
        }
        private void fillObjects_Click(object sender, EventArgs e)
        {//Set new color to selected objects;
            try  {
                //Apply color of the first selected object as default
                colorDialog1.Color = allObjects[listShape.SelectedIndices[0]].GetColor();

                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (int i in listShape.SelectedIndices) allObjects[i].SetColor(colorDialog1.Color);
                    Backup();
                }
            }
            catch(Exception) { }    
        }
        //Applying entered values after pressing the Enter key:
        private void xNumeric_KeyDown(object sender, KeyEventArgs e) 
        {
            if(e.KeyCode == Keys.Enter) Transform();
        }   
        private void xNumeric_Leave(object sender, EventArgs e) => Transform();
        private void yNumeric_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) Transform();
        }
        private void yNumeric_Leave(object sender, EventArgs e) => Transform();
        private void zNumeric_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) Transform();
        }
        private void zNumeric_Leave(object sender, EventArgs e) => Transform();
        private void X1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) changeVertex();
        }
        private void X1_Leave(object sender, EventArgs e) => changeVertex();
        private void Y1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) changeVertex();
        }
        private void Y1_Leave(object sender, EventArgs e) => changeVertex(); 
        private void Z1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) changeVertex();
        }
        private void Z1_Leave(object sender, EventArgs e) => changeVertex();
        private void X2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) changePolygon();
        }
        private void X2_Leave(object sender, EventArgs e) => changePolygon();
        private void Y2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) changePolygon();
        }
        private void Y2_Leave(object sender, EventArgs e) => changePolygon();
        private void Z2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) changePolygon();
        }
        private void Z2_Leave(object sender, EventArgs e) => changePolygon();
        private void rename3_Click(object sender, EventArgs e) => Rename(comboShapes.SelectedIndex);
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
        private void Rename(int i)
        {//Rename and object by its index
            if (i >= 0) {
                string input = Interaction.InputBox($"{localizedLabels[100]}{i + 1}:", "Penteract", allObjects[i].Name);
                if (input.Length > 0)
                {
                    if (input.Length > 22) input = input.Substring(0, 22);
                    allObjects[i].Name = input;
                    comboShapes.Items[i] = i + 1 + ". " + input;
                    listShape.Items[i] = i + 1 + ". " + input;
                    currentObject.Text = i + 1 + ". " + input; 
                }
                Backup();
            }
        }
        private void listShape_KeyDown(object sender, KeyEventArgs e)
        {//Control of the left list of objects, through which you can modify many objects at once
            if (e.Control)
                switch (e.KeyCode)
                {
                    case Keys.A:
                        for (int i = 0; i < listShape.Items.Count; i++) listShape.SetSelected(i, true);
                        break;
                    case Keys.C:
                        clipBoard.Clear();
                        foreach (int i in listShape.SelectedIndices) Copy(i);
                        break;
                    case Keys.V:
                        Paste();
                        break;
                    case Keys.Z:
                        if (e.Shift) Discard(redo);
                        else Discard(undo);
                        break;
                }
            else if (e.KeyCode == Keys.Delete) DeleteSelected();
            else if (e.KeyCode == Keys.F2 && listShape.SelectedIndices.Count == 1) Rename(listShape.SelectedIndices[0]);
            else if (e.KeyCode == Keys.Escape) listShape.ClearSelected();
            else if (e.Alt)
            {
                if(e.KeyCode == Keys.M && listShape.SelectedItems.Count > 1) Merge(sender, e);
                else if(e.KeyCode == Keys.C) fillObjects_Click(sender, e);
                else if (e.KeyCode == Keys.T) SelectTexture(sender, e);
                else if (e.KeyCode == Keys.D) DeleteTexture(sender, e);
            }
        }
        private void listShape_DoubleClick(object sender, EventArgs e)
        {
            if(listShape.SelectedIndices.Count == 1) cam.MoveTo(allObjects[listShape.SelectedIndex].Position);
        }
        private void îòìåíèòüCtrlZToolStripMenuItem_Click(object sender, EventArgs e) => Discard(undo);
        private void ïîâòîðèòüCtrlZToolStripMenuItem_Click(object sender, EventArgs e) => Discard(redo);
        private void ñîõðàíèòüÊàêToolStripMenuItem_Click(object sender, EventArgs e) => Save3dFile("");
        private void deleteObjects_Click(object sender, EventArgs e) => DeleteSelected();
        private void DeleteSelected(bool backup = true)
        {//Deleting objects selected via the left list
            try
            {
                var newList = new List<Object3D>();
                if(listShape.SelectedIndices.Count == 0)
                {
                    for (int i = 0; i < allObjects.Count; i++)
                        if (i != comboShapes.SelectedIndex) newList.Add(allObjects[i]);
                }
                else {
                    for (int i = 0; i < allObjects.Count; i++)
                        if (!listShape.SelectedIndices.Contains(i)) newList.Add(allObjects[i]);
                }   
                ClearAll();
                allObjects = newList;
                UpdateInterface(null, null);
                comboShapes.SelectedIndex = comboShapes.Items.Count - 1;
                if(backup) Backup();
            }
            catch (Exception) { }
        }
        private void Transform2()
        {//Transforming the objects selected via left list
            try
            {//Âåêòîð èçìåíåíèÿ âûáðàííûõ îáúåêòîâ
                int n = listShape.SelectedIndices.Count;
                if (n == 0) return;//If nothing is selected
                Vertex v = new Vertex((float)xNumeric2.Value, (float)yNumeric2.Value, (float)zNumeric2.Value);
                xNumeric2.Value = 0;//Reset field values
                yNumeric2.Value = 0;
                zNumeric2.Value = 0;
                Vertex commonCenter = new Vertex(0, 0, 0);
                if(!moveRadio2.Checked)
                    foreach (int i in listShape.SelectedIndices) commonCenter += allObjects[i].FindPosition();
                commonCenter /= n;
                foreach (int i in listShape.SelectedIndices)
                {
                    if (moveRadio2.Checked && v.Magnitude() != 0)
                    {                       
                        allObjects[i].Move(allObjects[i].Position + v);
                    }
                    else if (rotateRadio2.Checked && v.Magnitude() != 0)
                    {
                        allObjects[i].Rotate(v, commonCenter);
                    }
                    else if (scaleRadio2.Checked && (v.X != 1 || v.Y != 1 || v.Z != 1))  
                    {
                        if (v.X == 0) v.X = 1;
                        if (v.Y == 0) v.Y = 1;
                        if (v.Z == 0) v.Z = 1;
                        allObjects[i].Scale(allObjects[i].Size * v, commonCenter);
                        xNumeric2.Value = 1;
                        yNumeric2.Value = 1;
                        zNumeric2.Value = 1;   
                    }
                }
                Backup();
            }
            catch(Exception) { }
        }
        private void óäàëèòüToolStripMenuItem_Click(object sender, EventArgs e) => DeleteSelected();
        private void êîïèðîâàòüToolStripMenuItem_Click(object sender, EventArgs e)
        {//Copy the selected objects into clipboard
            clipBoard.Clear();
            foreach (int i in listShape.SelectedIndices) Copy(i);
        }
        private void âñòàâèòüToolStripMenuItem_Click(object sender, EventArgs e) => Paste();
        private void rename2_Click(object sender, EventArgs e) => Rename(listShape.SelectedIndices[0]);
        private void îToolStripMenuItem_Click(object sender, EventArgs e)
        {//About box
            MessageBox.Show($"Penteract\n\n© {localizedLabels[71]}", localizedLabels[19]);
        }
        private void Merge(object sender, EventArgs e)
        {//Merge the selected objects into new object
            try {       
                var obj = new Object3D(screen3D.OpenGL, objectColor);
                string input = Interaction.InputBox(localizedLabels[97], "Penteract - " + localizedLabels[114], localizedLabels[64]);
                if (input != null && input.Length > 0) obj.Name = input;
                else return;
                var totalPolygons = new HashSet<Polygon>();
                int totalVertices = 0;
                foreach (int i in listShape.SelectedIndices)
                {
                    obj.Vertices.AddRange(allObjects[i].Vertices);
                    foreach (var polygon in allObjects[i].Polygons)
                    {
                        var newPoly = new Polygon(polygon, obj);
                        for (int j = 0; j < newPoly.VerticesID.Count; j++)
                        {
                            newPoly.VerticesID[j] += totalVertices;
                            totalPolygons.Add(newPoly);
                        }
                    }
                    totalVertices += allObjects[i].Vertices.Count;
                }
                obj.Polygons = totalPolygons.ToList();
                DeleteSelected(false);
                allObjects.Add(obj);
                comboShapes.Items.Add(allObjects.Count + ". " + obj.Name);
                comboShapes.SelectedIndex = comboShapes.Items.Count - 1;
                Backup();
            }
            catch (Exception) { }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            editor.Close();
            if (!changesSaved) 
            {
                var saveRequest = MessageBox.Show(localizedLabels[72], "Penteract", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (saveRequest == DialogResult.Yes) Save3dFile("");
                else if (saveRequest == DialogResult.Cancel) e.Cancel = true;
            }          
        } 
        private void modify_Click(object sender, EventArgs e)
        {//Open a window for modification of vertices and polygons
            if (!editor.Visible)
            {
                editor = new Form3(this);
                editor.Show();
            }
            else editor.Focus();
        }
        private void listShape_SelectedIndexChanged(object sender, EventArgs e) 
        {
            if (listShape.SelectedIndices.Count == 1) {
                comboShapes.SelectedIndex = listShape.SelectedIndex;
            }          
            rename2.Visible = listShape.SelectedIndices.Count == 1;
            mergeItem.Visible = listShape.SelectedIndices.Count > 1;
            showSelection.Checked = true;
            updated = true;
        }
        //Changing light source coordinates
        private void lightX_ValueChanged(object sender, EventArgs e) {
            lightSource.Position.X = (float)lightX.Value;
            updated = true;
        }
        private void lightY_ValueChanged(object sender, EventArgs e) {
            lightSource.Position.Y = (float)lightY.Value;
            updated = true;
        }
        private void lightZ_ValueChanged(object sender, EventArgs e) {
            lightSource.Position.Z = (float)lightZ.Value;
            updated = true;
        } 
        private void setLightColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = lightSource.GetColor();
            colorDialog1.ShowDialog();
            lightSource.SetColor(colorDialog1.Color);
            LightButtonColor(colorDialog1.Color);
            Backup();
        }
        private void LightButtonColor(Color color)
        {
            double R = color.R, G = color.G, B = color.B;
            if (R < 228) R *= 1.13 ; if (G < 228) G *= 1.13; if (B < 228) B *= 1.13;
            setLightColor.BackColor = Color.FromArgb((int)R, (int)G, (int)B);
        }
        private void setBackColor(object sender, EventArgs e)
        {
            colorDialog1.Color = backroundColor;
            colorDialog1.ShowDialog();
            backroundColor = colorDialog1.Color;
            setBackground.BackColor = backroundColor;
            Backup();
        }
        private void currentObject_DoubleClick(object sender, EventArgs e)
        {
            cam.MoveTo(allObjects[comboShapes.SelectedIndex].Position);
        }   
        private void saveScreenShotDialog(object sender, EventArgs e)
        {//Save a screenshot of the 3D scene to the selected graphic file format
            editor.Close();
            saveScreen.FileName = Path.GetFileNameWithoutExtension(file_Path);//File name without extension
            if (saveScreen.ShowDialog() == DialogResult.OK)
            {
                if (saveScreen.FileName.ToLower().EndsWith(".png"))
                    saveScreenShot(saveScreen.FileName, ImageFormat.Png);
                else if (saveScreen.FileName.ToLower().EndsWith(".jpg"))
                    saveScreenShot(saveScreen.FileName, ImageFormat.Jpeg);
                else if (saveScreen.FileName.ToLower().EndsWith(".bmp"))
                    saveScreenShot(saveScreen.FileName, ImageFormat.Bmp);
                else MessageBox.Show(localizedLabels[73] + localizedLabels[96], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }  
        }
        
        private void saveScreenShot(string filePath, ImageFormat format)
        {
            try
            {//Saving a screenshot of 3D-scene
                var gl = screen3D.OpenGL;
                int W = screen3D.Width, H = screen3D.Height;
                var pixels = new byte[W * H * 4];
                openGLControl1_OpenGLDraw(null, null);
                gl.ReadPixels(0, 0, W, H, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, pixels);
                var image = new Bitmap(W, H);
                for (int i = 0; i < pixels.Length; i += 4)
                {
                    int j = pixels.Length - i - 4, x = (j / 4) % W, y = i / 4 / W;
                    image.SetPixel(x, y, Color.FromArgb(pixels[j], pixels[j + 1], pixels[j + 2]));
                }
                image.Save(filePath, format);
                MessageBox.Show(localizedLabels[74] + saveScreen.FileName + '.', "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start(filePath);//Open image after saving
            }
            catch (Exception e)
            {
                MessageBox.Show(localizedLabels[73] + e.Message.ToLower(), "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void renderMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                changeRenderMode(comboShapes.SelectedIndex);
                foreach (int i in listShape.SelectedIndices) changeRenderMode(i);
                updated = true;
            }
            catch(Exception) { }
        }
        private void changeRenderMode(int i)
        {
            if (i < 0) return;
            switch (renderMode.SelectedIndex)
            {
                case 0:
                    allObjects[i].RenderMode = OpenGL.GL_LINES;
                    break;
                case 1:
                    allObjects[i].RenderMode = OpenGL.GL_TRIANGLES;
                    break;
                case 2:
                    allObjects[i].RenderMode = OpenGL.GLU_NONE;
                    break;
            }
        }
        private void Form1_Resize(object sender, EventArgs e) => updated = true;

        private void screen3D_MouseEnter(object sender, EventArgs e) => updated = true;

        private void showSelection_CheckedChanged(object sender, EventArgs e) => updated = true;

        private void showAxes_CheckedChanged(object sender, EventArgs e) => updated = true;

        private void showGrid_CheckedChanged(object sender, EventArgs e) => updated = true;
        private void drawSpeedField_ValueChanged(object sender, EventArgs e)
        {
            allObjects[comboShapes.SelectedIndex].drawFrequency = (float)drawSpeedField.Value;
        }
        private void screen3D_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawLatency > 0) {
                Backup();
                drawLatency = 0;
            }
        }

        private void FlipX(object sender, EventArgs e)
        {
            foreach (int i in listShape.SelectedIndices) allObjects[i].FlipX();
            Backup();
        }

        private void FlipY(object sender, EventArgs e)
        {
            foreach (int i in listShape.SelectedIndices) allObjects[i].FlipY();
            Backup();
        }

        private void FlipZ(object sender, EventArgs e)
        {
            foreach (int i in listShape.SelectedIndices) allObjects[i].FlipZ();
            Backup();
        }

        private void backColorLabel_Click(object sender, EventArgs e)
        {

        }

        private void switchLight(object sender, EventArgs e)
        {
            lightX.Enabled = lightSwitch.Checked;
            lightY.Enabled = lightSwitch.Checked;
            lightZ.Enabled = lightSwitch.Checked;
            label21.Enabled = lightSwitch.Checked;
            label22.Enabled = lightSwitch.Checked;
            label23.Enabled = lightSwitch.Checked;
            setLightColor.Enabled = lightSwitch.Checked;
            updated = true;
        }
        private void ïàðàìåòðûToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            if (!settings.Visible) {
                settings = new Form2(this);
                settings.Show();
            } 
            else settings.Focus();
        }
        private void Restart_Click(object sender, EventArgs e) {
            Application.Exit();
            Process.Start(Application.ExecutablePath);
        }
        private void setCameraView(object sender, EventArgs e)
        { //Orthogonal camera view float lookDistance = (float)(cam.Position - cam.LookAt).Magnitude();
            float lookDistance = (float)(cam.Position - cam.LookAt).Magnitude();
            switch (camViews.SelectedIndex)
            {
                case 0://Left View 
                    cam.Position = cam.LookAt + new Vertex(lookDistance, 0, 0);
                    break;
                case 1://Right View 
                    cam.Position = cam.LookAt + new Vertex(-lookDistance, 0, 0);
                    break;
                case 2://Top View 
                    cam.Position = cam.LookAt + new Vertex(0.00001f, lookDistance, 0.00001f);
                    break;
                case 3://Bottom view
                    cam.Position = cam.LookAt + new Vertex(-0.00001f, -lookDistance, -0.00001f);
                    break;
                case 4://Front view
                    cam.Position = cam.LookAt + new Vertex(0, 0, lookDistance);
                    break;
                case 5://Rear view
                    cam.Position = cam.LookAt + new Vertex(0, 0, -lookDistance);
                    break;
            }
            screen3D.Select(); updated = true;
        }
        private async void from_OBJ_format(string filePath)
        {//Loading objects from an OBJ file by parsing its text
            try
            {
                string fileName = Path.GetFileName(filePath);
                var objectsInFile = new List<Object3D>();
                Vertex totalSize = new Vertex(0, 0, 0);
                int totalCount = 1;
                var obj = new Object3D(screen3D.OpenGL, objectColor);
                string[] file = File.ReadAllLines(filePath);
                await Task.Run(() => {
                    foreach (var row in file)
                        if (row.StartsWith("o "))
                        {//Reading a set of objects with its names
                            if (objectsInFile.Count > 0) totalCount += obj.Vertices.Count;
                            obj = new Object3D(screen3D.OpenGL, objectColor);
                            obj.Name = row.Split()[1];
                            objectsInFile.Add(obj);
                        }
                        else if (row.StartsWith("v "))
                        {
                            var v = row.Split(' ');
                            float X = float.Parse(v[v.Length - 3], CultureInfo.InvariantCulture),
                                Y = float.Parse(v[v.Length - 2], CultureInfo.InvariantCulture),
                                Z = float.Parse(v[v.Length - 1], CultureInfo.InvariantCulture);
                            if (X > totalSize.X) totalSize.X = X;
                            if (Y > totalSize.Y) totalSize.Y = Y;
                            if (Z > totalSize.Z) totalSize.Z = Z;
                            obj.Vertices.Add(new Vertex(X, Y, Z));
                        }
                        else if (row.StartsWith("f "))
                        {//In the polygon data in the OBJ file - the vertex numbers
                            var polygon = row.Trim().Split();
                            var new_poly = new Polygon(obj);
                            if (polygon.Length > 1)
                                for (int j = 1; j < polygon.Length; j++)
                                {
                                    new_poly.VerticesID.Add(int.Parse(polygon[j]) - totalCount);
                                }
                            obj.Polygons.Add(new_poly);
                        }
                }); 
                if (objectsInFile.Count == 0) objectsInFile.Add(obj);
                totalSize.X = Math.Abs(totalSize.X);
                totalSize.Y = Math.Abs(totalSize.Y);
                totalSize.Z = Math.Abs(totalSize.Z);
                foreach (var obj2 in objectsInFile)
                {
                    allObjects.Add(obj2);
                    comboShapes.Items.Add(allObjects.Count + ". " + obj2.Name);
                }
                cam.LookAt = new Vertex(0, 0, 0);
                cam.Position = totalSize * 2;
                comboShapes.SelectedIndex = comboShapes.Items.Count - 1;
                file_Path = filePath;
                Backup();
            }
            catch (Exception e)
            {
                MessageBox.Show(localizedLabels[73] + e.Message.ToLower(), "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool UpdateProgress(Form4 progress, int i, int n, string fileName)
        {
            try {
                float percent = (int)(i * 1.0f / n * 1000f) / 10f;
                progress.Invoke((MethodInvoker)delegate {
                    progress.Bar.Value = (int)percent;
                    progress.Text = "Penteract - " + localizedLabels[126].ToLower() + fileName + localizedLabels[127] + percent + "%";
                });
                return false;
            }
            catch (Exception) { return true; }
        }
        private async void from_STL_format(string filePath)
        {
            MemoryStream stream = null;
            try  {
                stream = new MemoryStream(File.ReadAllBytes(filePath));
                var reader = new BinaryReader(stream);
                reader.ReadBytes(80);
                int triangleCount = (int)reader.ReadUInt32();//Êîëè÷åñòâî òðåóãîëüíèêîâ
                var totalVertices = new HashSet<Vertex>();
                Object3D obj = new Object3D(screen3D.OpenGL, objectColor);
                obj.Name = Path.GetFileNameWithoutExtension(filePath);
                //Reading triangle polygons
                bool aborted = false;
                string fileName = Path.GetFileName(filePath);
                var progress = new Form4();
                if(triangleCount > 100000) progress.Show();
                await Task.Run(() =>
                {
                    for (int i = 0; i < triangleCount; i++)
                    {
                        var triangle = new Polygon(obj);
                        // Reading normal (12 bytes)
                        reader.ReadBytes(12);
                        // Reading the vertices of the triangles
                        if (i % 1000 == 0 && triangleCount > 100000) {
                            aborted = UpdateProgress(progress, i, triangleCount, fileName);
                            if (aborted) break;
                        }
                        for (int j = 0; j < 3; j++)
                        {
                            float x = reader.ReadSingle(), z = reader.ReadSingle(), y = reader.ReadSingle();
                            var vertex = new Vertex(x, y, z);
                            if (!totalVertices.Contains(vertex))
                            {
                                triangle.VerticesID.Add((ushort)obj.Vertices.Count);
                                obj.Vertices.Add(vertex);
                                totalVertices.Add(vertex);
                            }
                            else //Finding an index of vertex
                                for (int k = obj.Vertices.Count - 1; k >= 0; k--)
                                {
                                    Vertex v = obj.Vertices[k];
                                    if (vertex.X == v.X && vertex.Y == v.Y && vertex.Z == v.Z)
                                    {
                                        triangle.VerticesID.Add(k);
                                        break;
                                    }
                                }
                        }
                        obj.Polygons.Add(triangle);
                        // Reading attribute (2 bytes)
                        reader.ReadBytes(2);
                    }
                });
                progress.Close();
                if(!aborted) {
                    progress.Close();
                    //Setting a selection on this object
                    allObjects.Add(obj);
                    comboShapes.Items.Add(allObjects.Count + ". " + obj.Name);
                    comboShapes.SelectedIndex = allObjects.Count - 1;
                    cam.LookAt = obj.Position;
                    cam.Position = obj.FindSize();
                    file_Path = filePath;
                    Backup();
                }
                else MessageBox.Show(localizedLabels[126] + fileName + localizedLabels[128], "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(localizedLabels[73] + e.Message.ToLower(), "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            stream?.Close();
        }
        private void from_PRCT_format(string filePath, bool saveName = true)
        {//Loading objects from a file of the program's own format - prct
            try
            {//It's based on an XML file with object data and JPG texture files packed into a zip archive, which is the prct file
             //Unpacking data from the archive into a temporary folder
                string ID = Guid.NewGuid().GetHashCode() + "";
                var folderPath = Path.Combine(Path.GetTempPath(), ID);
                Vertex totalSize = new Vertex(0, 0, 0);
                Directory.CreateDirectory(folderPath);
                ZipFile.ExtractToDirectory(filePath, folderPath);
                //Reading parameters of each object from XML file
                var document = XElement.Load(Path.Combine(folderPath, "data.xml"));
                var xObjects = document.Element("Objects").Elements();
                var objectsInFile = new List<Object3D>();
                foreach (var xObject in document.Element("Objects").Elements())
                {//Reading object name
                    var name = new StringBuilder(xObject.Name.LocalName);
                    name.Remove(0, 1); name.Replace('_', ' ');
                    var obj = new Object3D(name.ToString(), (uint)xObject.Attribute("RenderMode"), screen3D.OpenGL);
                    obj.Name.Remove(0, 1);
                    //Reading a matrix with object properties
                    obj.Rotation = new Vertex((float)xObject.Attribute("Rotation.X"), (float)xObject.Attribute("Rotation.Y"), (float)xObject.Attribute("Rotation.Z"));
                    obj.Size = new Vertex((float)xObject.Attribute("Size.X"), (float)xObject.Attribute("Size.Y"), (float)xObject.Attribute("Size.Z"));
                    foreach (var xPoly in xObject.Elements("V"))
                    {//Reading the data about vertices
                        var vertex = new Vertex((float)xPoly.Attribute("X"), (float)xPoly.Attribute("Y"), (float)xPoly.Attribute("Z"));
                        obj.Vertices.Add(vertex);
                        if (vertex.X > totalSize.X) totalSize.X = vertex.X;
                        if (vertex.Y > totalSize.Y) totalSize.Y = vertex.Y;
                        if (vertex.Z > totalSize.Z) totalSize.Z = vertex.Z;
                    }
                    foreach (var xPoly in xObject.Elements("P"))
                    {//Reading the data about polygons
                        var polygon = new Polygon(obj);
                        foreach (var xVertex in xPoly.Elements("V")) polygon.VerticesID.Add((int)xVertex.Attribute("I"));
                        try
                        {
                            foreach (var xTexCoords in xPoly.Elements("T"))
                                polygon.TextureCoords.Add(new SharpDX.Half2((float)xTexCoords.Attribute("U"), (float)xTexCoords.Attribute("V")));
                        }
                        catch (Exception) { }
                        obj.Polygons.Add(polygon);
                    }
                    var imageName = (string)xObject.Attribute("Texture");
                    if (imageName.Length > 0)
                    {//Mapping a textures to the objects
                        obj.TexturePath = Path.Combine(folderPath, "Textures", imageName);
                        obj.setTexture(obj.TexturePath);
                    }
                    obj.color = new Vertex((float)xObject.Attribute("Red"), (float)xObject.Attribute("Green"), (float)xObject.Attribute("Blue"));
                    objectsInFile.Add(obj);
                }
                foreach (var obj in objectsInFile)
                {
                    allObjects.Add(obj);
                    comboShapes.Items.Add(allObjects.Count + ". " + obj.Name);
                }
                comboShapes.SelectedIndex = comboShapes.Items.Count - 1;
                if (saveName) {
                    cam.LookAt = new Vertex(0, 0, 0);
                    cam.Position = totalSize * 2;
                    file_Path = filePath;
                    changesSaved = true;
                } 
                Backup();
            }
            catch (Exception e)
            {
                MessageBox.Show(localizedLabels[73] + e.Message.ToLower(), "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void to_OBJ_format(string filePath)
        {//Saving objects to a text OBJ file by constructing an array of strings
            try {
                StringBuilder S = new StringBuilder();
                S.Append("# Penteract\n\n");
                int totalVertices = 0;
                Vertex commonCenter = new Vertex(0, 0, 0);
                foreach (var shape in allObjects) commonCenter += shape.Position;
                commonCenter /= allObjects.Count;
                foreach (var shape in allObjects) {
                    S.Append($"o {shape.Name.Replace(' ', '_')}\n");
                    foreach (var vertex in shape.Vertices)
                    {//Writing numbers to a file in international format - with floating POINT
                        var v = vertex - commonCenter;
                        string row = $"v {Math.Round(v.X, 4)} {Math.Round(v.Y, 4)} {Math.Round(v.Z, 4)}\n";
                        row = row.Replace(',', '.');
                        S.Append(row);
                    }
                    S.Append("\ns 0\n");
                    foreach (var polygon in shape.Polygons)
                    {//In the polygon data in the OBJ file - the numbers of their vertices in each object
                        StringBuilder t = new StringBuilder("f");
                        foreach (int i in polygon.VerticesID)
                        {
                            int index = totalVertices + i + 1;
                            t.Append(" " + index.ToString());
                        }
                        t.Append('\n'); S.Append(t);

                    }
                    totalVertices += shape.Vertices.Count;
                }
                File.WriteAllText(filePath, S.ToString());
                MessageBox.Show(localizedLabels[74] + filePath + '.', "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Information);
                changesSaved = true;//Parameter for checking whether changes are saved before closing the program

            }
            catch (Exception e)
            {
                MessageBox.Show(localizedLabels[73] + e.Message.ToLower(), "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void to_STL_format(string filePath)
        {
            FileStream stream = null;
            try {
                //Creating a file to write data to via the binary writer
                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                var writer = new BinaryWriter(stream);
                // Çàïèñü çàãîëîâêà
                byte[] header = new byte[84];
                writer.Write(header);
                int triangleCount = 0;
                Vertex commonCenter = new Vertex(0, 0, 0);
                foreach (var shape in allObjects) commonCenter += shape.Position;
                commonCenter /= allObjects.Count;
                foreach (var shape in allObjects)
                    foreach (var polygon in shape.Polygons)
                    {
                        int n = polygon.VerticesID.Count;
                        if (n >= 3)  // Splitting a polygon into triangles if it has more than 3 vertices
                            for (int i = 1; i < n - 1; i++, triangleCount++)
                            {
                                Vertex v1 = shape.Vertices[polygon.VerticesID[0]],
                                       v2 = shape.Vertices[polygon.VerticesID[i]],
                                       v3 = shape.Vertices[polygon.VerticesID[i + 1]],
                                       n1 = v1 - shape.Position, n2 = v2 - shape.Position, n3 = v3 - shape.Position,
                                       normal = new Vertex(n1.X + n2.X + n3.X, n1.Y + n2.Y + n3.Y, n1.Z + n2.Z + n3.Z);
                                v1 -= commonCenter;
                                v2 -= commonCenter;
                                v3 -= commonCenter;
                                normal.Normalize();
                                // Writing a normal vector
                                writer.Write(normal.X); writer.Write(normal.Y); writer.Write(normal.Z);
                                // Writing a triangle vertices
                                writer.Write(v1.X); writer.Write(v1.Z); writer.Write(v1.Y);
                                writer.Write(v2.X); writer.Write(v2.Z); writer.Write(v2.Y);
                                writer.Write(v3.X); writer.Write(v3.Z); writer.Write(v3.Y);
                                // Writing a 2-byte attribute
                                writer.Write((ushort)0);
                            }
                    }
                stream.Seek(80, SeekOrigin.Begin);
                writer.Write(triangleCount); // Writing a count of the triangles
                MessageBox.Show(localizedLabels[74] + filePath + '.', "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Information);
                changesSaved = true; // Parameter for checking for saving changes before closing the program
            }
            catch (Exception e)
            {
                MessageBox.Show(localizedLabels[73] + e.Message.ToLower(), "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            stream?.Close();
        }
 
        void to_PRCT_format(string filePath, bool message = true, bool shiftObjects = true)
        {//Saving objects to a file of the program's own format - prct
            try
            {//It's based on an XML file with object data and JPG texture files packed into a zip archive, which is the prct file
                //Ñîçäàíèå âðåìåííîé ïàïêè, ñîäåðæèìîå êîòîðîé óïàêîâûâàåòñÿ â zip-àðõèâ
                string ID = Guid.NewGuid().GetHashCode() + "";
                var folderPath = Path.Combine(Path.GetTempPath(), ID);
                while (Directory.Exists(folderPath))
                    folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().GetHashCode() + "");
                Directory.CreateDirectory(folderPath);
                Directory.CreateDirectory(folderPath + "\\Textures");
                
                //Creating XML-file with data about 3D-objects  
                var xObjects = new XElement("Objects");
                Vertex commonCenter = new Vertex(0, 0, 0);
                if (shiftObjects) {
                    foreach (var shape in allObjects) commonCenter += shape.Position;
                    commonCenter /= allObjects.Count;
                }
                foreach (var shape in allObjects)
                    if (shape.Polygons.Count > 0)
                    {//Saving the name and properties of objects
                        var xObject = new XElement(shape.correctString(shape.Name));
                        xObject.Add(new XAttribute("RenderMode", shape.RenderMode));

                        xObject.Add(new XAttribute("Position.X", shape.Position.X));
                        xObject.Add(new XAttribute("Position.Y", shape.Position.Y));
                        xObject.Add(new XAttribute("Position.Z", shape.Position.Z));

                        xObject.Add(new XAttribute("Rotation.X", shape.Rotation.X));
                        xObject.Add(new XAttribute("Rotation.Y", shape.Rotation.Y));
                        xObject.Add(new XAttribute("Rotation.Z", shape.Rotation.Z));

                        xObject.Add(new XAttribute("Size.X", shape.Size.X));
                        xObject.Add(new XAttribute("Size.Y", shape.Size.Y));
                        xObject.Add(new XAttribute("Size.Z", shape.Size.Z));

                        xObject.Add(new XAttribute("Red", shape.color.X));
                        xObject.Add(new XAttribute("Green", shape.color.Y));
                        xObject.Add(new XAttribute("Blue", shape.color.Z));
                        //Saving a textures of objects into JPG-files
                        shape.saveTexture(folderPath + "\\Textures\\");
                        xObject.Add(new XAttribute("Texture", shape.TextureFile));
                        foreach (var vertex in shape.Vertices)
                        {
                            var xVertex = new XElement("V");
                            Vertex v = vertex - commonCenter;
                            xVertex.Add(new XAttribute("X", Math.Round(v.X, 4)));
                            xVertex.Add(new XAttribute("Y", Math.Round(v.Y, 4)));
                            xVertex.Add(new XAttribute("Z", Math.Round(v.Z, 4)));
                            xObject.Add(xVertex);
                        }
                        foreach (var polygon in shape.Polygons)
                        {//Saving data about polygons and their vertex coordinates
                            var xPoly = new XElement("P");
                            foreach (int i in polygon.VerticesID)
                            {
                                var xVertex = new XElement("V");
                                xVertex.Add(new XAttribute("I", i));
                                xPoly.Add(xVertex);
                            }
                            if(polygon.TextureCoords.Count == polygon.VerticesID.Count)
                            {   
                                foreach(var UV in polygon.TextureCoords)
                                {
                                    var xTexcoords = new XElement("T");
                                    xTexcoords.Add(new XAttribute("U", Math.Round(UV.X, 4)));
                                    xTexcoords.Add(new XAttribute("V", Math.Round(UV.Y, 4)));
                                    xPoly.Add(xTexcoords);
                                }
                            }
                            xObject.Add(xPoly);
                        }
                        xObjects.Add(xObject);
                    }
                var document = new XElement("Scene");
                document.Add(xObjects);//Saving an XML-file
                File.WriteAllText(Path.Combine(folderPath, "data.xml"), $"{document}");
                if (File.Exists(filePath)) File.Delete(filePath);
                //Creating ZIP-archive and packing XML-file and textures
                ZipFile.CreateFromDirectory(folderPath, filePath);
                if (message) {
                    MessageBox.Show(localizedLabels[74] + filePath + '.', "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    changesSaved = true;//Parameter for checking whether changes are saved before closing the program
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(localizedLabels[73] + e.Message.ToLower(), "Penteract", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
    }
}