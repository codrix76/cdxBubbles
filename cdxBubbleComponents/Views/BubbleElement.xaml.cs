using cdxBubbleComponents.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cdxBubbleComponents.Views
{

    public partial class BubbleElement : UserControl
    {
        public BubbleElement()
        {
            InitializeComponent();
            this.DataContext = new BubbleElementModel();
        }
    }
}
