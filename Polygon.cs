using SharpGL.SceneGraph;
using System.Collections.Generic;
using SharpGL;
using System;
using SharpDX;

namespace Penteract
{
    public class Polygon
    {
        public Object3D Host;//Host object of the polygon
        public List<int> VerticesID = new List<int>();
        public List<Half2> TextureCoords = new List<Half2>();//Coordinates for texture mapping
        public Polygon(Object3D host) { Host = host; }//Basic constructor
        public Polygon(Polygon other, Object3D newHost)
        {//Copy constructor
            Host = newHost;
            VerticesID = new List<int>(other.VerticesID);
            TextureCoords = new List<Half2>(other.TextureCoords);
        }
        public void Draw(uint renderMode, LightSource lightSource)
        {//Drawing a vertices of the polygon
            //Checking for texture coordinates
            bool has_UV = TextureCoords.Count == VerticesID.Count;
            if (renderMode != OpenGL.GL_LINES)
                for (int i = 1; i < VerticesID.Count - 1; i++)
                {
                    RenderVertex(0, has_UV, lightSource);
                    RenderVertex(i, has_UV, lightSource);
                    RenderVertex(i + 1, has_UV, lightSource);
                }
            else for (int i = 0; i <= VerticesID.Count; i++) Host.gl.Vertex(Host.Vertices[VerticesID[i % VerticesID.Count]]);
        }
        void RenderVertex(int i, bool has_UV, LightSource lightSource)
        {
            if (lightSource != null) Host.gl.Normal(Host.Vertices[VerticesID[i]] - Host.Position);
            if (has_UV) Host.gl.TexCoord(TextureCoords[i].X, TextureCoords[i].Y);
            Host.gl.Vertex(Host.Vertices[VerticesID[i]]);
        }
        public void SetTextureCoords()
        {//The coordinates OpenGL needs for texture mapping
            if (TextureCoords.Count != VerticesID.Count) {
                TextureCoords.Clear();
                if (VerticesID.Count == 3)
                {
                    TextureCoords.Add(new Half2(0f, 0f));
                    TextureCoords.Add(new Half2(0f, 1f));
                    TextureCoords.Add(new Half2(1f, 0f));
                }
                else if (VerticesID.Count == 4)
                {
                    TextureCoords.Add(new Half2(0f, 0f));
                    TextureCoords.Add(new Half2(0f, 1f));
                    TextureCoords.Add(new Half2(1f, 1f));
                    TextureCoords.Add(new Half2(1f, 0f));
                }
                else for (int i = 0; i < VerticesID.Count; i++)
                        TextureCoords.Add(new Half2(i / (float)VerticesID.Count, 0));
            }
        }
        public Vertex Center()
        {//Finding polygon center
            Vertex center = new Vertex(0, 0, 0);
            foreach (int i in VerticesID) center += Host.Vertices[i];
            return center / VerticesID.Count;
        }
        public void Move(Vertex d) {
            for (int i = 0; i < VerticesID.Count; i++) Host.Vertices[VerticesID[i]] += d;
        }//Move the polygon's vertices
        public void Rotate(Vertex d)
        {//Rotation a polygon around it's center
            if (d.Magnitude() == 0) return;
            d *= (float)(Math.PI / 180);//From degrees to radians
            Vertex center = Center();
            foreach (var i in VerticesID)
            {
                var delta = Host.Vertices[i] - center;
                float X = delta.X, Y = delta.Y, Z = delta.Z;
                if (d.X != 0)
                {
                    float y = Y;//Rotation around X axis
                    Y = (float)(y * Math.Cos(d.X) - Z * Math.Sin(d.X));
                    Z = (float)(y * Math.Sin(d.X) + Z * Math.Cos(d.X));
                }
                if (d.Y != 0)
                {
                    float z = Z;//Rotation around Y axis
                    Z = (float)(z * Math.Cos(d.Y) - X * Math.Sin(d.Y));
                    X = (float)(z * Math.Sin(d.Y) + X * Math.Cos(d.Y));
                }
                if (d.Z != 0)
                {
                    float x = X;//Rotation around Z axis
                    X = (float)(x * Math.Cos(d.Z) - Y * Math.Sin(d.Z));
                    Y = (float)(x * Math.Sin(d.Z) + Y * Math.Cos(d.Z));
                }//Finding a new normal vector for vertex
                Host.Vertices[i] = new Vertex(X, Y, Z) + center;
            }
        }
        public void Scale(Vertex scaling)
        {//Scaling a polygon across it's vertex coordinates
            if (scaling.X == 1 && scaling.Y == 1 && scaling.Z == 1) return;
            Vertex center = Center();
            //Protection for division by zero
            if (scaling.X == 0) scaling.X = 0.0001f;
            if (scaling.Y == 0) scaling.Y = 0.0001f;
            if (scaling.Z == 0) scaling.Z = 0.0001f;
            //Protection from negative size
            if (scaling.X < 0) scaling.X = (float)Math.Abs(scaling.X);
            if (scaling.Y < 0) scaling.Y = (float)Math.Abs(scaling.Y);
            if (scaling.Z < 0) scaling.Z = (float)Math.Abs(scaling.Z);
            //Loop for scaling a coordinates of the vertices
            foreach (int i in VerticesID) {
                Vertex p = (Host.Vertices[i] - center) * scaling;
                Host.Vertices[i] = p + center;
            }
        }
    }
}