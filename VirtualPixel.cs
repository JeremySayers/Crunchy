using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crunchy
{
    public class VirtualPixel
    {
        private int _vertexArrayObject = 0;
        public bool active = false;

        public VirtualPixel(int vertexArrayObject)
        {
            _vertexArrayObject = vertexArrayObject;
        }

        public void Draw()
        {
            if (!active)
            {
                return;
            }

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}
