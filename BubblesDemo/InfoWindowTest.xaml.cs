using System.Windows;
using System.Windows.Input;
using BubbleControlls.ControlViews;
using BubbleControlls.Models;

namespace BubblesDemo
{
    /// <summary>
    /// Interaktionslogik für InfoWindowTest.xaml
    /// </summary>
    public partial class InfoWindowTest : Window
    {
        BubbleInfoBox infoBox = new BubbleInfoBox();
        public InfoWindowTest()
        {
            InitializeComponent();
            this.Closed += InfoWindowTest_Closed;
            DemoBtn.MouseEnter += DemoBtn_MouseEnter;
            DemoBtn.MouseLeave += DemoBtn_MouseLeave;
            DemoBtn.MouseMove += DemoBtn_MouseMove;
            DemoBtn.Click += DemoBtn_Click;
            infoBox.ShowInTaskbar = false;
            infoBox.Topmost = true;
        }

        private void DemoBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = BubbleMsgBox.Show(
            "Möchten Sie das Projekt wirklich schließen?\nEine Adaption der klassischen chinesischen Geschichte „Die Reise nach Westen“,\ndie die Abenteuer des Affenkönigs Sun Wukong erzählt. Mit beeindruckenden Spezialeffekten und einer epischen Handlung bietet der Film ein visuelles Spektakel.",
            "Projekt schließen",
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Warning,null,
            BubbleVisualThemes.Dark()
            );

            if (result == MessageBoxResult.Yes)
            {
                BubbleMsgBox.Show("yes");
            }
            else if (result == MessageBoxResult.No)
            {
                BubbleMsgBox.Show("No", "will nicht");
            }
            else
            {
                // Abbrechen oder Fenster geschlossen
            }
        }

        private void DemoBtn_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = PointToScreen(Mouse.GetPosition(this));
            infoBox.Left = mousePos.X + 5;  // leichter Versatz
            infoBox.Top = mousePos.Y + 5;
        }

        private void InfoWindowTest_Closed(object? sender, EventArgs e)
        {
            if (infoBox != null)
            {
                infoBox.Close();
                infoBox = null;
            }
        }
        private void DemoBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            infoBox.Hide();
        }

        private void DemoBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            infoBox.Height = 250;
            infoBox.Width = 350;
            infoBox.DisplayText = GetInfo();

            
            // Bildschirmposition der Maus holen
            Point mousePos = PointToScreen(Mouse.GetPosition(this));
            infoBox.Left = mousePos.X + 5;  // leichter Versatz
            infoBox.Top = mousePos.Y + 5;

            infoBox.Show();
        }

        private string GetInfo()
        {
            string text = "Eine Adaption der klassischen chinesischen Geschichte „Die Reise nach Westen“,\n" +
                "die die Abenteuer des Affenkönigs Sun Wukong erzählt. Mit beeindruckenden Spezialeffekten\n" +
                "und einer epischen Handlung bietet der Film ein visuelles Spektakel.";
            return text;
        }
    }
}
