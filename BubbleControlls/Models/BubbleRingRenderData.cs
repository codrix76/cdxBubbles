using System.Windows;
using System.Windows.Media;
using BubbleControlls.Geometry;

namespace BubbleControlls.Models;

public class BubbleRingRenderData
{
    public bool initialized {get; set;} = false;
    public Point Center { get; set; }
    public double RadiusX { get; set; }
    public double RadiusY { get; set; }
    public double PathWidth { get; set; }
    public double StartAngleDeg { get; set; }
    public double EndAngleDeg { get; set; }
    public double RotationDeg { get; set; }
    public Brush Fill { get; set; } = new SolidColorBrush();
    public Brush Border { get; set; } = new SolidColorBrush();
    public int BorderOpacity { get; set; }
    public int FillOpacity { get; set; }
    public int RingOpacity { get; set; }
    public int BorderThickness { get; set; }
    public double StartAngleRad  { get; set; }
    public double EndAngleRad  { get; set; }
    public double RingRotationRad  { get; set; }
    public List<BubblePlacement> Placements { get; set; } = new();
}