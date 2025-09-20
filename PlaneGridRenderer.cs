using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;
using System;


namespace Penteract
{
    class Axes
    {
        private Vertex center = new Vertex(0, 0, 0), x, y, z;
        public Axes(float lineLength)
        {//Scheme of axis coordinates, where the X axis is red, the Y axis is green, the Z axis is blue
            x = new Vertex(1, 0, 0) * lineLength;
            y = new Vertex(0, 1, 0) * lineLength;
            z = new Vertex(0, 0, 1) * lineLength;
        }
        public void Render(OpenGL gl)
        {//Render an lines for coordinate axes
            gl.Disable(OpenGL.GL_LIGHT0);
            gl.Disable(OpenGL.GL_LIGHTING);
            var texture = new Texture();
            texture.Create(gl);
            texture.Bind(gl);
            texture.Destroy(gl);

            gl.LineWidth(3);
            gl.Color(1, 0.0f, 0.0f, 0.5f);
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(center);
            gl.Vertex(x);
            gl.End();

            gl.Color(0.0f, 1.0f, 0.0f, 0.5f);
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(center);
            gl.Vertex(y);
            gl.End();

            gl.Color(0.0f, 0.0f, 1.0f, 0.5f);
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(center);
            gl.Vertex(z);
            gl.End();

        }
    }
    class PlaneGridRenderer
    {//Drawing a plane grid
        private float size;
        public PlaneGridRenderer(float x) => size = x;
        public void Render(OpenGL gl, Camera cam)
        {
            try {
                gl.Disable(OpenGL.GL_LIGHT0);
                gl.Disable(OpenGL.GL_LIGHTING);
                var texture = new Texture();
                texture.Create(gl);
                texture.Bind(gl);
                texture.Destroy(gl);
                gl.Color(1, 1, 1, 0.5f);
                float width = 1 / cam.LookDistance();
                if (width > 0.5f) width = 0.5f;
                gl.LineWidth(width);
                gl.Begin(OpenGL.GL_LINES);

                for (float i = -size; i < size; i++)
                {
                    gl.Vertex(new Vertex(i, 0f, -size));
                    gl.Vertex(new Vertex(i, 0f, size));
                    gl.Vertex(new Vertex(-size, 0f, i));
                    gl.Vertex(new Vertex(size, 0f, i));
                }
                gl.End();
            }
            catch(Exception) { }
           
        }
    }
}
