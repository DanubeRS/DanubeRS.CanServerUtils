using SkiaSharp;

namespace DanubeRS.CANServerDask.SkiaElements;

public interface IRenderableControl
{
    public void Render(SKCanvas canvas);
}