using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crunchy
{
    public class Window : GameWindow
    {
        private readonly Chip _chip;
        private Shader _shader;
        private readonly VirtualScreen _virtualScreen;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _chip = new Chip();
            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _virtualScreen = new VirtualScreen();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            _virtualScreen.Setup();
            _shader.Use();

            SetupChip();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            _virtualScreen.Draw();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Check if the Escape button is currently being pressed.
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                // If it is, close the window.
                Close();
            }

            _chip.Step();
            _virtualScreen.Update(_chip.displayBuffer);

            base.OnUpdateFrame(e);
        }

        private void SetupChip()
        {
            var rom = File.ReadAllBytes(@"Roms/ibm.ch8");
            Console.WriteLine(rom);

            _chip.LoadRom(rom);
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // When the window gets resized, we have to call GL.Viewport to resize OpenGL's viewport to match the new size.
            // If we don't, the NDC will no longer be correct.
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
