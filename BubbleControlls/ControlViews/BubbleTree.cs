using BubbleControlls.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BubbleControlls.ControlViews;

public class BubbleTree : Canvas
{
    public event Action<BubbleTreeViewItem, MouseButtonEventArgs>? NodeClick;
    public event Action<BubbleTreeViewItem, MouseButtonEventArgs>? NodeRightClick;
    public event Action<BubbleTreeViewItem>? NodeExpanded;
    public event Action<BubbleTreeViewItem>? NodeCollapsed;
    public event Action<BubbleTree>? SelectionChanged;
    public event Action<BubbleTree>? RightClick;

    #region Variablen
    private ITreeViewDataSource _dataSource = new LocalTreeViewSource();
    private List<BubbleTreeViewItem> _rootList = new();
    private Dictionary<string, List<BubbleTreeViewItem>> _pathList = new();
    private Dictionary<string, List<BubbleTreeViewItem>> _subList = new();
    private List<BubbleTreeViewItem> _selectionList = new();
    private List<BubbleSwitch> _keepUIList = new();

    private BubbleVisualTheme _theme = BubbleVisualThemes.Standard();

    private double _horizontalStep = 40.0;
    private double _verticalStep = 4.0;
    private double _bubbleSwitchHeight = 25;
    private Point _startPos = new Point(10, 10);
    private Point _lastPos = new Point(10, 10);

    private readonly Dictionary<string, BubbleSwitch> _swByKey = new(StringComparer.Ordinal);

    #endregion

    #region Properties
    public bool MultiSelection { get; set; } = false;
    public List<BubbleTreeViewItem> Root
    {
        set { 
            _dataSource.SetRoot(value); Refresh();
        }
    }
    #endregion

    public BubbleTree()
    {
        this.MouseRightButtonDown += OnMouseRightButtonDown;
        Refresh();
    }

    #region Common Methods
    public void SetDataSource(ITreeViewDataSource dataSource)
    {
        _dataSource = dataSource ?? new LocalTreeViewSource();
        Refresh();
    }
    public void Refresh()
    {
        _rootList = _dataSource.GetRoot();
        _swByKey.Clear();
        Children.Clear();
        EnsureBucketsForRoots();
        UpdateUI();
    }
    private void EnsureBucketsForRoots()
    {
        foreach (var r in _rootList)
        {
            if (!_pathList.ContainsKey(r.Key))
                _pathList[r.Key] = new List<BubbleTreeViewItem>();
            if (!_subList.ContainsKey(r.Key))
                _subList[r.Key] = new List<BubbleTreeViewItem>();
        }
    }   
    public void ApplyTheme(BubbleVisualTheme theme)
    {
        _theme = theme ?? BubbleVisualThemes.Standard();
        InvalidateVisual();
    }
    private BubbleSwitch? FindSwitchByKey(string key) 
        => _swByKey.TryGetValue(key, out var sw) ? sw : null;

    private void NodeExpand(BubbleTreeViewItem node)
    {
        if (node.Parent == null)
        {
            _pathList[node.Key].Clear();
            _subList[node.Key] = new List<BubbleTreeViewItem>();
            var listp = _dataSource.GetChildren(node.Key);
            foreach (var child in listp)
            {
                _pathList[node.Key].Add(child);
            }
            UpdateUI();
            NodeExpanded?.Invoke(node);
            return;
        }

        string parentKey = _dataSource.GetRootParentKey(node.Key);
        _subList[parentKey] = new List<BubbleTreeViewItem>();
        var list = _dataSource.GetChildren(node.Key);
        foreach (var child in list)
        {
            _subList[parentKey].Add(child);
        }

        if (_pathList[parentKey].Contains(node))
        {
            int i = _pathList[parentKey].IndexOf(node);
            if (node.Parent?.Key == parentKey)
            {
                _pathList[parentKey].Clear();
                _pathList[parentKey].Add(node);
            }
            else
            {
                _pathList[parentKey].RemoveRange(i + 1, _pathList[parentKey].Count - i - 1);
            }
        }
        else
        {
            _pathList[parentKey].Add(node);
        }
        UpdateUI();
    }
    #endregion
    #region Drawing
    private static double GetTop(UIElement el) => GetTop(el as DependencyObject);
    private static double GetTop(DependencyObject? el)
    {
        var v = Canvas.GetTop(el as UIElement);
        return double.IsNaN(v) ? 0 : v;
    }

