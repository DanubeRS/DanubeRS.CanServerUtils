using SkiaSharp;

namespace DanubeRS.CANServer.UI.SkiaElements;

public interface IRenderableControl<TPayload>
{
    public void Render(SKCanvas canvas, TPayload value, int width, int height, int x = 0, int y = 0);
}