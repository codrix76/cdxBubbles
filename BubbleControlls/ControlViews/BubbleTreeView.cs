using BubbleControlls.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BubbleControlls.ControlViews
{
    public class BubbleTreeView : Canvas
    {
        public event Action<BubbleTreeViewItem> NodeClick;
        public event Action<BubbleTreeViewItem> NodeRightClick;
        public event Action<BubbleTreeViewItem> NodeExpanded;
        public event Action<BubbleTreeViewItem> NodeCollapsed;
        public event Action<BubbleTreeView> SelectionChanged;

        #region Variablen
        private BubbleTreeViewItem _root = new BubbleTreeViewItem(0, "Root");
        private BubbleVisualTheme _theme = BubbleVisualThemes.Standard();
        BubbleSwitch? rootNode = null;
        private Point _startPos = new Point(10,10);
        private Point _lastPos = new Point(10,10);
        private double _horizontalStep = 40.0;
        private double _verticalStep = 4.0;
        private double _bubbleSwitchHeight = 25;
        private bool _isMultiSelect = false;
        private List<List<BubbleTreeViewItem>> _pathList = new List<List<BubbleTreeViewItem>>();
        private List<List<BubbleTreeViewItem>> _SubList = new List<List<BubbleTreeViewItem>>();
        private List<BubbleTreeViewItem> _selectionList = new List<BubbleTreeViewItem>();

        private List<BubbleSwitch> _currentList = new List<BubbleSwitch>();
        private List<BubbleSwitch> _keepList = new List<BubbleSwitch>();
        #endregion
        public BubbleTreeView()
        {
            RebuildTree();
            _root.ItemAdded += ItemAdded;
            _root.ItemRemoved += ItemRemoved;
        }

        #region Properties
        public BubbleTreeViewItem Root
        {
            get { return _root; }
            set
            {
                _root = value;
                RebuildTree(true);
            }
        }
        public double SwitchFontSize { get => _theme.FontSize; set { _theme.FontSize = value; } }
        public double SwitchHeight { get => _bubbleSwitchHeight; set => _bubbleSwitchHeight = value; }
        public double Indentation { get => _horizontalStep; set => _horizontalStep = value; }
        public double VerticalSpacing { get => _verticalStep; set => _verticalStep = value; }
        public bool IsMultiSelect { get => _isMultiSelect; set => _isMultiSelect = value; }
        public List<BubbleTreeViewItem> SelectionList { get => _selectionList; set => _selectionList = value; }

        #endregion

        #region Methods
        public void ApplyTheme(BubbleVisualTheme theme)
        {
            _theme = theme ?? BubbleVisualThemes.Standard();
            InvalidateVisual();
            RebuildTree();
        }
        private void RebuildTree(bool newData = false)
        {
            //this.Children.Clear();
            if (rootNode == null || newData) CreateRoot();
            CreateParents();
            InvalidateVisual();
        }
        private void CreateRoot()
        {
            this.Children.Clear();
            BubbleSwitch root = new BubbleSwitch();
            root.IsSwitchable = _root.IsExpandeable;
            root.IsSwitched = _root.IsExpanded;
            root.ApplyTheme(_theme);
            root.Height = _bubbleSwitchHeight;
            root.SwitchLabel = _root.Label;
            root.Tag = new SwitchInfo() { ID = -1, ParentID = -1 };

            root.Toggled += SwitchToggle;
            Canvas.SetLeft(root, _startPos.X);
            Canvas.SetTop(root, _startPos.Y);
            this.Children.Add(root);
            rootNode = root;

            ClearLists();
        }
        private void ClearLists(int? parentindex = null)
        {
            if (parentindex == null)
            {
                _pathList.Clear();
                _SubList.Clear();
            }
            //foreach (var child in _root.Children)
            for (int i = 0;  i < _root.Children.Count; i++)
            {
                if (parentindex == null)
                {
                    _pathList.Add(new List<BubbleTreeViewItem>());
                    _SubList.Add(new List<BubbleTreeViewItem>());
                }
                else if (parentindex == i)
                {
                    _pathList[i] = new List<BubbleTreeViewItem>();
                    _SubList[i] = new List<BubbleTreeViewItem>();
                }
            }
        }
        private BubbleSwitch CreateNodeSwitch(BubbleTreeViewItem obj)
        {
            BubbleSwitch node = new BubbleSwitch();
            node.IsSwitchable = obj.IsExpandeable;
            //node.IsSwitched = obj.IsExpanded;
            node.ApplyTheme(_theme);
            if (obj.CustomColor != null) node.InnerBackground = obj.CustomColor;
            node.Height = _bubbleSwitchHeight;
            node.SwitchLabel = obj.Label;
            node.Tag = new SwitchInfo() { ID = obj.ID, ParentID = obj.Parent != null ? obj.Parent.ID : -1 };
            node.Toggled += SwitchToggle;
            if (obj.IsExpanded)
                node.SwitchON();
            else
                node.SwitchOFF();
            node.Clicked += Node_Clicked;
            node.RightClicked += Node_RightClicked;
            Canvas.SetLeft(node, _startPos.X);
            Canvas.SetTop(node, _startPos.Y);
            this.Children.Add(node);
            return node;
        }
        private void SwitchToggle(BubbleSwitch obj)
        {
            BubbleTreeViewItem? itm = _root.FindByID(((SwitchInfo)obj.Tag).ID);
            if (itm != null)
            {
                itm.IsExpanded = obj.IsSwitched;
                int lvl = BubbleTreeViewItem.GetLevel(itm);
                int parentIndex = GetParentIndex(itm);
                if (lvl>=2)
                {
                    int index = _pathList[parentIndex].IndexOf(itm);
                    if (index >= 0)
                    {
                        // Klicke auf älteren Pfadpunkt → alles danach löschen
                        _pathList[parentIndex] = _pathList[parentIndex].Take(index + 1).ToList();
                    }
                    else
                    {
                        // Neuen Pfadpunkt hinzufügen
                        _pathList[parentIndex].Add(itm);
                    }
                    if (_pathList[parentIndex].Count == 1)
                    {
                        if (!_pathList[parentIndex][0].IsExpanded)
                        {
                            ClearLists(parentIndex);
                        }
                    }
                }
            }

            RebuildTree();
            if (itm != null)
            {
                if (obj.IsSwitched) { NodeExpanded?.Invoke(itm); }
                else { NodeCollapsed?.Invoke(itm); }
            }
                
        }
        private void ItemAdded(BubbleTreeViewItem obj)
        {            
            ItemCountChanged(obj);
        }
        private void ItemRemoved(BubbleTreeViewItem obj)
        {
            ItemCountChanged(obj);
            BubbleSwitch? node = FindByTagID((int)obj.ID);
            if (node != null) this.Children.Remove(node);
        }
        private void ItemCountChanged(BubbleTreeViewItem obj)
        {
            BubbleSwitch? node = FindByTagID((int)obj.ID);
            if (node != null)
            {
                if (obj.Parent != null)
                {
                    BubbleSwitch? parent = FindByTagID(obj.Parent.ID);
                    if (parent == null) return;

                    if (obj.Parent.Children.Count > 0)
                    {
                        parent.IsSwitchable = true;
                    }
                    else
                    {
                        parent.IsSwitchable = false;
                    }
                }
            }
        }    
        private BubbleSwitch? FindByTagID(int id)
        {
            foreach (UIElement child in this.Children)
            {
                if (child is BubbleSwitch bs && ((SwitchInfo)bs.Tag).ID == id)
                {
                    return bs;
                }
            }
            return null;
        }
        private void CreateParents()
        {
            _currentList = this.Children
                .OfType<BubbleSwitch>()
                .OrderBy(s => Canvas.GetTop(s))
                .ToList();

            Canvas.SetLeft(rootNode, _startPos.X);
            Canvas.SetTop(rootNode, _startPos.Y);

            _keepList = new List<BubbleSwitch>() { rootNode };

            _lastPos = _startPos;
            _root.IsExpanded = rootNode.IsSwitched;
            if (_root.IsExpanded)
            {
                for (int i = 0; i < _root.Children.Count; i++)
                {
                    BubbleTreeViewItem itm = _root.Children[i];
                    // Paging the parents
                    //BubbleSwitch? node = _currentList.FirstOrDefault(sw => sw.Tag is SwitchInfo si && si.ID == itm.ID);
                    //if (node == null) node = FindByTagID(itm.ID);
                    //if (node == null) node = CreateNodeSwitch(itm);
                    BubbleSwitch? node = EnsureSwitch(itm);
                    if (!itm.IsExpanded)
                        node.SwitchOFF();
                    
                    Point newPos = new Point(_startPos.X + (_horizontalStep), _lastPos.Y + _bubbleSwitchHeight + _verticalStep);
                    Canvas.SetTop(node, newPos.Y);
                    Canvas.SetLeft(node, newPos.X);
                    _lastPos = newPos;
                    _keepList.Add(node);
                    if (itm.IsExpanded)
                    {
                        CreatePath(i);
                        if (_pathList[i].Count == 0)
                            CreateSub(i, itm);
                    }
                }
            }
            else
            {
                ClearLists();
            }
            var toRemove = _currentList.Where(sw => !_keepList.Contains(sw)).ToList();

            foreach (var sw in toRemove)
            {
                BubbleTreeViewItem? node = _root.FindByID(((SwitchInfo)sw.Tag).ID);
                if (node != null) node.IsExpanded = false;
                this.Children.Remove(sw);
            }
                
        }
        private void CreatePath(int parentIndex)
        {
            for (int i = 0; i < _pathList[parentIndex].Count; i++)
            {
                BubbleTreeViewItem itm = _pathList[parentIndex][i];
                // Paging the parents
                //BubbleSwitch? node = _currentList.FirstOrDefault(sw => sw.Tag is SwitchInfo si && si.ID == itm.ID);
                //if (node == null) node = FindByTagID(itm.ID);
                //if (node == null) node = CreateNodeSwitch(itm);
                BubbleSwitch? node = EnsureSwitch(itm);
                Point newPos = new Point(_startPos.X + (_horizontalStep * 2), _lastPos.Y + _bubbleSwitchHeight + _verticalStep);
                Canvas.SetTop(node, newPos.Y);
                Canvas.SetLeft(node, newPos.X);
                _lastPos = newPos;
                _keepList.Add(node);
                if (itm.IsExpanded && i == _pathList[parentIndex].Count-1)
                {
                    CreateSub(parentIndex, itm);
                    break;
                }
            }
        }
        private void CreateSub(int parentIndex, BubbleTreeViewItem itm)
        {
            int addition = _pathList[parentIndex].Count == 0 ? 2 : 3;

            if (addition == 3)
            {
                _SubList[parentIndex].Clear();
            }
            _SubList[parentIndex].Clear();
            for (int i = 0; i < itm.Children.Count; i++)
            {
                BubbleTreeViewItem item = itm.Children[i];
                // Paging the parents
                //BubbleSwitch? node = _currentList.FirstOrDefault(sw => sw.Tag is SwitchInfo si && si.ID == item.ID);
                //if (node == null) node = FindByTagID(item.ID);
                //if (node == null) node = CreateNodeSwitch(item);
                BubbleSwitch? node = EnsureSwitch(item);
                Point newPos = new Point(_startPos.X + (_horizontalStep * addition), _lastPos.Y + _bubbleSwitchHeight + _verticalStep);
                Canvas.SetTop(node, newPos.Y);
                Canvas.SetLeft(node, newPos.X);
                _lastPos = newPos;
                _keepList.Add(node);
                _SubList[parentIndex].Add(item);
            }
        }
        public int GetParentIndex(BubbleTreeViewItem item)
        {
            if (_root == null || item == null)
                return -1;

            var current = item;

            // Hochlaufen bis direkt unter der Root
            while (current != null && current.Parent != _root)
            {
                current = current.Parent;
            }

            if (current?.Parent == _root)
            {
                return _root.Children.IndexOf(current);
            }

            return -1; // kein gültiger Root-Child-Vorfahre
        }
        private BubbleSwitch EnsureSwitch(BubbleTreeViewItem item)
        {
            return _currentList.FirstOrDefault(sw => ((SwitchInfo)sw.Tag).ID == item.ID)
                   ?? FindByTagID(item.ID)
                   ?? CreateNodeSwitch(item);
        }
        private Point GetRightCenter(UIElement el) =>
            new(Canvas.GetLeft(el) + _horizontalStep / 2, Canvas.GetTop(el) + el.DesiredSize.Height / 2);
        private Point GetLeftCenter(UIElement el) =>
            new(Canvas.GetLeft(el), Canvas.GetTop(el) + el.DesiredSize.Height / 2);
        private void DrawLLine(DrawingContext dc, Point from, Point to, Pen pen, bool horizontalFirst = true)
        {
            if (horizontalFirst)
            {
                // Horizontaler Knick, dann runter
                var mid = new Point(to.X, from.Y);
                dc.DrawLine(pen, from, mid);
                dc.DrawLine(pen, mid, to);
            }
            else
            {
                // Vertikaler Knick, dann rüber
                var mid = new Point(from.X, to.Y);
                dc.DrawLine(pen, from, mid);
                dc.DrawLine(pen, mid, to);
            }
        }
        #endregion

        #region Events
        private void Node_Clicked(BubbleSwitch obj)
        {
            BubbleTreeViewItem? node = _root.FindByID(((SwitchInfo)obj.Tag).ID);
            if (node == null) return;

            bool isCtrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (_isMultiSelect)
            {
                obj.IsSelectable = true;

                if (isCtrl)
                {
                    if (_selectionList.Contains(node))
                    {
                        _selectionList.Remove(node);
                        obj.IsSelected = false;
                    }
                    else
                    {
                        _selectionList.Add(node);
                        obj.IsSelected = true;
                    }
                }
                else
                {
                    // Ohne STRG → neue Einzel-Selektion
                    foreach (var sw in _keepList)
                    {
                        sw.IsSelected = false;
                    }

                    _selectionList.Clear();
                    _selectionList.Add(node);
                    obj.IsSelected = true;
                }

                // Auswahl geändert
                SelectionChanged?.Invoke(this);
            }
            else
            {
                foreach (var sw in _keepList)
                {
                    sw.IsSelectable = true;
                    sw.IsSelected = false;
                }

                _selectionList.Clear();
                _selectionList.Add(node);
                obj.IsSelected = true;

                // Kein SelectionChanged nötig – Clicked deckt es ab
            }

            NodeClick?.Invoke(node);
        }
        private void Node_RightClicked(BubbleSwitch obj)
        {
            BubbleTreeViewItem? node = _root.FindByID(((SwitchInfo)obj.Tag).ID);
            if (node != null)
            {
                NodeRightClick?.Invoke(node);
            }
        }
        #endregion

        #region Overrides
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Pen pen = new Pen(Brushes.Gray, 1);
            // 1. Root → ParentNodes
            foreach (var child in _root.Children)
            {
                var parentNode = FindByTagID(child.ID);
                if (rootNode != null && parentNode != null)
                {
                    DrawLLine(dc,
                        GetRightCenter(rootNode),
                        GetLeftCenter(parentNode),
                        pen,
                        horizontalFirst: false);
                }
            }
            // 2. Parent → erster PfadNode
            for (int i = 0; i < _root.Children.Count; i++)
            {
                var parent = _root.Children[i];
                var parentNode = FindByTagID(parent.ID);

                if (_pathList.Count > i && _pathList[i].Count > 0)
                {
                    var firstPathNode = _pathList[i][0];
                    var pathSwitch = FindByTagID(firstPathNode.ID);
                    if (parentNode != null && pathSwitch != null)
                    {
                        DrawLLine(dc,
                            GetRightCenter(parentNode),
                            GetLeftCenter(pathSwitch),
                            pen,
                            horizontalFirst: true); // links + runter
                    }
                }
            }

            // 3. PathNode → PathNode
            for (int i = 0; i < _pathList.Count; i++)
            {
                var path = _pathList[i];
                for (int j = 1; j < path.Count; j++)
                {
                    var from = FindByTagID(path[j - 1].ID);
                    var to = FindByTagID(path[j].ID);
                    if (from != null && to != null)
                    {
                        dc.DrawLine(pen, GetRightCenter(from), GetRightCenter(to)); // direkte vertikale Linie
                    }
                }
            }

            // 4. Letzter PathNode → SubNodes
            for (int i = 0; i < _SubList.Count; i++)
            {
                var parent = _root.Children[i];
                var path = _pathList[i];
                var from = path.Count > 0 ? FindByTagID(path.Last().ID) : FindByTagID(parent.ID);

                foreach (var sub in _SubList[i])
                {
                    var subNode = FindByTagID(sub.ID);
                    if (from != null && subNode != null)
                    {
                        DrawLLine(dc,
                            GetRightCenter(from),
                            GetLeftCenter(subNode),
                            pen,
                            horizontalFirst: false); // rechts + runter
                    }
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double maxX = 0;
            double maxY = 0;

            foreach (UIElement child in Children)
            {
                if (child == null) continue;

                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                double left = Canvas.GetLeft(child);
                double top = Canvas.GetTop(child);

                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                maxX = Math.Max(maxX, left + child.DesiredSize.Width);
                maxY = Math.Max(maxY, top + child.DesiredSize.Height);
            }

            return new Size(maxX, maxY); // gewünschte Größe
        }
        #endregion
    }

    /// <summary>
    /// Zusätzliche Hilfsklasse für die Verwendung in der Tag-Eigenschaft der BubbleSwitches
    /// </summary>
    public class SwitchInfo()
    {
        public int ID { get; set; } = -1;
        public int ParentID { get; set; } = -1;

    }
}
