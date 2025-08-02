using System;
using System.Windows;

namespace BubbleControlls.Helpers
{
    public enum BubbleContextPlacement
    {
        BottomRight,
        BottomLeft,
        TopRight,
        TopLeft,
        Centered
    }

    public class BubbleContextPlacementInfo
    {
        public BubbleContextPlacement Placement { get; set; }
        public Point MenuTopLeft { get; set; }
        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
    }

    public static class BubbleContextPlacer
    {
        public static BubbleContextPlacementInfo GetBestPlacement(Point mousePos, Size menuSize, Size screenSize, double margin = 20)
        {
            bool fitsRight = (mousePos.X + menuSize.Width + margin) <= screenSize.Width;
            bool fitsLeft = (mousePos.X - menuSize.Width - margin) >= 0;
            bool fitsBottom = (mousePos.Y + menuSize.Height + margin) <= screenSize.Height;
            bool fitsTop = (mousePos.Y - menuSize.Height - margin) >= 0;

            if (fitsRight && fitsBottom)
            {
                return new BubbleContextPlacementInfo
                {
                    Placement = BubbleContextPlacement.BottomRight,
                    MenuTopLeft = new Point(mousePos.X + margin, mousePos.Y + margin),
                    StartAngle = 0,
                    EndAngle = 180
                };
            }

            if (fitsLeft && fitsBottom)
            {
                return new BubbleContextPlacementInfo
                {
                    Placement = BubbleContextPlacement.BottomLeft,
                    MenuTopLeft = new Point(mousePos.X - menuSize.Width - margin, mousePos.Y + margin),
                    StartAngle = 180,
                    EndAngle = 360
                };
            }

            if (fitsRight && fitsTop)
            {
                return new BubbleContextPlacementInfo
                {
                    Placement = BubbleContextPlacement.TopRight,
                    MenuTopLeft = new Point(mousePos.X + margin, mousePos.Y - menuSize.Height - margin),
                    StartAngle = 270,
                    EndAngle = 90
                };
            }

            if (fitsLeft && fitsTop)
            {
                return new BubbleContextPlacementInfo
                {
                    Placement = BubbleContextPlacement.TopLeft,
                    MenuTopLeft = new Point(mousePos.X - menuSize.Width - margin, mousePos.Y - menuSize.Height - margin),
                    StartAngle = 180,
                    EndAngle = 0
                };
            }

            // Notfall: zentriert
            return new BubbleContextPlacementInfo
            {
                Placement = BubbleContextPlacement.Centered,
                MenuTopLeft = new Point(
                    (screenSize.Width - menuSize.Width) / 2,
                    (screenSize.Height - menuSize.Height) / 2
                ),
                StartAngle = 0,
                EndAngle = 360
            };
        }
    }
}
