using EEDN.Render;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

var window = new EednWindow(new GameWindowSettings()
{
    RenderFrequency = 120,
    UpdateFrequency = 120
}, new NativeWindowSettings()
{
    Title = "EEDN",
    APIVersion = new Version(4, 6),
    Size = new Vector2i(1366, 768)
});
window.Run();
