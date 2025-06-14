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
            myBubbleRing.RingRotation = 0.0;
            myBubbleRing.StartAngle = 30.0;
            myBubbleRing.EndAngle = 120.0;
            myBubbleRing.ElementDistance = 10.0;
            myBubbleRing.TrackAlignment = BubbleTrackAlignment.Start;
            myBubbleRing.AddElements(GetElements());
        }

        private List<UIElement> GetElements()
        {
            List<UIElement> elements = new List<UIElement>();
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
            elements.Add(bubble);
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
            
            elements.Add(new Bubble { Height = 40,RenderStyle = BubbleRenderStyle.Style3D });
            elements.Add(new Bubble { Height = 40,RenderStyle = BubbleRenderStyle.Style3D });
            elements.Add(new Bubble { Height = 40,RenderStyle = BubbleRenderStyle.Style3D });
            elements.Add(new Bubble { Height = 40,RenderStyle = BubbleRenderStyle.Style3D });
            elements.Add(new Bubble { Height = 40,RenderStyle = BubbleRenderStyle.Style3D });
            
            elements.Add(bubbleEnd);
            return elements;
        }
    }
}
