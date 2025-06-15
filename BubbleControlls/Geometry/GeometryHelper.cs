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
    
}