    private Point GetRightCenter(UIElement el)
    {
        double h = el.RenderSize.Height > 0 ? el.RenderSize.Height : _bubbleSwitchHeight;
        return new Point(GetLeft(el) + _horizontalStep / 2, GetTop(el) + h / 2);
    }

    private Point GetLeftCenter(UIElement el)
    {
        double h = el.RenderSize.Height > 0 ? el.RenderSize.Height : _bubbleSwitchHeight;
        return new Point(GetLeft(el), GetTop(el) + h / 2);
    }

    private static double GetLeft(UIElement el)
    {
        var v = Canvas.GetLeft(el);
        return double.IsNaN(v) ? 0 : v;
    }
    private static void DrawLLine(DrawingContext dc, Point from, Point to, Pen pen, bool horizontalFirst)
    {
        Point mid = horizontalFirst ? new Point(to.X, from.Y) : new Point(from.X, to.Y);
        dc.DrawLine(pen, from, mid);
        dc.DrawLine(pen, mid, to);
    }
    #endregion

    #region Node/Switch/Tree
    private void UpdateUI()
    {
        List<BubbleSwitch> _currentList = new();
        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i] is BubbleSwitch)
                _currentList.Add((BubbleSwitch)Children[i]);
        }
        _keepUIList.Clear();

        _lastPos = _startPos;
        // Roots
        for (int r = 0; r < _rootList.Count; r++)
        {
            _lastPos.X = _startPos.X;
            BubbleSwitch? rootSwitch = FindSwitchByKey(_rootList[r].Key);
            if (rootSwitch == null)
                rootSwitch = CreateNodeSwitch(_rootList[r]);
            _keepUIList.Add(rootSwitch);
            SetLeft(rootSwitch, _lastPos.X);
            SetTop(rootSwitch, _lastPos.Y);
            _lastPos.Y += _bubbleSwitchHeight + _verticalStep;

            // Pfadliste je Root
            _lastPos.X += _horizontalStep;
            for (int p = 0; p < _pathList[_rootList[r].Key].Count; p++)
            {
                var item = _pathList[_rootList[r].Key][p];
                BubbleSwitch? sw = FindSwitchByKey(item.Key);
                if (sw == null)
                    sw = CreateNodeSwitch(item);
                _keepUIList.Add(sw);
                SetLeft(sw, _lastPos.X);
                SetTop(sw, _lastPos.Y);
                sw.IsSwitched = false;
                _lastPos.Y += _bubbleSwitchHeight + _verticalStep;
            }   
            // Children 
            _lastPos.X += _horizontalStep;
            for (int s = 0; s < _subList[_rootList[r].Key].Count; s++)
            {
                var item = _subList[_rootList[r].Key][s];
                BubbleSwitch? sw = FindSwitchByKey(item.Key);
                if (sw == null)
                    sw = CreateNodeSwitch(item);
                _keepUIList.Add(sw);
                SetLeft(sw, _lastPos.X);
                SetTop(sw, _lastPos.Y);
                sw.IsSwitched = false;
                _lastPos.Y += _bubbleSwitchHeight + _verticalStep;
            }
        }

        // Überflüssige entfernen
        for (int i = _currentList.Count - 1; i >= 0; i--)
        {
            var item = _currentList[i];
            if (!_keepUIList.Contains(item))
            {
                _swByKey.Remove(item.Key);
                Children.Remove(item);   // aus Canvas entfernen
            }
        }
        InvalidateVisual();
    }
    private BubbleSwitch CreateNodeSwitch(BubbleTreeViewItem obj)
    {
        BubbleSwitch node = new BubbleSwitch(obj.Key)
        {
            Height = _bubbleSwitchHeight,
            SwitchLabel = obj.Label
        };
        if (_swByKey.ContainsKey(obj.Key))
            throw new InvalidOperationException($"Duplicate node key: {obj.Key}");
        _swByKey.Add(obj.Key, node);

        // Auto-Expandable per Live-Check
        node.IsSwitchable = _dataSource.GetChildren(obj.Key).Count > 0;
        node.IsSelectable = true;

        node.ApplyTheme(_theme);
        if (obj.CustomColor != null) node.InnerBackground = obj.CustomColor;

        //node.Toggled += SwitchToggle;
        if (obj.IsExpanded) node.SwitchON(); else node.SwitchOFF();
        node.Clicked += Node_Clicked;
        node.RightClicked += Node_RightClicked;
        node.Expanded += Node_Expanded;
        node.Colapsed += Node_Colapsed;

        Children.Add(node);
        return node;
    }

    #endregion

    #region Event Handling
    private void Node_Clicked(BubbleSwitch obj, MouseButtonEventArgs e)
    {
        BubbleTreeViewItem? node = _dataSource.FindByID(obj.Key);
        if (node == null) return;

        bool isCtrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        if (MultiSelection)
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
                foreach (var sw in _keepUIList) sw.IsSelected = false;
                _selectionList.Clear();
                _selectionList.Add(node);
                obj.IsSelected = true;
            }
        }
        else
        {
            foreach (var sw in _keepUIList) sw.IsSelected = false;

            _selectionList.Clear();
            _selectionList.Add(node);
            obj.IsSelected = true;
        }

        SelectionChanged?.Invoke(this);
        NodeClick?.Invoke(node, e);
        InvalidateVisual();
    }

    private void Node_RightClicked(BubbleSwitch obj, MouseButtonEventArgs e)
    {
        BubbleTreeViewItem? node = _dataSource.FindByID(obj.Key);
        if (node == null) return; 
        NodeRightClick?.Invoke(node, e);
    }

    private void Node_Colapsed(BubbleSwitch obj)
    {
        BubbleTreeViewItem? node = _dataSource.FindByID(obj.Key);
        if (node == null) return;

        if (_rootList.Contains(node)) // Roots
        {
            _pathList[node.Key].Clear();
            _subList[node.Key].Clear();
        }
        UpdateUI();
        NodeCollapsed?.Invoke(node);
    }
    private void Node_Expanded(BubbleSwitch obj)
    {
        BubbleTreeViewItem? node = _dataSource.FindByID(obj.Key);
        if (node == null) return;

        NodeExpand(node);
        NodeExpanded?.Invoke(node);
    }
    private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        RightClick?.Invoke(this);
    }

    #endregion

    #region override
    // BubbleTree.cs
    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        var pen = new Pen(Brushes.Gray, 1);

        // über alle Workspaces/Roots
        foreach (var root in _rootList)
        {
            // UI-Elemente auf dem Canvas finden
            var rootSw = FindSwitchByKey(root.Key);
            if (rootSw == null) continue;

            // Pfad & Subs für diesen Root holen (können leer sein)
            _pathList.TryGetValue(root.Key, out var path);
            _subList.TryGetValue(root.Key, out var subs);
            path ??= new List<BubbleTreeViewItem>();
            subs ??= new List<BubbleTreeViewItem>();

            if (path.Count > 0)
            {
                // (1) Root -> erster Pfadknoten (L-Linie, erst runter, dann rüber)
                var firstPathSw = FindSwitchByKey(path[0].Key);
                if (firstPathSw != null)
                {
                    DrawLLine(dc, GetRightCenter(rootSw), GetLeftCenter(firstPathSw), pen, horizontalFirst: false);
                }

                // (2) Pfadkette verbinden
                for (int i = 1; i < path.Count; i++)
                {
                    var from = FindSwitchByKey(path[i - 1].Key);
                    var to = FindSwitchByKey(path[i].Key);
                    if (from != null && to != null)
                    {
                        // L-Linie mit erst horizontalem Schritt wirkt sauberer als Diagonale
                        DrawLLine(dc, GetRightCenter(from), GetLeftCenter(to), pen, horizontalFirst: true);
                    }
                }

                // (3) Letzter Pfadknoten -> Subs
                var lastPathSw = FindSwitchByKey(path[^1].Key);
                if (lastPathSw != null)
                {
                    foreach (var sub in subs)
                    {
                        var subSw = FindSwitchByKey(sub.Key);
                        if (subSw != null)
                        {
                            DrawLLine(dc, GetRightCenter(lastPathSw), GetLeftCenter(subSw), pen, horizontalFirst: false);
                        }
                    }
                }
            }
            else
            {
                // (4) Kein Pfad: Root -> Subs (entspricht "Parent -> SubNodes" aus der Doku)
                foreach (var sub in subs)
                {
                    var subSw = FindSwitchByKey(sub.Key);
                    if (subSw != null)
                    {
                        DrawLLine(dc, GetRightCenter(rootSw), GetLeftCenter(subSw), pen, horizontalFirst: false);
                    }
                }
            }
        }
    }

    #endregion

    #region  Handling
    public void NewNodeAdded(string parentKey)
    {
        BubbleTreeViewItem? node = _dataSource.FindByID(parentKey);
        if (node == null) return;
        NodeExpand(node);
    }
    public void NodeRemovedAdded(string parentKey)
    {
        BubbleTreeViewItem? node = _dataSource.FindByID(parentKey);
        if (node == null) return;
        NodeExpand(node);
    }
    #endregion
}