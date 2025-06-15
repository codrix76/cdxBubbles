using System.Windows;
using System.Windows.Media;

namespace BubbleControlls.Geometry;

public static class GeometryHelper
{
    public static double NormalizeRad(double rad)
    {
        while (rad < 0) rad += 2 * Math.PI;
        while (rad >= 2 * Math.PI) rad -= 2 * Math.PI;
        return rad;
    }
    
    public static double GetArcBetween(double startRad, double endRad)
    {
        startRad = NormalizeRad(startRad);
        endRad = NormalizeRad(endRad);

        double diff = Math.Abs(startRad - endRad);
        return Math.Min(diff, 2 * Math.PI - diff);
    }
    public static double DegToRad(double degree)
    {
        return degree * Math.PI / 180.0;
    }
    public static double RadToDeg(double rad) => rad * 180.0 / Math.PI;

    public static Point EllipticalPoint(Point center, double rx, double ry, double angleRad)
    {
        return new Point(
            center.X + rx * Math.Cos(angleRad),
            center.Y + ry * Math.Sin(angleRad)
        );
    }

    public static System.Windows.Media.Geometry CreateArc(Point center, double rx, double ry, double startDeg, double endDeg)
    {
        double startRad = DegToRad(startDeg);
        double endRad = DegToRad(endDeg);
        Point start = EllipticalPoint(center, rx, ry, startRad);
        Point end = EllipticalPoint(center, rx, ry, endRad);

        bool isLargeArc = Math.Abs(GetArcSweep(startRad, endRad)) > Math.PI;

        var geo = new StreamGeometry();
        using (var ctx = geo.Open())
        {
            ctx.BeginFigure(start, false, false);
            ctx.ArcTo(end, new Size(rx, ry), 0, isLargeArc, SweepDirection.Clockwise, true, false);
        }
        geo.Freeze();
        return geo;
    }

    public static double GetArcSweep(double startRad, double endRad)
    {
        double sweep = endRad - startRad;
        if (sweep < 0) sweep += 2 * Math.PI;
        return sweep;
    }

    public static SolidColorBrush WithOpacity(Brush brush, int opacity)
    {
        if (brush is SolidColorBrush solid)
        {
            var color = solid.Color;
            color.A = (byte)Math.Clamp(opacity, 0, 255);
            return new SolidColorBrush(color);
        }
        return new SolidColorBrush(Color.FromArgb((byte)Math.Clamp(opacity, 0, 255), 100, 149, 237)); // fallback CornflowerBlue
    }
}