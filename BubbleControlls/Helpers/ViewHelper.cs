using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BubbleControlls.Helpers
{
    public static class ViewHelper
    {
        public static double DegToRad(double deg) => deg * Math.PI / 180.0;
        public static double RadToDeg(double rad) => rad * 180.0 / Math.PI;

        public static System.Windows.Shapes.Line DrawLine(Point from, Point to, Brush color, double thickness, bool hitTest)
        {
            var line = new System.Windows.Shapes.Line
            {
                X1 = from.X,
                Y1 = from.Y,
                X2 = to.X,
                Y2 = to.Y,
                Stroke = color,
                StrokeThickness = thickness,
                IsHitTestVisible = hitTest
            };
            return line;
        }
        public static Path DrawArc(Point center, double radius, double startAngleRad, double endAngleRad,
                           Brush stroke, double thickness, bool hitTest = false)
        {
            Point start = new Point(
                center.X + radius * Math.Cos(startAngleRad),
                center.Y + radius * Math.Sin(startAngleRad));

            Point end = new Point(
                center.X + radius * Math.Cos(endAngleRad),
                center.Y + radius * Math.Sin(endAngleRad));

            bool isLargeArc = Math.Abs(endAngleRad - startAngleRad) > Math.PI;

            var arcSegment = new ArcSegment
            {
                Point = end,
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = isLargeArc
            };

            var figure = new PathFigure
            {
                StartPoint = start,
                Segments = new PathSegmentCollection { arcSegment }
            };

            var geometry = new PathGeometry { Figures = new PathFigureCollection { figure } };

            return new Path
            {
                Data = geometry,
                Stroke = stroke,
                StrokeThickness = thickness,
                IsHitTestVisible = hitTest
            };
        }
        public static System.Windows.Shapes.Path DrawArc(Point center, double rx, double ry, Brush stroke, double thickness, bool hitTest
            , SweepDirection sweepDirection = SweepDirection.Clockwise, bool isLargeArc = false)
        {
            var figure = new PathFigure { StartPoint = new Point(rx, center.Y) };
            var arc = new ArcSegment
            {
                Point = new Point(center.X, ry),
                Size = new Size(rx, ry),
                SweepDirection = sweepDirection,
                IsLargeArc = isLargeArc
            };
            figure.Segments.Add(arc);

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            var path = new System.Windows.Shapes.Path
            {
                Stroke = stroke,
                StrokeThickness = thickness,
                Data = geometry,
                IsHitTestVisible = hitTest
            };

            return path;
        }

        public static Path CreateMarker(Point origin, double angleRadian, double distance, double radius = 3, Brush? color = null)
        {
            color ??= Brushes.Red;

            double x = origin.X + distance * Math.Cos(angleRadian);
            double y = origin.Y + distance * Math.Sin(angleRadian);

            var geometry = new EllipseGeometry(new Point(x,y), radius, radius);
            var path = new Path
            {
                Data = geometry,
                Fill = Brushes.Red,
                Stroke = color
            };

            return path;
        }
    }
}
