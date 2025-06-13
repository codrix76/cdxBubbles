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

        public IEnumerable<BubblePlacement> PlaceBubbles(IEnumerable<Size> bubbleSizes, BubbleTrackAlignment alignment, double startAngleRad, double endAngleRad)
        {
            var sizes = bubbleSizes.ToList();
            if (sizes.Count == 0)
                yield break;

            if (alignment == BubbleTrackAlignment.Start)
            {
                foreach (var placement in PlaceForward(sizes, startAngleRad))
                    yield return placement;
            }
            else if (alignment == BubbleTrackAlignment.End)
            {
                sizes.Reverse();
                foreach (var placement in PlaceBackward(sizes, endAngleRad))
                    yield return placement;
            }
            else if (alignment == BubbleTrackAlignment.Center)
            {
                // 1. Gesamtlänge berechnen
                double totalLength = 0.0;
                var tangents = new List<Vector>();
                foreach (var size in sizes)
                {
                    // grob mittige Tangente annehmen (wird überschrieben)
                    tangents.Add(new Vector(1, 0));
                    totalLength += 2 * ComputeProjectedRadius(size, new Vector(1, 0)) + _spacing;
                }
                totalLength -= _spacing; // letzter Abstand zählt nicht

                // 2. Mittelpunktwinkel berechnen
                double midAngle = NormalizeAngle(startAngleRad + (endAngleRad - startAngleRad) / 2);

                // 3. Startwinkel finden, der totalLength/2 vor midAngle liegt
                double arcCenter = _path.GetArcLength(midAngle);
                double arcStart = arcCenter - totalLength / 2;
                arcStart = Math.Max(0, arcStart);
                double correctedStartAngle = _path.GetAngleAtArcLength(arcStart);

                foreach (var placement in PlaceForward(sizes, correctedStartAngle))
                    yield return placement;
            }
        }

        private IEnumerable<BubblePlacement> PlaceForward(IList<Size> sizes, double startAngleRad)
        {
            Size firstSize = sizes[0];
            Vector initialTangent = _path.GetTangent(startAngleRad);
            double initialProjectedRadius = ComputeProjectedRadius(firstSize, initialTangent);
            double initialArc = _path.GetArcLength(startAngleRad) + initialProjectedRadius;
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
                // if (currentArc >= _path.TotalArcLength)
                //     yield break;

                currentAngle = _path.GetAngleAtArcLength(currentArc);

                yield return new BubblePlacement
                {
                    Center = _path.GetPoint(currentAngle),
                    AngleRad = currentAngle,
                    Size = size
                };
            }
        }

        private IEnumerable<BubblePlacement> PlaceBackward(IList<Size> sizes, double startAngleRad)
        {
            Size firstSize = sizes[0];
            Vector initialTangent = _path.GetTangent(startAngleRad);
            double initialProjectedRadius = ComputeProjectedRadius(firstSize, -initialTangent);
            double initialArc = _path.GetArcLength(startAngleRad) - initialProjectedRadius;
            double currentAngle = _path.GetAngleAtArcLength(Math.Max(0, initialArc));
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
                double projectedRadius = ComputeProjectedRadius(size, -tangent);
                double stepLength = 2 * projectedRadius + _spacing;

                currentArc -= stepLength;
                // if (currentArc <= 0)
                //     yield break;

                currentAngle = _path.GetAngleAtArcLength(currentArc);

                yield return new BubblePlacement
                {
                    Center = _path.GetPoint(currentAngle),
                    AngleRad = currentAngle,
                    Size = size
                };
            }
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

        private static double NormalizeAngle(double angleRad)
        {
            while (angleRad < 0) angleRad += 2 * Math.PI;
            while (angleRad >= 2 * Math.PI) angleRad -= 2 * Math.PI;
            return angleRad;
        }
    }

    public record BubblePlacement
    {
        public Point Center;
        public double AngleRad;
        public Size Size;
    }
}
