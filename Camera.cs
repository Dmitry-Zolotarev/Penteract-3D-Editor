using SharpGL.SceneGraph;
using System;
using SharpGL;

namespace Penteract
{
    class Camera
    {//Class that describes the camera through which the 3D scene is projected onto the screen
        private OpenGL gl;
        public float fov = 80, near = 0.1f, far = 1024,//Camera field of vision and looking distance
                     AspectRatio = 1.5f,
                     MoveSpeed = 0.1f,
                     MouseSensivity = 0.005f;
        public Vertex LookAt, Position;
        public Camera(OpenGL m_gl, Vertex m_Position, Vertex m_cen)
        {//Initialization of the new camera
            gl = m_gl;
            Position = m_Position;//Camera coordinates
            LookAt = m_cen;//Point, where camera looks
        }
        public void Look(){//Update camera position and direction
            gl.Perspective(fov, AspectRatio, near, far);
            gl.LookAt(Position.X, Position.Y, Position.Z, LookAt.X, LookAt.Y, LookAt.Z, 0, 1, 0);
        }
        public void CenterRotateX(float angle)
        {//Camera horizontal rotation
            angle *= -MouseSensivity;
            Vertex viewPoint = Position - LookAt;
            float cos = (float)Math.Cos(angle), sin = (float)Math.Sin(angle);
            viewPoint.X = viewPoint.Z * sin + viewPoint.X * cos;
            viewPoint.Z = viewPoint.Z * cos - viewPoint.X * sin;
            Position = viewPoint + LookAt;
        }
        public void CenterRotateY(float angle)
        {//Camera vertical rotation
            angle *= -MouseSensivity;
            Vertex viewPoint = Position - LookAt;

            float cos = (float)Math.Cos(angle), sin = (float)Math.Sin(angle),
                rxz = (float)Math.Sqrt(Math.Pow(viewPoint.X, 2) + Math.Pow(viewPoint.Z, 2));
            
            float newAngle = viewPoint.Y * sin + rxz * cos;
            if (newAngle > 0)
            {
                viewPoint.Y = viewPoint.Y * cos - rxz * sin;
                float d = newAngle / rxz;
                viewPoint.X = d * viewPoint.X;
                viewPoint.Z = d * viewPoint.Z;
            }
            Position = viewPoint + LookAt;
        }
        public Vertex Rotate(Vertex dir, double dx)
        {//Rotate the camera around its axis along the X axis
            float sin = (float)Math.Sin(dx), cos = (float)Math.Cos(dx);
            return new Vertex(dir.X * cos + dir.Z * sin, dir.Y, -dir.X * sin + dir.Z * cos);
        }
        
        public float LookDistance()
        {
            return (float)(Position - LookAt).Magnitude();
        }
        public void Zoom(float speed)
        {//Change camera scaling via mouse wheel
            if (LookDistance() + speed > near * 2 && speed < 0 || LookDistance() + speed < far / 3 && speed > 0) 
            {
                Position += (Position - LookAt) * speed;
                gl.Perspective(fov, AspectRatio, near, LookDistance() * 2);
            }
        }
        public void MoveTo(Vertex v)
        {//Move a camera to a given point
            Position -= LookAt - v;
            LookAt = v;
        }   
    }
}
