using BubbleControlls.ControlViews;
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
                BubbleMenu.BubbleMenuAlignment = BubbleMenuAlignmentType.LeftEdge;
                BubbleMenu.Background = Brushes.DarkSlateBlue;
                BubbleMenu.DistributionAlignment = DistributionAlignmentType.Center;
                BubbleMenu.BubbleMenuBigSize = 80.0;
                BubbleMenu.BubbleMenuSmallSize = 45.0;
                BubbleMenu.MainMenu = BuildMenu();
            };
        }

        private BubbleMenuItem BuildMenu()
        {
            BubbleMenuItem mainMenu = new BubbleMenuItem()
            {
                Name = "MainMenu",
                Tooltip = "Hauptmenu",
                Text = "",
                IconPath = "pack://application:,,,/Assets/Destiny.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            BubbleMenuItem subItem = new BubbleMenuItem()
            {
                Name = "DataMenu",
                Tooltip = "Daten laden/konfigurieren",
                Text = "",
                IconPath = "pack://application:,,,/Assets/Data.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            BubbleMenuItem subItem1 = new BubbleMenuItem()
            {
                Name = "DataMenu01",
                Tooltip = "Data 01",
                Text = "",
                IconPath = "pack://application:,,,/Assets/chart01.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            subItem.AddSubItem(subItem1);
            mainMenu.AddSubItem(subItem);
            subItem = new BubbleMenuItem()
            {
                Name = "AnalyseMenu",
                Tooltip = "Analyse",
                Text = "",
                IconPath = "pack://application:,,,/Assets/analyse.ico",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            subItem1 = new BubbleMenuItem()
            {
                Name = "AnalyseMenu02",
                Tooltip = "Analyse 02",
                Text = "",
                IconPath = "pack://application:,,,/Assets/analyse02.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            subItem.AddSubItem(subItem1);
            subItem1 = new BubbleMenuItem()
            {
                Name = "AnalyseMenu03",
                Tooltip = "Analyse 03",
                Text = "",
                IconPath = "pack://application:,,,/Assets/curve01.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            BubbleMenuItem subItem2 = new BubbleMenuItem()
            {
                Name = "AnalyseMenu031",
                Tooltip = "Analyse 03.01",
                Text = "",
                IconPath = "pack://application:,,,/Assets/curve02.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            subItem1.AddSubItem(subItem2);
            subItem.AddSubItem(subItem1);
            mainMenu.AddSubItem(subItem);
            subItem = new BubbleMenuItem()
            {
                Name = "CloseProjectMenu",
                Tooltip = "Projekt schliessen",
                Text = "",
                IconPath = "pack://application:,,,/Assets/back_arrow.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            mainMenu.AddSubItem(subItem);
            subItem = new BubbleMenuItem()
            {
                Name = "SettingsMenu",
                Tooltip = "Einstellungen",
                Text = "",
                IconPath = "pack://application:,,,/Assets/Settings01.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            mainMenu.AddSubItem(subItem);
            subItem = new BubbleMenuItem()
            {
                Name = "CloseMenu",
                Tooltip = "Beenden",
                Text = "",
                IconPath = "pack://application:,,,/Assets/exit.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            mainMenu.AddSubItem(subItem);
            return mainMenu;
        }
    }
}
