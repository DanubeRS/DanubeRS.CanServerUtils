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

public class VerticalGauge : Control
{
    private readonly GlyphRun _noSkia;
    public VerticalGauge()
    {
        ClipToBounds = true;
        var text = "Current rendering API is not Skia";
        var glyphs = text.Select(ch => Typeface.Default.GlyphTypeface.GetGlyph(ch)).ToArray();
        _noSkia = new GlyphRun(Typeface.Default.GlyphTypeface, 12, text.AsMemory(), glyphs); 
    }
    
    public static readonly StyledProperty<decimal> ValueProperty =
        AvaloniaProperty.Register<VerticalGauge, decimal>(nameof(Value), 0);

    public static readonly StyledProperty<decimal> MinValueProperty =
        AvaloniaProperty.Register<VerticalGauge, decimal>(nameof(MinValue), 0);

    public static readonly StyledProperty<decimal> MaxValueProperty =
        AvaloniaProperty.Register<VerticalGauge, decimal>(nameof(MaxValue), 100);
    
    public static readonly StyledProperty<int> MultiplierProperty =
        AvaloniaProperty.Register<VerticalGauge, int>(nameof(Multiplier), 1);
    
    public decimal Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public decimal MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    public decimal MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public int Multiplier
    {
        get => GetValue(MultiplierProperty);
        set => SetValue(MultiplierProperty, value);
    }

    private class VerticalGaugeDrawOp(Rect bounds, GlyphRun noSkia, decimal minValue, decimal maxValue, decimal value, int multiplier) : ICustomDrawOperation
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

            var render = new CANServerDask.SkiaElements.Controls.VerticalGauge((float)bounds.Width,
                (float)bounds.Height, (float)minValue, (float)maxValue, (float)value/multiplier);
            render.Render(canvas);
            canvas.Restore();
        }

        public Rect Bounds => bounds;
    }
    
    public override void Render(DrawingContext context)
    {
        context.Custom(new VerticalGaugeDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), _noSkia, MinValue, MaxValue, Value, Multiplier));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}