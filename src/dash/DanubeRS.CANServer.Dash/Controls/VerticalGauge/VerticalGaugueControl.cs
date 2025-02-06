using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;

namespace DanubeRS.CANServer.Dash.Controls.VerticalGauge;

public class VerticalGaugeControl : Control
{
    private readonly GlyphRun _noSkia;
    public VerticalGaugeControl()
    {
        ClipToBounds = true;
        var text = "Current rendering API is not Skia";
        var glyphs = text.Select(ch => Typeface.Default.GlyphTypeface.GetGlyph(ch)).ToArray();
        _noSkia = new GlyphRun(Typeface.Default.GlyphTypeface, 12, text.AsMemory(), glyphs); 
    }

    private class VerticalGaugeDrawOp(Rect bounds, GlyphRun noSkia) : ICustomDrawOperation
    {
        public void Dispose()
        {
            // No-op
        }
        public bool HitTest(Point p) => false;
        public bool Equals(ICustomDrawOperation other) => false;
        
        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature == null)
            {
                context.DrawGlyphRun(Brushes.Black, noSkia.TryCreateImmutableGlyphRunReference());
                return;
            }

            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;
            canvas.Save();

            var render = new CANServerDask.SkiaElements.Controls.VerticalGauge((float)bounds.Width, (float)bounds.Height, 310, -70);
            render.Render(canvas);
            canvas.Restore();
        }

        public Rect Bounds => bounds;
    }
    
    public override void Render(DrawingContext context)
    {
        context.Custom(new VerticalGaugeDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), _noSkia));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}