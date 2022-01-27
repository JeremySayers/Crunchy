using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crunchy
{
    public class VirtualScreen
    {
        private const int width = 64;
        private const int height = 32;
        private const int numberOfVertices = 18;

        private VirtualPixel[] _virtualPixels = Array.Empty<VirtualPixel>();

        public VirtualScreen()
        {
            _virtualPixels = new VirtualPixel[width * height];            
        }

        public void Update(byte[] displayBuffer)
        {
            for (int i = 0; i < width * height; i++)
            {
                _virtualPixels[i].active = displayBuffer[i] == 0xFF;
            }
        }

        public void DrawOne(int index)
        {
            _virtualPixels[index].Draw();
        }

        public void Draw()
        {
            for (int i = 0; i < _virtualPixels.Length; i++)
            {
                _virtualPixels[i].Draw();
            }
        }
        public void Setup()
        {
            for (int i = 0; i < _virtualPixels.Length; i++)
            {
                var xGridLocation = i % width;
                var yGridLocation = i / width;

                float pixelWidth = 1.0f / width;
                float pixelHeight = 1.0f / height;

                float[] vertices = new float[numberOfVertices];

                // top left corner
                vertices[0] = (pixelWidth * xGridLocation * 2) - 1.0f;
                vertices[1] = 1.0f - (pixelHeight * yGridLocation * 2);
                vertices[2] = 0.0f;

                // bottom left corner
                vertices[3] = (pixelWidth * xGridLocation * 2) - 1.0f;
                vertices[4] = 1.0f - ((pixelHeight * yGridLocation + pixelHeight) * 2);
                vertices[5] = 0.0f;

                // bottom right corner
                vertices[6] = ((pixelWidth * xGridLocation + pixelWidth) * 2) - 1.0f;
                vertices[7] = 1.0f - ((pixelHeight * yGridLocation + pixelHeight) * 2);
                vertices[8] = 0.0f;

                // top left corner
                vertices[9] = (pixelWidth * xGridLocation * 2) - 1.0f;
                vertices[10] = 1.0f - (pixelHeight * yGridLocation * 2);
                vertices[11] = 0.0f;

                // top right corner
                vertices[12] = ((pixelWidth * xGridLocation + pixelWidth) * 2) - 1.0f;
                vertices[13] = 1.0f - (pixelHeight * yGridLocation * 2);
                vertices[14] = 0.0f;

                // bottom right corner
                vertices[15] = ((pixelWidth * xGridLocation + pixelWidth) * 2) - 1.0f;
                vertices[16] = 1.0f - ((pixelHeight * yGridLocation + pixelHeight) * 2);
                vertices[17] = 0.0f;

                int vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                var vertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(vertexArrayObject);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                _virtualPixels[i] = new VirtualPixel(vertexArrayObject);
            }
        }
    }
}
