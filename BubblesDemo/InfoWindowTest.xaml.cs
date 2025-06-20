using System.Windows;
using System.Windows.Input;
using BubbleControlls.ControlViews;

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
            infoBox.ShowInTaskbar = false;
            infoBox.Topmost = true;
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
            GetInfo();

            infoBox.Height = 100;
            infoBox.Width = 350;

            // Bildschirmposition der Maus holen
            Point mousePos = PointToScreen(Mouse.GetPosition(this));
            infoBox.Left = mousePos.X + 5;  // leichter Versatz
            infoBox.Top = mousePos.Y + 5;

            infoBox.Show();
        }

        private void GetInfo()
        {
            string text = "Eine Adaption der klassischen chinesischen Geschichte „Die Reise nach Westen“, die die Abenteuer des Affenkönigs Sun Wukong erzählt. Mit beeindruckenden Spezialeffekten und einer epischen Handlung bietet der Film ein visuelles Spektakel.";
            infoBox.DisplayText = text;
        }
    }
}
