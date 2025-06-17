using SkiaSharp;

namespace DanubeRS.CANServerDask.SkiaElements.Controls;

public record GaugeValue(decimal Value);

public class Gauge : IRenderableControl<GaugeValue>
{
    private readonly string? _name;
    private readonly float _minValue;
    private readonly float _maxValue;
    private readonly int _precision;

    public Gauge(string? name, decimal minValue, decimal maxValue,
        int precision = 0)
    {
        _name = name;
        _minValue = (float)minValue;
        _maxValue = (float)maxValue;
        _precision = precision;
    }

    public void Render(SKCanvas canvas, GaugeValue value, int width, int height, int x = 0, int y = 0)
    {
        canvas.Save();
        var valueNum = decimal.Round(value.Value, _precision, MidpointRounding.ToPositiveInfinity);
        var size = Math.Min(width, height);
        var centerX = width / 2 + x;
        var centerY = height / 2 + y;
        var scale = size / 210f;

        canvas.Translate(centerX, centerY);
        canvas.Scale(scale);

        // DrawBackground(canvas, size);
        DrawGauge(canvas);
        DrawNeedle(canvas, valueNum);
        DrawNeedleScrew(canvas);
        DrawValueText(canvas, valueNum);
        canvas.Restore();
    }

    /// <summary>
    ///     Draws the background of the gauge.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="size">The size of the canvas.</param>
    private static void DrawBackground(
        SKCanvas canvas,
        float size)
    {
        canvas.DrawRect(new SKRect(-size / 2, -size / 2, size / 2, size / 2),
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Blue,
            });
    }

    /// <summary>
    ///     Draws the gauge on the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    private static void DrawGauge(
        SKCanvas canvas)
    {
        SKRect rect = new(-100, -100, 100, 100);
        rect.Inflate(-10, -10);

        DrawArc(canvas, rect, 135, 270, SKColors.DarkGray);
    }

    /// <summary>
    ///     Draws an arc on the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="rect">The rectangle bounding the arc.</param>
    /// <param name="startAngle">The starting angle of the arc.</param>
    /// <param name="sweepAngle">The sweep angle of the arc.</param>
    /// <param name="color">The color of the arc.</param>
    private static void DrawArc(
        SKCanvas canvas,
        SKRect rect,
        float startAngle,
        float sweepAngle,
        SKColor color)
    {
        using SKPath path = new();

        path.AddArc(rect, startAngle, sweepAngle);

        canvas.DrawPath(path, new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = 5
        });
    }

    /// <summary>
    ///     Draws the needle of the gauge.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="value">The value represented by the needle.</param>
    private void DrawNeedle(
        SKCanvas canvas,
        decimal value)
    {
        var clampValue = Math.Min(_maxValue, Math.Max(_minValue, (float)value));
        var angle = -135f + ((float)clampValue - _minValue) / (_maxValue - _minValue) * 270f;
        canvas.Save();
        canvas.RotateDegrees(angle);

        SKPaint paint = new()
        {
            IsAntialias = true,
            Color = SKColors.LightGray
        };

        SKPath needlePath = new();
        needlePath.MoveTo(0, -76);
        needlePath.LineTo(-6, 0);
        needlePath.LineTo(6, 0);
        needlePath.Close();

        canvas.DrawPath(needlePath, paint);
        canvas.Restore();
    }


    /// <summary>
    ///     Draws the screw at the center of the needle.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    private void DrawNeedleScrew(
        SKCanvas canvas)
    {
        canvas.DrawCircle(0, 0, 10, new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = SKColors.Gray
        });
    }

    /// <summary>
    ///     Draws the value text and unit text on the gauge.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    private void DrawValueText(
        SKCanvas canvas, decimal value)
    {
        SKPaint textPaint = new()
        {
            IsAntialias = true,
            Color = SKColors.White,
            TextSize = 12f,
            Typeface = SKTypeface.FromFamilyName("monospace")
        };
        if (_name != null)
            DrawUnitText(canvas, _name, 95, textPaint);
        textPaint.TextSize = 16;
        DrawUnitText(canvas, value.ToString("N0"), 75, textPaint);
    }

    /// <summary>
    ///     Draws a unit text on the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="y">The y-coordinate of the text.</param>
    /// <param name="paint">The paint to use for drawing the text.</param>
    private static void DrawUnitText(
        SKCanvas canvas,
        string text,
        float y,
        SKPaint paint)
    {
        SKRect textBounds = new();
        paint.MeasureText(text, ref textBounds);
        canvas.DrawText(text, -textBounds.MidX, y - textBounds.Height, paint);
    }
}