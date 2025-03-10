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

namespace DanubeRS.CANServer.Dash.Controls.Gauge;

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
        AvaloniaProperty.Register<Gauge, decimal>(nameof(MinValue), 0);

    public static readonly StyledProperty<decimal> MaxValueProperty =
        AvaloniaProperty.Register<Gauge, decimal>(nameof(MaxValue), 100);

    public static readonly StyledProperty<string?> GuageNameProperty =
        AvaloniaProperty.Register<Gauge, string?>(nameof(GuageName), null);

    public static readonly StyledProperty<int> PrecisionProperty =
        AvaloniaProperty.Register<Gauge, int>(nameof(Precision), 0);

    public string? GuageName
    {
        get => GetValue(GuageNameProperty);
        set => SetValue(GuageNameProperty, value);
    }

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

    public int Precision
    {
        get => GetValue(PrecisionProperty);
        set => SetValue(PrecisionProperty, value);
    }


    private class GaugeDrawOp : ICustomDrawOperation
    {
        private readonly string? _name;
        private readonly decimal? _value;
        private readonly IImmutableGlyphRunReference? _noSkia;
        private readonly decimal _minValue;
        private readonly decimal _maxValue;
        private readonly int _precision;

        public GaugeDrawOp(Rect bounds, GlyphRun noSkia, string? name, decimal? value, decimal minValue,
            decimal maxValue, int precision = 0)
        {
            _name = name;
            _value = value;
            Bounds = bounds;
            _noSkia = noSkia.TryCreateImmutableGlyphRunReference();
            _minValue = minValue;
            _maxValue = maxValue;
            _precision = precision;
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

            var render = new CANServerDask.SkiaElements.Controls.Gauge(_name,
                Math.Min(Math.Max(_value.GetValueOrDefault(0), _minValue), _maxValue), (float)Bounds.Width,
                (float)Bounds.Height, _minValue, _maxValue, _precision);
            render.Render(canvas);
            canvas.Restore();
        }
    }

    public override void Render(DrawingContext context)
    {
        context.Custom(new GaugeDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), _noSkia, GuageName, Value, MinValue,
            MaxValue));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}