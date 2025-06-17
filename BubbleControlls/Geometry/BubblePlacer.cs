using System.Diagnostics;
using System.Windows;

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

        public IEnumerable<BubblePlacement> PlaceBubbles(IEnumerable<Size> sizes, double startAngleRad)
        {
            var sizeList = sizes.ToList();
            if (sizeList.Count == 0)
                yield break;
            Debug.WriteLine($"PlaceBubbles: startAngleRad: {startAngleRad}");
            foreach (var p in PlaceForward(sizeList, startAngleRad))
                yield return p;
        }

        private IEnumerable<BubblePlacement> PlaceForward(IList<Size> sizes, double startAngleRad)
        {
            double currentArc = _path.GetArcLength(startAngleRad);
            double currentAngle = _path.GetAngleAtArcLength(currentArc);
            double stepLength = 0;
            for (int i = 0; i < sizes.Count; i++)
            {
                Size size = sizes[i];
                Vector tangent = _path.GetTangent(currentAngle);
                double projectedRadius = ComputeProjectedRadius(size, tangent);
                if (i == 0)
                {
                    stepLength = projectedRadius + _spacing;
                }
                else
                {
                    stepLength = 2 * projectedRadius + _spacing;
                }

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

        public static double ComputeProjectedRadius(Size size, Vector direction)
        {
            double dx = size.Width / 2.0;
            double dy = size.Height / 2.0;
            return Math.Sqrt(
                Math.Pow(dx * Math.Abs(direction.X), 2) +
                Math.Pow(dy * Math.Abs(direction.Y), 2)
            );
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
