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

        public IEnumerable<BubblePlacement> PlaceBubbles(IEnumerable<Size> sizes, 
            double startAngleRad, double endAngleRad, double scrollOffset,
            bool isCentered)
        {
            var sizeList = sizes.ToList();
            if (sizeList.Count == 0)
                yield break;

            if (!isCentered)
            {
                foreach (var p in PlaceForward(sizeList, startAngleRad, scrollOffset))
                    yield return p;
            }
            else
            {
                foreach (var p in PlaceCentered(sizeList, startAngleRad, endAngleRad, scrollOffset))
                    yield return p;
            }
        }

        private IEnumerable<BubblePlacement> PlaceForward(IList<Size> sizes, double startAngleRad, 
            double scrollOffset)
        {
            double currentArc = _path.GetArcLength(startAngleRad  - scrollOffset);
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
        
        private IEnumerable<BubblePlacement> PlaceCentered(IList<Size> sizes, double startAngleRad, double endAngleRad,
            double scrollOffset)
        {

            double totalLength = 0;
            foreach (var size in sizes)
            {
                Vector tangent = new Vector(1, 0); // Approximation, da wir keinen realen Pfadpunkt haben
                double r = ComputeProjectedRadius(size, tangent);
                totalLength += 2 * r + (_spacing);
            }
            double visibleLength = _path.GetArcLengthBetween(startAngleRad, endAngleRad);
            if (totalLength > visibleLength)
            {
                return PlaceForward(sizes, startAngleRad, scrollOffset);
            }

            if (endAngleRad < startAngleRad)
                endAngleRad += 2 * Math.PI;
            double centerAngle = (startAngleRad + endAngleRad) / 2.0;
        
            var placements = PlaceForward(sizes, centerAngle, scrollOffset).ToList();
            double centerOffset = (ComputeTotalArcLength(placements) / 2.0);

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
                total += 2 * r + (_spacing);
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
