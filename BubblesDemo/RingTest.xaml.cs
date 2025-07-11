﻿using System.Windows;
using System.Windows.Media;
using BubbleControlls.ControlViews;
using BubbleControlls.Models;

namespace BubblesDemo
{
    /// <summary>
    /// Interaktionslogik für RingTest.xaml
    /// </summary>
    public partial class RingTest : Window
    {
        private double bubbleSize = 55;
        public RingTest()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
            
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            double StartRad = 90.0;
            myBubbleRing.RadiusX = 250;
            myBubbleRing.RadiusY = 200;
            myBubbleRing.PathWidth = bubbleSize + 10;
            myBubbleRing.RingOpacity = 20;
            myBubbleRing.RingBorderThickness = 0;
            myBubbleRing.RingRotation = 0.0;
            myBubbleRing.StartAngle = 00;
            myBubbleRing.EndAngle = 269.0;
            myBubbleRing.ElementDistance = 10.0;
            myBubbleRing.ScrollArrowHeight = 10;
            myBubbleRing.IsCentered = true;
            myBubbleRing.IsInverted = true;
            myBubbleRing.AddElements(GetElements());
        }

        private List<UIElement> GetElements()
        {
            List<UIElement> elements = new List<UIElement>();
            var bubble = new Bubble
            {
                Height = bubbleSize,
                Width = bubbleSize,
                Text = "",
                ToolTipText = "",
                FontSizeValue = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                BorderDistance = 6
            };
            bubble.BackgroundBrush = new SolidColorBrush(Colors.Red);
            bubble.RenderStyle = BubbleRenderStyle.Style3D;
            elements.Add(bubble);
            var bubbleEnd = new Bubble
            {
                Height = bubbleSize,
                Width = bubbleSize,
                Text = "",
                ToolTipText = "",
                FontSizeValue = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                BorderDistance = 6
            };
            bubbleEnd.BackgroundBrush = new SolidColorBrush(Colors.Blue);
            bubbleEnd.RenderStyle = BubbleRenderStyle.Style3D;
            
            //elements.Add(new Bubble { Height = bubbleSize, Width = bubbleSize, RenderStyle = BubbleRenderStyle.Style3D });
            //elements.Add(new Bubble { Height = bubbleSize, Width = bubbleSize, RenderStyle = BubbleRenderStyle.Style3D });
            //elements.Add(new Bubble { Height = bubbleSize, Width = bubbleSize, RenderStyle = BubbleRenderStyle.Style3D });
            //elements.Add(new Bubble { Height = bubbleSize, Width = bubbleSize, RenderStyle = BubbleRenderStyle.Style3D });
            
            elements.Add(bubbleEnd);
            return elements;
        }
    }
}
