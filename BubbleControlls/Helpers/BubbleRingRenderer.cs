using System.Windows;
using System.Windows.Media;
using BubbleControlls.Geometry;
using BubbleControlls.Models;

namespace BubbleControlls.Helpers;

public static class BubbleRingRenderer
{
    public static void DrawRing(DrawingContext dc, BubbleRingRenderData data)
    {
        double outerRx = data.RadiusX;
        double outerRy = data.RadiusY;
        double innerRx = data.RadiusX - data.PathWidth;
        double innerRy = data.RadiusY - data.PathWidth;

        if (innerRx < 0 || innerRy < 0)
            return;

        StreamGeometry ringGeometry = new StreamGeometry();
        using (StreamGeometryContext ctx = ringGeometry.Open())
        {
            double sa = GeometryHelper.DegToRad(data.StartAngleDeg + data.RotationDeg);
            double ea = GeometryHelper.DegToRad(data.EndAngleDeg + data.RotationDeg);

            Point outerStart = GeometryHelper.EllipticalPoint(data.Center, outerRx, outerRy, sa);
            Point outerEnd   = GeometryHelper.EllipticalPoint(data.Center, outerRx, outerRy, ea);
            Point innerEnd   = GeometryHelper.EllipticalPoint(data.Center, innerRx, innerRy, ea);
            Point innerStart = GeometryHelper.EllipticalPoint(data.Center, innerRx, innerRy, sa);

            double sweep = GeometryHelper.GetArcSweep(sa, ea);
            bool isLargeArc = Math.Abs(sweep) > Math.PI;

            ctx.BeginFigure(outerStart, true, true);
            ctx.ArcTo(outerEnd, new Size(outerRx, outerRy), 0, isLargeArc, SweepDirection.Clockwise, true, false);
            ctx.LineTo(innerEnd, true, false);
            ctx.ArcTo(innerStart, new Size(innerRx, innerRy), 0, isLargeArc, SweepDirection.Counterclockwise, true, false);
        }
        ringGeometry.Freeze();

        Brush fill = GeometryHelper.WithOpacity(data.Fill, data.FillOpacity);
        Pen border = new Pen(GeometryHelper.WithOpacity(data.Border, data.BorderOpacity), data.BorderThickness);

        dc.DrawGeometry(fill, border, ringGeometry);
    }

    public static void DrawGlow(DrawingContext dc, BubbleRingRenderData data)
    {
        double thickness = 2;
        double innerRx = data.RadiusX - data.PathWidth;
        double innerRy = data.RadiusY - data.PathWidth;
        double outerRx = data.RadiusX;
        double outerRy = data.RadiusY;
        
        byte opacity1 = (byte)Math.Clamp(data.RingOpacity, 0, 255);
        byte opacity2 = (byte)Math.Clamp(data.RingOpacity + 50, 0, 255);
        byte opacity3 = (byte)Math.Clamp(data.RingOpacity + 150, 0, 255);

        Color baseColor = (data.Fill as SolidColorBrush)?.Color ?? Colors.CornflowerBlue;

        SolidColorBrush innerBrush = new(Color.FromArgb(opacity1, baseColor.R, baseColor.G, baseColor.B));
        SolidColorBrush midBrush = new(Color.FromArgb(opacity2, baseColor.R, baseColor.G, baseColor.B));
        SolidColorBrush outerBrush = new(Color.FromArgb(opacity3, baseColor.R, baseColor.G, baseColor.B));

        var innerPath1 = CreateRingArcPath(innerRx + thickness * 2, innerRy+ thickness * 2, data);
        var innerPath2 = CreateRingArcPath(innerRx + thickness, innerRy + thickness, data);
        var innerPath3 = CreateRingArcPath(innerRx, innerRy, data);
        var outerPath1 = CreateRingArcPath(outerRx - thickness * 2, outerRy - thickness * 2, data);
        var outerPath2 = CreateRingArcPath(outerRx - thickness, outerRy - thickness, data);
        var outerPath3 = CreateRingArcPath(outerRx, outerRy, data);
        
        // Innerer Pfad (Glow von innen nach Mitte)
        dc.DrawGeometry(null, new Pen(innerBrush, thickness * 0.8), innerPath1);
        dc.DrawGeometry(null, new Pen(midBrush,   thickness), innerPath2);
        dc.DrawGeometry(null, new Pen(outerBrush, thickness * 1.2), innerPath3);
        //
        // // Äußerer Pfad (Glow von außen nach Mitte)
        dc.DrawGeometry(null, new Pen(innerBrush, thickness * 0.8), outerPath1);
        dc.DrawGeometry(null, new Pen(midBrush,   thickness), outerPath2);
        dc.DrawGeometry(null, new Pen(outerBrush, thickness * 1.2), outerPath3);

        
    }
    private static System.Windows.Media.Geometry CreateRingArcPath(double rx, double ry, BubbleRingRenderData data)
    {
        var geo = new StreamGeometry();
        using (var ctx = geo.Open())
        {
            Point start = new Point(
                data.Center.X + rx * Math.Cos(data.StartAngleRad + data.RingRotationRad),
                data.Center.Y + ry * Math.Sin(data.StartAngleRad + data.RingRotationRad));

            Point end = new Point(
                data.Center.X + rx * Math.Cos(data.EndAngleRad + data.RingRotationRad),
                data.Center.Y + ry * Math.Sin(data.EndAngleRad + data.RingRotationRad));

            double sweep = data.EndAngleRad - data.StartAngleRad;
            if (sweep <= 0)
                sweep += 2 * Math.PI;
            bool isLargeArc = Math.Abs(sweep) > Math.PI;

            ctx.BeginFigure(start, false, false);
            ctx.ArcTo(end, new Size(rx, ry), 0, isLargeArc, SweepDirection.Clockwise, true, false);
        }
        geo.Freeze();
        return geo;
    }
    public static Rect DrawArrow(DrawingContext dc, Point center, double angleRad, bool isLeft, Brush brush, double pathWidth, double height)
    {
        Point p1 = new(
            center.X - pathWidth * Math.Cos(angleRad),
            center.Y - pathWidth * Math.Sin(angleRad));
        Point p2 = center;

        Point midpoint = new((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        Vector tangent = new(-Math.Sin(angleRad), Math.Cos(angleRad));
        if (isLeft) tangent *= -1;

        Point p3 = new(midpoint.X + height * tangent.X, midpoint.Y + height * tangent.Y);

        StreamGeometry geo = new();
        using (var ctx = geo.Open())
        {
            ctx.BeginFigure(p1, true, true);
            ctx.LineTo(p2, true, false);
            ctx.LineTo(p3, true, false);
        }
        geo.Freeze();

        dc.DrawGeometry(brush, new Pen(brush, 1), geo);

        double minX = Math.Min(p1.X, Math.Min(p2.X, p3.X));
        double maxX = Math.Max(p1.X, Math.Max(p2.X, p3.X));
        double minY = Math.Min(p1.Y, Math.Min(p2.Y, p3.Y));
        double maxY = Math.Max(p1.Y, Math.Max(p2.Y, p3.Y));

        return new Rect(new Point(minX, minY), new Point(maxX, maxY));
    }
}
