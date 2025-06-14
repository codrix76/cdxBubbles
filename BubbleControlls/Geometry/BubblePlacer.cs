using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BubbleControlls.Models;

namespace BubbleControlls.Geometry
{
    public class BubblePlacer
    {
        private readonly EllipsePath _path;
        private readonly double _spacing;

        public BubblePlacer(EllipsePath path, double spacing)
        {
            _path = path;
            _spacing = spacing;
        }

        public IEnumerable<BubblePlacement> PlaceBubbles(IEnumerable<Size> sizes, BubbleTrackAlignment alignment, double startAngleRad, double endAngleRad, double scrollOffset)
        {
            var sizeList = sizes.ToList();
            if (sizeList.Count == 0)
                yield break;

            switch (alignment)
            {
                case BubbleTrackAlignment.Start:
                    foreach (var p in PlaceForward(sizeList, startAngleRad, scrollOffset))
                        yield return p;
                    break;
                case BubbleTrackAlignment.End:
                    foreach (var p in PlaceBackward(sizeList, endAngleRad, scrollOffset))
                        yield return p;
                    break;
                case BubbleTrackAlignment.Center:
                    foreach (var p in PlaceCentered(sizeList, startAngleRad, endAngleRad, scrollOffset))
                        yield return p;
                    break;
            }
        }

        private IEnumerable<BubblePlacement> PlaceForward(IList<Size> sizes, double startAngleRad, double scrollOffset)
        {
            Size firstSize = sizes[0];
            Vector initialTangent = _path.GetTangent(startAngleRad);
            double initialProjectedRadius = ComputeProjectedRadius(firstSize, initialTangent);

            double initialArc = _path.GetArcLength(startAngleRad) + scrollOffset + initialProjectedRadius;
            double currentAngle = _path.GetAngleAtArcLength(initialArc);
            double currentArc = _path.GetArcLength(currentAngle);

            yield return new BubblePlacement
            {
                Center = _path.GetPoint(currentAngle),
                AngleRad = currentAngle,
                Size = firstSize
            };

            for (int i = 1; i < sizes.Count; i++)
            {
                Size size = sizes[i];
                Vector tangent = _path.GetTangent(currentAngle);
                double projectedRadius = ComputeProjectedRadius(size, tangent);
                double stepLength = 2 * projectedRadius + _spacing;

                currentArc += stepLength;
                currentAngle = _path.GetAngleAtArcLength(currentArc);

                yield return new BubblePlacement
                {
                    Center = _path.GetPoint(currentAngle),
                    AngleRad = currentAngle,
                    Size = size
                };
            }
        }

        private IEnumerable<BubblePlacement> PlaceBackward(IList<Size> sizes, double endAngleRad, double scrollOffset)
        {
            Size firstSize = sizes[0];
            Vector initialTangent = _path.GetTangent(endAngleRad);
            double initialProjectedRadius = ComputeProjectedRadius(firstSize, initialTangent);

            double initialArc = _path.GetArcLength(endAngleRad) - scrollOffset - initialProjectedRadius;
            double currentAngle = _path.GetAngleAtArcLength(initialArc);
            double currentArc = _path.GetArcLength(currentAngle);

            yield return new BubblePlacement
            {
                Center = _path.GetPoint(currentAngle),
                AngleRad = currentAngle,
                Size = firstSize
            };

            for (int i = 1; i < sizes.Count; i++)
            {
                Size size = sizes[i];
                Vector tangent = _path.GetTangent(currentAngle);
                double projectedRadius = ComputeProjectedRadius(size, tangent);
                double stepLength = 2 * projectedRadius + _spacing;

                currentArc -= stepLength;
                currentAngle = _path.GetAngleAtArcLength(currentArc);

                yield return new BubblePlacement
                {
                    Center = _path.GetPoint(currentAngle),
                    AngleRad = currentAngle,
                    Size = size
                };
            }
        }

        private IEnumerable<BubblePlacement> PlaceCentered(IList<Size> sizes, double startAngleRad, double endAngleRad, double scrollOffset)
        {
            double centerAngle = (startAngleRad + endAngleRad) / 2.0;

            var forward = PlaceForward(sizes, centerAngle, scrollOffset).ToList();
            double centerOffset = ComputeTotalArcLength(forward) / 2.0;

            // Shift zurück um die Hälfte der Gesamtlänge
            return PlaceForward(sizes, centerAngle, scrollOffset - centerOffset);
        }

        public static double ComputeProjectedRadius(Size size, Vector direction)
        {
            double dx = size.Width / 2.0;
            double dy = size.Height / 2.0;
            return Math.Sqrt(
                Math.Pow(dx * Math.Abs(direction.X), 2) +
                Math.Pow(dy * Math.Abs(direction.Y), 2)
            );
        }

        private double ComputeTotalArcLength(IEnumerable<BubblePlacement> placements)
        {
            double total = 0;
            foreach (var p in placements)
            {
                double r = ComputeProjectedRadius(p.Size, _path.GetTangent(p.AngleRad));
                total += 2 * r + _spacing;
            }
            return total;
        }
    }

    public record BubblePlacement
    {
        public Point Center;
        public double AngleRad;
        public Size Size;

        public override string ToString() => $"Center=({Center.X:F2}, {Center.Y:F2}), Angle={AngleRad:F4} rad, Size=({Size.Width:F1}, {Size.Height:F1})";
    }
}
