using SharpGL.SceneGraph;
using System.Collections.Generic;

namespace Penteract
{
    public class State
    {//A backup for Undo and Redo operations
        public List<Object3D> allObjects = new List<Object3D>();
        public int selectedIndex;
        public Vertex lightColor, backgroundColor;
        public State()
        {
        }
        public State(Form1 form)
        {
            foreach (var obj in form.allObjects) allObjects.Add(new Object3D(obj));
            selectedIndex = form.comboShapes.SelectedIndex;
            lightColor = form.lightSource.lightColor;
            backgroundColor = new Vertex(form.backroundColor.R / 255f, form.backroundColor.G / 255f, form.backroundColor.B / 255f);
        }
    }
    
}
