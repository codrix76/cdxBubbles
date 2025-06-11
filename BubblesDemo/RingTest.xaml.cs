using System.Windows;
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
        public RingTest()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            myBubbleRing.RingRotation = 45.0;
            myBubbleRing.StartAngle = 0.0;
            myBubbleRing.EndAngle = 90.0;
            myBubbleRing.ElementDistance = 50.0;
            var bubble = new Bubble
            {
                Height = 40,
                Text = "",
                ToolTipText = "",
                FontSizeValue = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                BorderDistance = 6
            };
            bubble.BackgroundBrush = new SolidColorBrush(Colors.Red);
            bubble.RenderStyle = BubbleRenderStyle.Style3D;
            
            var bubbleEnd = new Bubble
            {
                Height = 40,
                Text = "",
                ToolTipText = "",
                FontSizeValue = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                BorderDistance = 6
            };
            bubbleEnd.BackgroundBrush = new SolidColorBrush(Colors.Blue);
            bubbleEnd.RenderStyle = BubbleRenderStyle.Style3D;
            
            myBubbleRing.Children.Add(bubble);
            myBubbleRing.Children.Add(new Bubble { Height = 40 });
            myBubbleRing.Children.Add(new Bubble { Height = 40 });
            myBubbleRing.Children.Add(new Bubble { Height = 40 });
            myBubbleRing.Children.Add(new Bubble { Height = 40 });
            myBubbleRing.Children.Add(new Bubble { Height = 40 });
            myBubbleRing.Children.Add(new Bubble { Height = 40 });
            myBubbleRing.Children.Add(bubbleEnd);
            
        }
    }
}
