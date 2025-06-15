using System.Windows;

namespace BubbleControlls.Geometry
{
    public class EllipsePath
    {
        private readonly Point _center;
        private readonly double _a; // RadiusX
        private readonly double _b; // RadiusY
        private readonly double _rotationRad;
        private readonly List<(double angle, double arcLength)> _lookupTable;
        private readonly double _totalArcLength;

        public EllipsePath(Point center, double radiusX, double radiusY, double rotationDegrees, double resolution = 0.001)
        {
            _center = center;
            _a = radiusX;
            _b = radiusY;
            _rotationRad = rotationDegrees * Math.PI / 180.0;
            _lookupTable = new List<(double, double)>();

            double arc = 0.0;
            Point last = GetPointInternal(0);
            _lookupTable.Add((0.0, 0.0));

            for (double angle = resolution; angle <= 2 * Math.PI; angle += resolution)
            {
                Point current = GetPointInternal(angle);
                arc += Distance(last, current);
                _lookupTable.Add((angle, arc));
                last = current;
            }

            _totalArcLength = arc;
        }

        public double TotalArcLength => _totalArcLength;

        public Point GetPoint(double angleRad)
        {
            Point p = GetPointInternal(angleRad);
            return RotateAroundCenter(p, _center, _rotationRad);
        }

        public Vector GetTangent(double angleRad)
        {
            // Tangente vor Rotation
            double dx = -_a * Math.Sin(angleRad);
            double dy =  _b * Math.Cos(angleRad);
            Vector tangent = new Vector(dx, dy);
            tangent = RotateVector(tangent, _rotationRad);
            tangent.Normalize();
            return tangent;
        }

        public double GetArcLength(double angleRad)
        {
            angleRad = NormalizeAngle(angleRad);
            for (int i = 1; i < _lookupTable.Count; i++)
            {
                if (_lookupTable[i].angle >= angleRad)
                {
                    double a0 = _lookupTable[i - 1].angle;
                    double a1 = _lookupTable[i].angle;
                    double l0 = _lookupTable[i - 1].arcLength;
                    double l1 = _lookupTable[i].arcLength;
                    double t = (angleRad - a0) / (a1 - a0);
                    return l0 + t * (l1 - l0);
                }
            }
            return _totalArcLength;
        }

        public double GetAngleAtArcLength(double arcLength)
        {
            arcLength %= _totalArcLength;
            if (arcLength < 0)
                arcLength += _totalArcLength;
            for (int i = 1; i < _lookupTable.Count; i++)
            {
                if (_lookupTable[i].arcLength >= arcLength)
                {
                    double l0 = _lookupTable[i - 1].arcLength;
                    double l1 = _lookupTable[i].arcLength;
                    double a0 = _lookupTable[i - 1].angle;
                    double a1 = _lookupTable[i].angle;
                    double t = (arcLength - l0) / (l1 - l0);
                    return a0 + t * (a1 - a0);
                }
            }
            return _lookupTable[^1].angle;
        }
        public double GetArcLengthBetween(double startRad, double endRad)
        {
            if (endRad < startRad)
                endRad += 2 * Math.PI;
            double arcLength = 0;
            int steps = 100; // Feinheit
            double angleStep = (endRad - startRad) / steps;

            for (int i = 0; i < steps; i++)
            {
                double a1 = startRad + i * angleStep;
                double a2 = startRad + (i + 1) * angleStep;

                Point p1 = GetPoint(a1);
                Point p2 = GetPoint(a2);

                arcLength += (p2 - p1).Length;
            }

            return arcLength;
        }
        // --- Hilfsmethoden ---

        private Point GetPointInternal(double angleRad)
        {
            double x = _center.X + _a * Math.Cos(angleRad);
            double y = _center.Y + _b * Math.Sin(angleRad);
            return new Point(x, y);
        }

        private static double Distance(Point a, Point b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static Point RotateAroundCenter(Point p, Point center, double angleRad)
        {
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);
            double dx = p.X - center.X;
            double dy = p.Y - center.Y;
            double rx = dx * cos - dy * sin;
            double ry = dx * sin + dy * cos;
            return new Point(center.X + rx, center.Y + ry);
        }

        private static Vector RotateVector(Vector v, double angleRad)
        {
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);
            return new Vector(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
        }

        private static double NormalizeAngle(double angleRad)
        {
            while (angleRad < 0) angleRad += 2 * Math.PI;
            while (angleRad >= 2 * Math.PI) angleRad -= 2 * Math.PI;
            return angleRad;
        }
    }
}
