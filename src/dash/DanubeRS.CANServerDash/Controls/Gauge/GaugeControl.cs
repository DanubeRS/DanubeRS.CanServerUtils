using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;

namespace DanubeRS.CANServerDash.Controls.Gauge;

public class Gauge : Control
{
    private readonly GlyphRun _noSkia;

    public Gauge()
    {
        ClipToBounds = true;
        var text = "Current rendering API is not Skia";
        var glyphs = text.Select(ch => Typeface.Default.GlyphTypeface.GetGlyph(ch)).ToArray();
        _noSkia = new GlyphRun(Typeface.Default.GlyphTypeface, 12, text.AsMemory(), glyphs);
    }

    public static readonly StyledProperty<decimal> ValueProperty =
        AvaloniaProperty.Register<Gauge, decimal>(nameof(Value), 0);
    public static readonly StyledProperty<decimal> MinValueProperty =
        AvaloniaProperty.Register<Gauge, decimal>(nameof(MinValue), -5000);
    public static readonly StyledProperty<decimal> MaxValueProperty = AvaloniaProperty.Register<Gauge, decimal>(nameof(MaxValue), 5000);

    private decimal Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private decimal MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    private decimal MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }
    

    class GaugeDrawOp : ICustomDrawOperation
    {
        private readonly decimal _value;
        private readonly IImmutableGlyphRunReference? _noSkia;
        private readonly decimal _minValue;
        private readonly decimal _maxValue;

        public GaugeDrawOp(Rect bounds, GlyphRun noSkia, decimal value, decimal minValue, decimal maxValue)
        {
            _value = value;
            Bounds = bounds;
            _noSkia = noSkia.TryCreateImmutableGlyphRunReference();
            _minValue = minValue;
            _maxValue = maxValue;
        }
        public void Dispose()
        {
            // No-op
        }

        public Rect Bounds { get; }
        public bool HitTest(Point p) => false;
        public bool Equals(ICustomDrawOperation other) => false;
        
        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature == null)
            {
                context.DrawGlyphRun(Brushes.Black, _noSkia);
                return;
            }

            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;
            canvas.Save();

            var render = new CANServerDask.SkiaElements.Controls.Gauge(Math.Min(Math.Max(_value, _minValue), _maxValue), (float)Bounds.Width, (float)Bounds.Height, _minValue, _maxValue);
            render.Render(canvas);
            canvas.Restore();
        }
    }
    
    public override void Render(DrawingContext context)
    {
        context.Custom(new GaugeDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), _noSkia, Value, MinValue, MaxValue));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}