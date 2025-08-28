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
    /// Interaktionslogik für TreeDemo.xaml
    /// </summary>
    public partial class TreeDemo : Window
    {
        private Random _random = new Random();
        public TreeDemo()
        {
            InitializeComponent();
            CreateTree();
        }

        public void CreateTree()
        {
            List<BubbleTreeViewItem> roots = new List<BubbleTreeViewItem>();
            roots.Add(GenerateNode("WS1", 6));
            roots.Add(GenerateNode("WS2", 6));
            roots.Add(GenerateNode("WS3", 6));
            
            BTree.Root = roots;

        }


        private BubbleTreeViewItem GenerateNode(string name, int depthLeft)
        {
            int _nodeID = 0;
            var node = new BubbleTreeViewItem(name + "-" + _nodeID, name);
            if (depthLeft > 0)
            {
                int childCount = _random.Next(1, 6); // 1–5 Kinder

                for (int i = 0; i < childCount; i++)
                {
                    _nodeID++;
                    var child = GenerateNode(name + " - " + _nodeID, depthLeft - 1);
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
    }
}