using BubbleControlls.ControlViews;
using BubbleControlls.Models;
using System.Windows;
using System.Windows.Media;

namespace BubblesDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Console.WriteLine("Hallo Bubble");
            InitializeComponent();
            //testBubble.OuterBorderBrush = new SolidColorBrush(Colors.LightBlue);
            //testBubble.BackgroundBrush = new SolidColorBrush(Colors.LightGreen);
            //testBubble.MouseLeftButtonDown += (s, e) => { MessageBox.Show("Klick!"); };
            //testBubble.MouseEnter += (s, e) => MessageBox.Show("Hover!");
            //testBubble.GotFocus += (s, e) => MessageBox.Show("Fokus!");
            testBubble.ToolTipText = "Startet die Analyse für diesen Knoten";
            testBubble.ApplyTheme(BubbleVisualThemes.NeonEdge());

            testBubble.RenderStyle = BubbleRenderStyle.Style3D;
        }

    }
}