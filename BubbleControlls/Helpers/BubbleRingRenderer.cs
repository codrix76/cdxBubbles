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
        if (!data.IsGlowActive) return;
        double t = 2.0;
        byte o1 = (byte)Math.Clamp(data.FillOpacity, 0, 255);
        byte o2 = (byte)Math.Clamp(data.FillOpacity + 50, 0, 255);
        byte o3 = (byte)Math.Clamp(data.FillOpacity + 150, 0, 255);

        Color baseColor = (data.Fill as SolidColorBrush)?.Color ?? Colors.CornflowerBlue;

        SolidColorBrush c1 = new(Color.FromArgb(o1, baseColor.R, baseColor.G, baseColor.B));
        SolidColorBrush c2 = new(Color.FromArgb(o2, baseColor.R, baseColor.G, baseColor.B));
        SolidColorBrush c3 = new(Color.FromArgb(o3, baseColor.R, baseColor.G, baseColor.B));

        foreach (var scale in new[] { t * 2, t, 0 })
        {
            double innerRx = data.RadiusX - data.PathWidth + scale;
            double innerRy = data.RadiusY - data.PathWidth + scale;
            double outerRx = data.RadiusX - scale;
            double outerRy = data.RadiusY - scale;

            var innerPath = GeometryHelper.CreateArc(data.Center, innerRx, innerRy, data.StartAngleDeg + data.RotationDeg, data.EndAngleDeg + data.RotationDeg);
            var outerPath = GeometryHelper.CreateArc(data.Center, outerRx, outerRy, data.StartAngleDeg + data.RotationDeg, data.EndAngleDeg + data.RotationDeg);

            dc.DrawGeometry(null, new Pen(c1, t * 0.8), innerPath);
            dc.DrawGeometry(null, new Pen(c2, t), outerPath);
            dc.DrawGeometry(null, new Pen(c3, t * 1.2), outerPath);
        }
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
