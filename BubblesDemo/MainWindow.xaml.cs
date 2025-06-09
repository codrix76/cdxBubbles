using BubbleControlls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BubblesDemo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            this.Loaded += (s, e) =>
            {
                bbMenu.BubbleMenuAlignment = BubbleMenuAlignmentType.TopLeftCorner;
                bbMenu.Background = Brushes.DarkSlateBlue;
                bbMenu.DistributionAlignment = DistributionAlignmentType.Center;
                bbMenu.MainMenu = BuildMenu();
            };
        }

        private BubbleMenuItem BuildMenu()
        {
            BubbleMenuItem mainMenu = new BubbleMenuItem()
            {
                Tooltip = "Hauptmenu",
                Text = "",
                IconPath = "pack://application:,,,/Assets/mainMenu.png"
            };
            BubbleMenuItem subItem = new BubbleMenuItem()
            {
                Tooltip = "Daten laden/konfigurieren",
                Text = "",
                IconPath = "pack://application:,,,/Assets/Data.png"
            };
            BubbleMenuItem subItem1 = new BubbleMenuItem()
            {
                Tooltip = "Data 01",
                Text = "",
                IconPath = "pack://application:,,,/Assets/chart01.png"
            };
            subItem.SubItems.Add(subItem1);
            mainMenu.SubItems.Add(subItem);
            subItem = new BubbleMenuItem()
            {
                Tooltip = "Analyse",
                Text = "",
                IconPath = "pack://application:,,,/Assets/analyse.ico"
            };
            subItem1 = new BubbleMenuItem()
            {
                Tooltip = "Analyse 02",
                Text = "",
                IconPath = "pack://application:,,,/Assets/analyse02.png"
            };
            subItem.SubItems.Add(subItem1);
            mainMenu.SubItems.Add(subItem);
            subItem1 = new BubbleMenuItem()
            {
                Tooltip = "Analyse 03",
                Text = "",
                IconPath = "pack://application:,,,/Assets/curve01.png"
            };
            BubbleMenuItem subItem2 = new BubbleMenuItem()
            {
                Tooltip = "Analyse 03.01",
                Text = "",
                IconPath = "pack://application:,,,/Assets/curve02.png"
            };
            subItem1.SubItems.Add(subItem2);
            subItem.SubItems.Add(subItem1);
            mainMenu.SubItems.Add(subItem);
            subItem = new BubbleMenuItem()
            {
                Tooltip = "Projekt schliessen",
                Text = "",
                IconPath = "pack://application:,,,/Assets/back_aarow.png"
            };
            mainMenu.SubItems.Add(subItem);
            subItem = new BubbleMenuItem()
            {
                Tooltip = "Einstellungen",
                Text = "",
                IconPath = "pack://application:,,,/Assets/Settings01.png"
            };
            mainMenu.SubItems.Add(subItem);
            subItem = new BubbleMenuItem()
            {
                Tooltip = "Beenden",
                Text = "",
                IconPath = "pack://application:,,,/Assets/exit.png"
            };
            mainMenu.SubItems.Add(subItem);
            return mainMenu;
        }
    }
}
