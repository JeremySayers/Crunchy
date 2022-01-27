using Crunchy;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

WindowSetup();

void WindowSetup()
{
    var nativeWindowSettings = new NativeWindowSettings()
    {
        Size = new Vector2i(640, 320),
        Title = "Crunchy - Chip8 Emulator",
        // This is needed to run on macos
        Flags = ContextFlags.ForwardCompatible,
    };

    using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
    {
        window.Run();
    }
}

