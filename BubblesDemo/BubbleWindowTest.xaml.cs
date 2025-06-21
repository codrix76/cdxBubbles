using BubbleControlls.ControlViews;
using BubbleControlls.Models;

namespace BubblesDemo
{
    /// <summary>
    /// Interaktionslogik für BubbleWindowTest.xaml
    /// </summary>
    public partial class BubbleWindowTest : BubbleBaseWindow
    {
        public BubbleWindowTest()
        {
            InitializeComponent();
            //this.WindowTheme = BubbleVisualThemes.Dark();
            this.ApplyTheme(BubbleVisualThemes.HudBlue());
        }
    }
}
