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
    public Brush Fill { get; set; }
    public Brush Border { get; set; }
    public int BorderOpacity { get; set; }
    public int FillOpacity { get; set; }
    public int BorderThickness { get; set; }
    public bool IsGlowActive { get; set; }
    public List<BubblePlacement> Placements { get; set; } = new();
}