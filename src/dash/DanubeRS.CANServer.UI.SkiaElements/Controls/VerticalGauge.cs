using System.Security.Cryptography.X509Certificates;
using SkiaSharp;

namespace DanubeRS.CANServerDask.SkiaElements.Controls;

public class VerticalGauge : IRenderableControl
{
    private readonly float _width;
    private readonly float _height;
    private readonly float _maxValue;
    private readonly float _value;
    private readonly float _minValue;

    public VerticalGauge(float width, float height, float minValue, float maxValue, float value)
    {
        _width = width;
        _height = height;
        _maxValue = maxValue;
        _value = value;
        _minValue = minValue;
    }
    public void Render(SKCanvas canvas)
    {
        canvas.Restore();
        DrawBackground(canvas);
        DrawValueMarkers(canvas);
        DrawValueBar(canvas, _value);
        DrawValueIndicatorBox(canvas, _value);
    }

    public void DrawValueIndicatorBox(SKCanvas canvas, float value)
    {
        var halfHeight = (_height - 8) / 2;
        var valueHeightInPx = halfHeight / Math.Abs(value < 0 ? _minValue : _maxValue) * value;
        var outerRect = new SKRect(24, halfHeight - valueHeightInPx - 18, 84, halfHeight - valueHeightInPx + 18);
        var innerRect = new SKRect(26, halfHeight - valueHeightInPx - 16, 82, halfHeight - valueHeightInPx + 16);
        canvas.DrawRect(outerRect, new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            StrokeWidth = 0,
            Color = SKColors.LightGray,
        });
        canvas.DrawRect(innerRect, new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = value < 0 ? SKColors.LimeGreen : SKColors.Blue
        });
        var indicatorPath = new SKPath();
        indicatorPath.MoveTo(outerRect.Left, outerRect.Top + 8);
        indicatorPath.LineTo(4, halfHeight - valueHeightInPx);
        indicatorPath.LineTo(outerRect.Left, outerRect.Bottom - 8);
        indicatorPath.Close();
        canvas.DrawPath(indicatorPath, new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            StrokeWidth = 0,
            Color = SKColors.LightGray
        });
        
        // canvas.Save();
        
        var mainTextPaint = new SKPaint()
        {
            IsAntialias = true,
            Color = value < 0 ? SKColors.Black : SKColors.White,
            TextSize = 24f,
            Typeface = SKTypeface.FromFamilyName("monospace")
        };
        
        var subTextPaint = new SKPaint()
        {
            IsAntialias = true,
            Color = value < 0 ? SKColors.Black : SKColors.White,
            TextSize = 16f,
            Typeface = SKTypeface.FromFamilyName("monospace")
        };
        
        var mainTextBounds = new SKRect();
        var subTextBounds = new SKRect();
        var text = value.ToString("N0");
        mainTextPaint.MeasureText(text, ref mainTextBounds);
        subTextPaint.MeasureText(text, ref subTextBounds);
        
        canvas.ClipRect(innerRect);
        canvas.DrawText(text, 30, halfHeight - valueHeightInPx + 12, mainTextPaint);
        canvas.DrawText("0", 30 + mainTextBounds.Width + 2, halfHeight - valueHeightInPx + 12 + 15, subTextPaint);
        canvas.DrawText("1", 30 + mainTextBounds.Width + 2, halfHeight - valueHeightInPx + 12 + 15 - (subTextBounds.Height + 2), subTextPaint);
        canvas.DrawText("2", 30 + mainTextBounds.Width + 2, halfHeight - valueHeightInPx + 12 + 15 - (subTextBounds.Height + 2) * 2, subTextPaint);
        
        canvas.Save();
        canvas.Restore();

    }

    public void DrawValueBar(SKCanvas canvas, float value)
    {
        var halfHeight = (_height - 8) / 2;
        var valueHeightInPx = halfHeight / Math.Abs(value < 0 ? _minValue : _maxValue) * value;
        var rect = new SKRect(4, halfHeight - valueHeightInPx, 14, halfHeight);
        canvas.DrawRect(rect, new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = value < 0 ? SKColors.LimeGreen : SKColors.Blue
        });
    }

    public void DrawValueMarkers(SKCanvas canvas)
    {
        var halfHeight = (_height - 8) / 2;
        
        // Render halfway line
        canvas.DrawLine(4, halfHeight, 104, halfHeight, new SKPaint()
        {
            StrokeWidth = 1,
            Color = SKColors.LightGray
        });

        DrawUpperValueMarkers(canvas, halfHeight);
        DrawLowerValueMarkers(canvas, halfHeight);
    }
    
    private void DrawLowerValueMarkers(SKCanvas canvas, float halfHeight)
    {
        var step = 1;
        const int markerThresholdInPx = 50;
        while (halfHeight / (_minValue * -1) * (step * 10) < markerThresholdInPx)
        {
            step *= 2;
        }

        var unitToPx = halfHeight / _minValue;
        var markerSpacing = unitToPx * step * 10;

        foreach (var marker in Enumerable.Range(1, (int)Math.Floor(_minValue / (step * 10) * -1)))
        {
            var value = marker * step * 10;
            var posInPx = halfHeight - markerSpacing * marker;
            if (posInPx > _height - 20) continue;
            canvas.DrawLine(4, posInPx, 24, posInPx, new SKPaint()
            {
                StrokeWidth = 1,
                Color = SKColors.LightGray
            });

            var textPaint = new SKPaint()
            {
                IsAntialias = true,
                Color = SKColors.LightGray,
                TextSize = 16f,
                Typeface = SKTypeface.FromFamilyName("monospace")
            };
            
            var textBounds = new SKRect();
            var text = value.ToString("N0");
            textPaint.MeasureText(text, ref textBounds);

            canvas.DrawText(text, 30, posInPx + textBounds.Height / 2, textPaint);
        }
    }

    private void DrawUpperValueMarkers(SKCanvas canvas, float halfHeight)
    {
        var step = 1;
        const int markerThresholdInPx = 50;
        while (halfHeight / _maxValue * (step * 10) < markerThresholdInPx)
        {
            step *= 2;
        }

        var unitToPx = halfHeight / _maxValue;
        var markerSpacing = unitToPx * step * 10;

        foreach (var marker in Enumerable.Range(1, (int)Math.Floor(_maxValue / (step * 10))))
        {
            var value = marker * step * 10;
            var posInPx = halfHeight - markerSpacing * marker;
            if (posInPx < 20) continue;
            canvas.DrawLine(4, posInPx, 24, posInPx, new SKPaint()
            {
                StrokeWidth = 1,
                Color = SKColors.LightGray
            });

            var textPaint = new SKPaint()
            {
                IsAntialias = true,
                Color = SKColors.LightGray,
                TextSize = 16f,
                Typeface = SKTypeface.FromFamilyName("monospace")
            };
            var textBounds = new SKRect();
            var text = value.ToString("N0");
            textPaint.MeasureText(text, ref textBounds);

            canvas.DrawText(text, 30, posInPx + textBounds.Height / 2, textPaint);
        }
    }

    private void DrawBackground(SKCanvas canvas)
    {
        var bgRect = new SKRect(4, 4, 104, _height - 8);
        canvas.DrawRect(bgRect, new SKPaint()
       {
           Style = SKPaintStyle.Fill,
           Color = SKColors.Black
       }); 
       canvas.DrawRect(bgRect, new SKPaint()
       {
           Style = SKPaintStyle.Stroke,
           Color = SKColors.LightGray,
           StrokeWidth = 2
       });
    }
}