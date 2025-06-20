using BubbleControlls.Models;
using System.Windows;
using System.Windows.Media;

namespace BubbleControlls.ControlViews
{
    /// <summary>
    /// Interaktionslogik für InfoBox.xaml
    /// </summary>
    public partial class BubbleInfoBox : Window
    {
        public BubbleInfoBox()
        {
            InitializeComponent();

        }

        public void ApplyTheme(BubbleVisualTheme theme)
        {
            InfoContent.ApplyTheme(theme);
        }

        public string DisplayText
        {
            get => InfoContent.DisplayText;
            set => InfoContent.DisplayText = value;
        }
    }
}
