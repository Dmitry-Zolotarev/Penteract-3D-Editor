using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using SharpGL;
using System;
using System.Threading.Tasks;

namespace Penteract
{
    public class Object3D
    {//A class describing an 3D objects in this program
        //Matrix of objects's properties
        public Vertex Position = new Vertex(0, 0, 0), Rotation = new Vertex(0, 0, 0), Size = new Vertex(1, 1, 1);
        public List<Polygon> Polygons = new List<Polygon>();//Polygons of this object 
        public List<Vertex> Vertices = new List<Vertex>();//Vertices of this object 
        public string Name = "", TextureFile = "", TexturePath = "";
        public Vertex color = new Vertex(0.8f, 1, 0.8f);
        public OpenGL gl;//3D space where the object is located
        public Texture Texture = new Texture();//A texture of this object
        public BitmapImage Texels;
        public uint RenderMode = OpenGL.GL_TRIANGLES;
        public bool drawMode = false;
        public float drawFrequency = 1;
        //Different constructors
        public Object3D()
        {
        }
        public Object3D(OpenGL GL, Color colour)
        {
            gl = GL;
            setColor(colour);
        }
        public Object3D(OpenGL GL)
        {
            gl = GL;
        }
        public Object3D(uint mode, OpenGL GL, Color colour)
        {
            RenderMode = mode; gl = GL; setColor(colour);
        }
        public Object3D(string name, uint mode, OpenGL GL) { Name = name; RenderMode = mode; gl = GL; }
        public void Render(LightSource lightSource)
        {//Start rendering an object via rendering polygons
            Texture.Bind(gl);
            gl.Color(color);
            if (RenderMode == OpenGL.GL_LINES || RenderMode == OpenGL.GL_POINTS) lightSource = null;
            if (lightSource != null)
            {//Gourau Lighting via Vertex Normals
                lightSource.Light(gl, color, this);
            }
            else gl.Disable(OpenGL.GL_LIGHTING);
            gl.Begin(RenderMode);//Render points
            if (RenderMode == OpenGL.GL_POINTS) foreach (var v in Vertices) gl.Vertex(v);
            else
            {//Render polygons
                foreach (var polygon in Polygons) polygon.Draw(RenderMode, lightSource);

            }
            gl.End();
        }
        public Object3D(Object3D other)
        {//Copy constructor
            Name = other.Name;
            drawMode = other.drawMode;
            TextureFile = other.TextureFile;
            gl = other.gl;
            setTexture(other.TexturePath);
            Position = other.Position;
            Rotation = other.Rotation;
            Size = other.Size;
            Texels = other.Texels;
            color = other.color;
            RenderMode = other.RenderMode;
            Vertices = new List<Vertex>(other.Vertices);
            foreach (var polygon in other.Polygons) Polygons.Add(new Polygon(polygon, this));
        }
        public void setColor(Color intColor) => color = new Vertex(intColor.R / 255f, intColor.G / 255f, intColor.B / 255f);
        public string correctString(string S)
        {//Replacing a spaces when saving objects to file
            var sb = new StringBuilder(S);
            for (int i = 0; i < sb.Length; i++) if (sb[i] == ' ' || sb[i] == ':') sb[i] = '_';
            return "_" + sb.ToString();
        }
        public void saveTexture(string folderPath)
        {//Saving texture when saving objects to PRCT file
            if (TextureFile.Length > 1 && !File.Exists(folderPath + TextureFile))
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(Texels));
                var stream = new FileStream(folderPath + TextureFile, FileMode.Create);
                encoder.Save(stream); stream.Dispose();
            }
        }
        public void setTexture(string filePath)
        {
            try
            {//Assigning a texture from a graphic file to an object
                TexturePath = filePath;
                if (TexturePath.Length > 1)
                {
                    foreach (var polygon in Polygons) polygon.SetTextureCoords();
                    Texture.Destroy(gl);
                    Texels = new BitmapImage(new Uri(TexturePath));
                    Texture.Create(gl, new Bitmap(TexturePath));
                    Texture.Bind(gl);
                    TextureFile = Path.GetFileName(TexturePath).GetHashCode() + ".jpg";
                    color = new Vertex(1, 1, 1);
                    RenderMode = OpenGL.GL_TRIANGLES;
                    for (int i = 0; i < Polygons.Count; i++) Polygons[i].SetTextureCoords();
                }
                else  {
                    setColor(Color.FromArgb(204, 255, 204));
                    TextureFile = "";
                    Texture.Destroy(gl);
                }
            }
            catch (Exception) {       
            }
        }
        public void DividePolygon(int i)
        {//Division of the n-gon into n parts by new vertex in center
            try {
                var poly = Polygons[i];
                Vertex newVertex = poly.Center();
                Vertices.Add(newVertex);
                int n = poly.VerticesID.Count;

                for (int j = 0; j < n; j++)
                {
                    var part = new Polygon(this);
                    part.VerticesID.Add(Vertices.Count - 1);
                    part.VerticesID.Add(poly.VerticesID[j]);
                    part.VerticesID.Add(poly.VerticesID[(j + 1) % n]);
                    part.SetTextureCoords();
                    Polygons.Add(part);
                }
            }
            catch(Exception) { }
            
        }
        public void ExtrudePolygon(int i)
        {//Extruding the polygon
            try
            {
                var copy = new Polygon(this);
                Polygons[i].SetTextureCoords();
                copy.TextureCoords = Polygons[i].TextureCoords;
                foreach (int j in Polygons[i].VerticesID) 
                {
                    copy.VerticesID.Add(Vertices.Count);
                    Vertices.Add(Vertices[j] + Polygons[i].Center() - Position); 
                }
                int n = Polygons[i].VerticesID.Count;
                for (int j = 0; j < n; j++)
                {//Creating side polygons of extrusion
                    int k = (j + 1) % n;
                    var connection = new Polygon(this);
                    connection.VerticesID.Add(Polygons[i].VerticesID[j]);
                    connection.VerticesID.Add(copy.VerticesID[j]);
                    connection.VerticesID.Add(copy.VerticesID[k]);
                    connection.VerticesID.Add(Polygons[i].VerticesID[k]);
                    connection.SetTextureCoords();
                    Polygons.Add(connection);
                }
                Polygons[i].VerticesID = copy.VerticesID;
            }
            catch (Exception) {  }    
        }
        public void Move(Vertex newPosition)
        {//Poligon shift to a new position
            Vertex delta = newPosition - Position;
            if (delta.Magnitude() > 0)
            {
                for (int i = 0; i < Vertices.Count; i++) Vertices[i] += delta;
                Position = newPosition;
            }
        }
        public void Rotate(Vertex d, Vertex center)
        {//Rotation a polygons around center of the object
            if (d.Magnitude() == 0) return;
            Rotation += d;
            d *= (float)(Math.PI / 180);//From degrees to radians
            for (int i = 0; i < Vertices.Count; i++)
            {
                var delta = Vertices[i] - center;
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
                }
                Vertices[i] = new Vertex(X, Y, Z) + center;
            }
        }
        public void FlipX()
        {//Flip by x
            for (int i = 0; i < Vertices.Count; i++)
            {
                float X = Vertices[i].X - Position.X;
                Vertices[i] = new Vertex(-X + Position.X, Vertices[i].Y, Vertices[i].Z);
            }
        }
        public void FlipY()
        {//Flip by x
            for (int i = 0; i < Vertices.Count; i++)
            {
                float Y = Vertices[i].Y - Position.Y;
                Vertices[i] = new Vertex(Vertices[i].X, -Y + Position.Y, Vertices[i].Z);
            }
        }
        public void FlipZ()
        {//Flip by x
            for (int i = 0; i < Vertices.Count; i++)
            {
                float Z = Vertices[i].Z - Position.Z;
                Vertices[i] = new Vertex(Vertices[i].X, Vertices[i].Y, -Z + Position.Z);
            }
        }
        public void Scale(Vertex newSize, Vertex center)
        {
            FindSize();
            Vertex t = Size;
            if (newSize.X == t.X && newSize.Y == t.Y && newSize.Z == t.Z) return;
            //Protection for division by zero
            if (newSize.X == 0) newSize.X = Size.X;
            if (newSize.Y == 0) newSize.Y = Size.Y;
            if (newSize.Z == 0) newSize.Z = Size.Z;
            Size = newSize;
            if (t.X == 0) t.X = 1;
            if (t.Y == 0) t.Y = 1;
            if (t.Z == 0) t.Z = 1;
            //Protection from negative size
            if (newSize.X < 0) newSize.X = (float)Math.Abs(newSize.X);
            if (newSize.Y < 0) newSize.Y = (float)Math.Abs(newSize.Y);
            if (newSize.Z < 0) newSize.Z = (float)Math.Abs(newSize.Z);
            Vertex scaling = new Vertex(newSize.X / t.X, newSize.Y / t.Y, newSize.Z / t.Z);
            //Loop for scaling a coordinates of the vertices
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertex p = (Vertices[i] - center) * scaling;
                Vertices[i] = p + center;
            }
        }
        public Color GetColor()
        {
            return Color.FromArgb((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255));
        }
        public void SetColor(Color intColor)
        {
            color = new Vertex(intColor.R / 255f, intColor.G / 255f, intColor.B / 255f);
        }
        public Vertex FindPosition()
        {//Updating position of the Object
            Vertex newPosition;
            if (Vertices.Count > 0)
            {
                newPosition = Vertices[0];
                for (int i = 1; i < Vertices.Count; i++) newPosition += Vertices[i];
                Position = newPosition / Vertices.Count;
            }
            return Position;
        }
        public Vertex FindSize()
        {
            if (Vertices.Count > 0)
            {
                Vertex min = Vertices[0], max = Vertices[0];
                foreach (var vertex in Vertices)
                {
                    if (vertex.X > max.X) max.X = vertex.X;
                    if (vertex.X < min.X) min.X = vertex.X;
                    if (vertex.Y > max.Y) max.Y = vertex.Y;
                    if (vertex.Y < min.Y) min.Y = vertex.Y;
                    if (vertex.Z > max.Z) max.Z = vertex.Z;
                    if (vertex.Z < min.Z) min.Z = vertex.Z;
                }
                Size = max - min;
                Size.X = Math.Abs(Size.X);
                Size.Y = Math.Abs(Size.Y);
                Size.Z = Math.Abs(Size.Z);
            }
            return Size;
        }
    }
    class Triangle : Object3D
    {
        public Triangle(OpenGL GL, Color colour, string name)
        {
            Name = name; gl = GL;
            float a = (2.0f / (float)Math.Sqrt(3));
            Polygons.Add(new Polygon(this));
            Vertices.Add(new Vertex(0, 0, a));
            Vertices.Add(new Vertex(a, 0, -a / 2));
            Vertices.Add(new Vertex(-a, 0, -a / 2));
            for(ushort i = 0; i < 3; i++) Polygons[0].VerticesID.Add(i);

            Polygons[0].SetTextureCoords();//Texture mapping
            SetColor(colour);//Applying a default color
            Scale(new Vertex(2, 0, 2), Position);
        }
    }
    class Square : Object3D
    {
        public Square(OpenGL GL, Color colour, string name)
        {
            Name = name; gl = GL;
            Polygons.Add(new Polygon(this));
            Vertices.Add(new Vertex(-1, 0, 1));
            Vertices.Add(new Vertex(1, 0, 1));
            Vertices.Add(new Vertex(1, 0, -1));
            Vertices.Add(new Vertex(-1, 0, -1));
            for (ushort i = 0; i < 4; i++) Polygons[0].VerticesID.Add(i);
            Polygons[0].SetTextureCoords();//Texture mapping
            SetColor(colour);//Applying a default color          
        }
    }
    
    class Circle : Object3D
    {
        public Circle(OpenGL GL, int n, Color colour, string name)
        {
            Name = n + name;
            gl = GL;
            Polygon polygon = new Polygon(this);
            double delta = 2 * Math.PI / n;
            //Добавление верщин
            for (int i = 0; i < n; i++) {
                float X = (float)Math.Cos(i * delta);
                float Z = (float)Math.Sin(i * delta);
                Vertices.Add(new Vertex(X, 0, Z));
                polygon.VerticesID.Add((n - i - 1));
            }
            polygon.SetTextureCoords();
            Polygons.Add(polygon);
            
            Scale(new Vertex(2, 0, 2), Position);
            SetColor(colour);//Applying a default color
            
        }
    }
    class Sphere : Object3D
    {
        public Sphere(OpenGL GL, int n, Color colour, string name)
        {
            gl = GL;
            var meridians = new List<List<int>>();
            var totalVertices = new HashSet<Vertex>();
            int n_vertices = (int)Math.Round(Math.Sqrt(n));
            double phi = Math.PI / n_vertices; // Delta of angle between meridians
            double theta = 2 * Math.PI / n_vertices; // Delta of angle between parallels
            // Цикл по параллелям (широте)
            for (int i = 0; i <= n_vertices; i++)
            {
                meridians.Add(new List<int>());
                //Creating a meridian of sphere
                for (int j = 0; j <= n_vertices; j++) 
                {
                    // Calculating the coordinates
                    float X = (float)(Math.Sin(i * phi) * Math.Cos(j * theta));
                    float Y = (float)(Math.Cos(i * phi));
                    float Z = (float)(Math.Sin(i * phi) * Math.Sin(j * theta));
                    Vertex v = new Vertex(X, Y, Z);
                    if (!totalVertices.Contains(v)) {
                        totalVertices.Add(v);
                        Vertices.Add(v * 2);
                    }
                    meridians[i].Add(Vertices.Count - 1);
                }
            }
            for (int i = 0; i < n_vertices; i++)
                for (int j = 0; j < n_vertices; j++)
                {
                    //Calculating a Texture coordinates
                    float s1 = 1 - (float)j / n_vertices, t1 = (float)i / n_vertices,
                          s2 = 1 - (float)(j + 1) / n_vertices, t2 = (float)(i + 1) / n_vertices;     
                    Polygon polygon = new Polygon(this);
                    polygon.VerticesID.Add(meridians[i][j]);
                    polygon.VerticesID.Add(meridians[i + 1][j]);
                    polygon.VerticesID.Add(meridians[i + 1][j + 1]);
                    polygon.VerticesID.Add(meridians[i][j + 1]);
                    //Texture coordinates
                    polygon.TextureCoords.Add(new SharpDX.Half2(s1, t1));
                    polygon.TextureCoords.Add(new SharpDX.Half2(s1, t2));
                    polygon.TextureCoords.Add(new SharpDX.Half2(s2, t2));
                    polygon.TextureCoords.Add(new SharpDX.Half2(s2, t1));
                    Polygons.Add(polygon);
                }
            Move(new Vertex(0, 0, 0));//Move to the center
            Name = Polygons.Count + name;
            SetColor(colour);//Applying a default color
        }
    }
    class Torus : Object3D
    {
        public Torus(OpenGL GL, int n, float innerRadius,Color colour, string name)
        {
            gl = GL;
            float outerRadius = 3 - innerRadius;
            var circles = new List<List<int>>();
            var totalVertices = new HashSet<Vertex>();
            int n_segments = (int)Math.Round(Math.Sqrt(n)); //Setting a number of torus segments
            double delta = 2 * Math.PI / n_segments; //Delta angle
            //Calculating vertices
            for (int i = 0; i <= n_segments; i++) {
                circles.Add(new List<int>());
                for (int j = 0; j <= n_segments; j++)
                {
                    float X = (float)((innerRadius + outerRadius * Math.Cos(j * delta)) * Math.Cos(i * delta));
                    float Y = (float)(Math.Sin(j * delta));
                    float Z = (float)((innerRadius + outerRadius * Math.Cos(j * delta)) * Math.Sin(i * delta));
                    Vertex vertex = new Vertex(X, Y, Z);
                    
                    if (!totalVertices.Contains(vertex))
                    {
                        totalVertices.Add(vertex);
                        Vertices.Add(vertex); 
                    }
                    circles[i].Add(Vertices.Count - 1);
                }
            }
            //Building a polygons
            for (int i = 0; i < n_segments; i++)
                for (int j = 0; j < n_segments; j++)
                {
                    Polygon polygon = new Polygon(this);
                    polygon.VerticesID.Add(circles[i][j]);
                    polygon.VerticesID.Add(circles[i + 1][j]);
                    polygon.VerticesID.Add(circles[i + 1][j + 1]);
                    polygon.VerticesID.Add(circles[i][j + 1]);
                    //Calculating texture coordinates: 
                    float s1 = 1 - (float)j / n_segments, t1 = (float)i / n_segments,
                          s2 = 1 - (float)(j + 1) / n_segments, t2 = (float)(i + 1) / n_segments;
                    polygon.TextureCoords.Add(new SharpDX.Half2(s1, t1));
                    polygon.TextureCoords.Add(new SharpDX.Half2(s1, t2));
                    polygon.TextureCoords.Add(new SharpDX.Half2(s2, t2));
                    polygon.TextureCoords.Add(new SharpDX.Half2(s2, t1));

                    Polygons.Add(polygon);
                }
            Name = Polygons.Count + name;
            Scale(new Vertex(4, 1, 4), Position);
            SetColor(colour); // Применение цвета по умолчанию
        }
    }
    class Pyramid : Object3D
    {
        public Pyramid(OpenGL GL, int n, Color colour, string tetra, string name1, string name2)
        {
            gl = GL;
            Name = n == 3 ? tetra : n + name1 + ' ' + name2.Split()[0].ToLower();
            double delta = 2 * Math.PI / n;
            var top = new Vertex(0, 1, 0);
            var bottom = new Polygon(this);

            Vertices.Add(top);//A top of the pyramid
            for (ushort i = 0; i < n; i++)
            {//Creating a bottom vertices
                float X = (float)Math.Cos(i * delta);
                float Z = (float)Math.Sin(i * delta);
                Vertices.Add(new Vertex(X, 0, Z));
                bottom.VerticesID.Add(i);
            }
            
            bottom.SetTextureCoords();
            Polygons.Add(bottom);

            for (ushort i = 0; i < n; i++)
            {//Creating a sides of pyramid
                var side = new Polygon(this);
                side.VerticesID.Add(i); // Current vertex
                side.VerticesID.Add(((i + 1) % n)); // Next vertex
                side.VerticesID.Add(n); // A top of the pyramid
                
                side.TextureCoords.Add(new SharpDX.Half2(1, i / (float)n));
                side.TextureCoords.Add(new SharpDX.Half2(1, (i + 1) / (float)n));
                side.TextureCoords.Add(new SharpDX.Half2(0, (i + 1) / (float)n));
                Polygons.Add(side);
            }
            Rotate(new Vertex(0, 45, 0), Position);
            Rotation *= 0; FindSize();
            Scale(new Vertex(2, 1.4f, 2), Position);
            SetColor(colour);//Applying a default color
        }
    }
    class Prism : Object3D
    {
        public Prism(OpenGL GL, int n, Color colour, string name1 = "", string name2 = "")
        {
            gl = GL;
            Name = n + name1 + ' ' + name2.Split()[0].ToLower();
            double delta = 2 * Math.PI / n;
            var top = new Polygon(this);
            var bottom = new Polygon(this);
            for (int i = 0; i < n; i++)
            {
                float X = (float)Math.Cos(i * delta), Z = (float)Math.Sin(i * delta);
                Vertices.Add(new Vertex(X, 1, Z));
                Vertices.Add(new Vertex(X, -1, Z));
                top.VerticesID.Add((Vertices.Count - 2));
                bottom.VerticesID.Add((Vertices.Count - 1));
            }
            top.SetTextureCoords();
            bottom.SetTextureCoords();
            Polygons.Add(top);
            for (int i = 0; i < n; i++)
            {//Creating a sides of prism
                var side = new Polygon(this);
                side.VerticesID.Add(top.VerticesID[i % n]);
                side.VerticesID.Add(bottom.VerticesID[i % n]);
                side.VerticesID.Add(bottom.VerticesID[(i + 1) % n]);
                side.VerticesID.Add(top.VerticesID[(i + 1) % n]);
                if (n > 4) {
                    side.TextureCoords.Add(new SharpDX.Half2(i / (float)n, 0));
                    side.TextureCoords.Add(new SharpDX.Half2(i / (float)n, 1));
                    side.TextureCoords.Add(new SharpDX.Half2((i + 1) / (float)n, 1));
                    side.TextureCoords.Add(new SharpDX.Half2((i + 1) / (float)n, 0));
                }
                else side.SetTextureCoords();
                Polygons.Add(side);
            }
            Polygons.Add(bottom);
            Scale(new Vertex(2, Size.Y, 2), Position);
            SetColor(colour);//Applying a default color
        }
        public Object3D ToCube()
        {
            Rotate(new Vertex(0, 45f, 0), Position); 
            Rotation *= 0;
            Scale(new Vertex(2, 2, 2), Position);
            return this;
        }
    }
}

