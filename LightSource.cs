using SharpGL;
using SharpGL.SceneGraph;
using System.Drawing;

namespace Penteract
{
    public class LightSource
    {
        public Vertex Position, lightColor = new Vertex(1.92f, 1.92f, 1.92f);
        public LightSource(Vertex position) { Position = position;}
        public void Light(OpenGL gl, Vertex polygonColor, Object3D shape)
        {//Setting a default OpenGL lighting
            polygonColor.Normalize();
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, Position - shape.Position);//Position of the light source
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, polygonColor);//Color of the surface
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, lightColor + polygonColor);//Color and brightness of the light
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
        }
        public Color GetColor()
        {//Convert and normalize RGB color from float vector
            return Color.FromArgb((int)(lightColor.X * 100f), (int)(lightColor.Y * 100f), (int)(lightColor.Z * 100f));
        }
        public void SetColor(Color color)
        {//Convert and normalize RGB color to float vector
            lightColor = new Vertex(color.R / 100f, color.G / 100f, color.B / 100f);
        }
    }
}
