using BubbleControlls.ControlViews;
using BubbleControlls.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BubblesDemo
{
    /// <summary>
    /// Interaktionslogik für InfoWindowTest.xaml
    /// </summary>
    public partial class InfoWindowTest : Window
    {
        BubbleInfoBox infoBox = new BubbleInfoBox();
        private Random _random = new Random();
        private int _nodeID = 0;
        public InfoWindowTest()
        {
            InitializeComponent();
            this.Closed += InfoWindowTest_Closed;
            DemoBtn.MouseEnter += DemoBtn_MouseEnter;
            DemoBtn.MouseLeave += DemoBtn_MouseLeave;
            DemoBtn.MouseMove += DemoBtn_MouseMove;
            DemoBtn.Click += DemoBtn_Click;
            DemoColor.Click += DemoColor_Click;
            infoBox.ShowInTaskbar = false;
            infoBox.Topmost = true;
            Loaded += (_, _) => { SetDefaults(); };
            //bblTree.ApplyTheme(BubbleVisualThemes.Dark());
            bblTree.SwitchFontSize = 11;
            bblTree.SwitchHeight = 25;
            bblTree.Indentation = 20;
            bblTree.VerticalSpacing = 2;
            //bblTree.NodeClick += BblTree_NodeClick;
            //bblTree.NodeExpanded += BblTree_NodeExpanded;
            //bblTree.NodeCollapsed += BblTree_NodeCollapsed;
            bblTree.NodeRightClick += BblTree_NodeRightClick;
            bblTree.SelectionChanged += BblTree_SelectionChanged;
            bblTree.IsMultiSelect = true;
            BblSwitch.MouseRightButtonDown += BblSwitch_MouseRightButtonDown;
            CreateTree();

            
        }

        private void DemoColor_Click(object sender, RoutedEventArgs e)
        {
            var cp = new BubbleColorPickerWindow();
            cp.ShowDialog();

        }

        private void BblSwitch_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            List<BubbleMenuItem> menu = CreateContextMenu();
            Point screenPos = PointToScreen(e.GetPosition(this));
            BubbleContextWindow cm = new BubbleContextWindow();
            cm.ShowAt(screenPos, menu);
        }

        private void BblTree_SelectionChanged(BubbleTreeView obj)
        {
            foreach(var itm in obj.SelectionList)
            {
                Debug.WriteLine(itm.Label);
            }
        }

        #region ContextMenu
        private List<BubbleMenuItem> CreateContextMenu()
        {
            List<BubbleMenuItem> menu = new List<BubbleMenuItem>();
            BubbleMenuItem conMenu = new BubbleMenuItem()
            {
                Name = "ConMenu01",
                Tooltip = "Context 01",
                Text = "Context 01",
                IconPath = "pack://application:,,,/Assets/Destiny.png",
                OnClick = (item) => {
                    Debug.WriteLine($"Aktion ausgelöst von: {item.Text}");
                }
            };
            menu.Add(conMenu);
            conMenu = new BubbleMenuItem()
            {
                Name = "ConMenu02",
                Tooltip = "Context 02",
                Text = "Context 02",
                IconPath = "pack://application:,,,/Assets/Data.png",
                OnClick = (item) => {
                    Debug.WriteLine($"Aktion ausgelöst von: {item.Text}");
                }
            };
            menu.Add(conMenu);
            conMenu = new BubbleMenuItem()
            {
                Name = "ConMenu03",
                Tooltip = "Context 03",
                Text = "Context 03",
                IconPath = "pack://application:,,,/Assets/chart01.png",
                OnClick = (item) => {
                    Debug.WriteLine($"Aktion ausgelöst von: {item.Text}");
                }
            };
            menu.Add(conMenu);
            conMenu = new BubbleMenuItem()
            {
                Name = "ConMenu02",
                Tooltip = "Context 02",
                Text = "Context 04",
                IconPath = "pack://application:,,,/Assets/Data.png",
                OnClick = (item) => {
                    Debug.WriteLine($"Aktion ausgelöst von: {item.Text}");
                }
            };
            menu.Add(conMenu);
            conMenu = new BubbleMenuItem()
            {
                Name = "ConMenu02",
                Tooltip = "Context 02",
                Text = "Context 05",
                IconPath = "pack://application:,,,/Assets/Data.png",
                OnClick = (item) => {
                    Debug.WriteLine($"Aktion ausgelöst von: {item.Text}");
                }
            };
            menu.Add(conMenu);
            conMenu = new BubbleMenuItem()
            {
                Name = "ConMenu02",
                Tooltip = "Context 02",
                Text = "Context 06",
                IconPath = "pack://application:,,,/Assets/Data.png",
                OnClick = (item) => {
                    Debug.WriteLine($"Aktion ausgelöst von: {item.Text}");
                }
            };
            menu.Add(conMenu);
            conMenu = new BubbleMenuItem()
            {
                Name = "ConMenu02",
                Tooltip = "Context 02",
                Text = "Context 07",
                IconPath = "pack://application:,,,/Assets/Data.png",
                OnClick = (item) => {
                    Debug.WriteLine($"Aktion ausgelöst von: {item.Text}");
                }
            };
            menu.Add(conMenu);
            conMenu = new BubbleMenuItem()
            {
                Name = "ConMenu02",
                Tooltip = "Context 02",
                Text = "Context 08",
                IconPath = "pack://application:,,,/Assets/Data.png",
                OnClick = (item) =>
                {
                    Debug.WriteLine($"Aktion ausgelöst von: {item.Text}");
                }
            };
            menu.Add(conMenu);
            return menu;
        }
        #endregion
        #region BubbleTree

        private void BblTree_NodeRightClick(BubbleTreeViewItem obj, MouseButtonEventArgs e)
        {
            BubbleMsgBox.Show(
            $"Objekt: {obj.Label}",
            "Right-Click",
            MessageBoxButton.OK
            );
            BubbleTreeViewItem newItem = new BubbleTreeViewItem(9999, "new Node");
            bblTree.AddChildTo(obj, newItem);
        }

        private void BblTree_NodeCollapsed(BubbleTreeViewItem obj)
        {
            BubbleMsgBox.Show(
            $"Objekt: {obj.Label}",
            "Node-Collapsed",
            MessageBoxButton.OK
            );
        }

        private void BblTree_NodeExpanded(BubbleTreeViewItem obj)
        {
            BubbleMsgBox.Show(
            $"Objekt: {obj.Label}",
            "Node-Expanded",
            MessageBoxButton.OK
            );
        }

        private void BblTree_NodeClick(BubbleTreeViewItem obj)
        {
            BubbleMsgBox.Show(
            $"Objekt: {obj.Label}",
            "Node-Click",
            MessageBoxButton.OK
            );
        }
        public void CreateTree()
        {
            BubbleTreeViewItem root = new BubbleTreeViewItem(-1, "Root");
            //BubbleTreeViewItem item1 = new BubbleTreeViewItem(1,"Parent 01");
            //item1.Add(new BubbleTreeViewItem(3, "Child 01.01"));
            //item1.Add(new BubbleTreeViewItem(4, "Child 01.02"));
            //BubbleTreeViewItem item2 = new BubbleTreeViewItem(2,"Parent 02");
            //item2.Add(new BubbleTreeViewItem(5, "Child 02.01"));
            //item2.Add(new BubbleTreeViewItem(6, "Child 02.02"));
            //root.Add(item1);
            //root.Add(item2);
            root = GenerateNode("root", 6);
            root.ID = -1;
            bblTree.Root = root;

        }
        public void CreateDynamicTestTree(int deep = 2)
        {
            int idCounter = 1;

            BubbleTreeViewItem root = new BubbleTreeViewItem(-1, "Root");

            for (int p = 1; p <= 2; p++) // Zwei Parents
            {
                var parent = new BubbleTreeViewItem(idCounter++, $"Parent {p:D2}");

                for (int c = 1; c <= 3; c++) // Erste Ebene Children
                {
                    var child = new BubbleTreeViewItem(idCounter++, $"Child {p:D2}.{c:D2}");

                    for (int gc = 1; gc <= 3; gc++) // Zweite Ebene (Grandchildren)
                    {
                        var grandChild = new BubbleTreeViewItem(idCounter++, $"Child {p:D2}.{c:D2}.{gc:D2}");
                        for (int ggc = 1; gc <= 3; gc++) // Zweite Ebene (Grandchildren)
                        {
                            var grandGChild = new BubbleTreeViewItem(idCounter++, $"Child {p:D2}.{c:D2}.{gc:D2}.{ggc:D2}");
                            grandChild.Add(grandGChild);
                        }
                        child.Add(grandChild);
                    }

                    parent.Add(child);
                }

                root.Add(parent);
            }

            bblTree.Root = root;
        }

        private BubbleTreeViewItem GenerateNode(string name, int depthLeft)
        {
            var node = new BubbleTreeViewItem(_nodeID, name);
            int p = _nodeID;
            if (depthLeft > 0)
            {
                int childCount = _random.Next(1, 6); // 1–5 Kinder

                for (int i = 0; i < childCount; i++)
                {
                    _nodeID++;
                    var child = GenerateNode($"Node: {_nodeID}, P:{p}", depthLeft - 1);
                    //child.CustomColor = new SolidColorBrush(HeatValueToColor(DoubleToByte(_random.NextDouble() * 100.0)));
                    node.Add(child);
                }
            }
            else
            {
                //node.CustomColor = new SolidColorBrush(HeatValueToColor(DoubleToByte(_random.NextDouble() * 100.0)));
            }
            return node;
        }

        public Color HeatValueToColor(byte value)
        {
            // Grün (0) → Gelb (128) → Rot (255)
            if (value < 128)
            {
                // Grün nach Gelb
                byte r = (byte)(value * 2);
                byte g = 255;
                return Color.FromRgb(r, g, 0);
            }
            else
            {
                // Gelb nach Rot
                byte r = 255;
                byte g = (byte)(255 - (value - 128) * 2);
                return Color.FromRgb(r, g, 0);
            }
        }

        public byte DoubleToByte(double value)
        {
            // Clamp auf gültigen Bereich
            value = Math.Max(0.0, Math.Min(100.0, value));

            // Skalierung: 0.0 → 0, 100.0 → 255
            return (byte)Math.Round(value / 100.0 * 255.0);
        }
        #endregion

        #region BubbleSwitch
        private void SetDefaults()
        {
            BblSwitch.SwitchLabel = "test";
            BblSwitch.Height = 26;
            BblSwitch.IsSwitchable = true;
            BblSwitch.IsSelectable = true;
            //BblSwitch.OuterBorderThickness = new Thickness(2);
            //BblSwitch.OuterBorderBrush = Brushes.Blue;
            //BblSwitch.InnerBorderBrush = Brushes.Red;
            //BblSwitch.InnerBackground = Brushes.Blue;
            //BblSwitch.InvalidateVisual();
            BblSwitch.ApplyTheme(BubbleVisualThemes.Standard());
        }
        #endregion

        #region InfoBox
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

        #endregion

    }
}
