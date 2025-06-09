using BubbleControlls.ControlViews;
using BubbleControlls.Helpers;
using BubbleControlls.Models;
using System;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Windows;

namespace BubbleControlls.ViewModels
{
    public class BubbleMenuViewModel : INotifyPropertyChanged
    {
        #region Variablen
        private BubbleMenuHandler _bubbleMenuHandler = new BubbleMenuHandler();
        private List<BubbleMenuItem> _currentMenuPath = new List<BubbleMenuItem>();
        private List<BubbleMenuItem> _currentMenuSelection = new List<BubbleMenuItem>();
        private MenuLevelDefinition[] _menuLevelSizes = new MenuLevelDefinition[4];
        #endregion

        #region Properties
        public BubbleMenuHandler BubbleMenuHandler { get => _bubbleMenuHandler; }
        public int MenuDepth => _bubbleMenuHandler.GetMaxDepth();
        public int MaxMenuLevels { get; } = 4;
        public Point MenuAreaFrom { get; set; } = new Point(0, 0);
        public Point MenuAreaTo { get; set; } = new Point(0, 0);
        public double MenuAreaFromRadian { get; set; } = 0.0;
        public double MenuAreaToRadian { get; set; } = 0.0;
        public double DistributionRadian { get; set; } = 0;
        public double GetMenuLevelLenght { get; private set; } = 0;
        public DistributionAlignmentType DistributionAlignment { get; set; } = DistributionAlignmentType.Center;
        public List<BubbleMenuItem> CurrentMenuPath { get => _currentMenuPath; }
        public List<BubbleMenuItem> CurrentMenuSelection { get => _currentMenuSelection; }
        public MenuLevelDefinition[] MenuLevelSizes { get => _menuLevelSizes; }
        #endregion
        public BubbleMenuViewModel()
        {
        }

        #region Methods
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void SetDistributionRadian(BubbleMenuAlignmentType menuAlignment)
        {
            DistributionRadian = (menuAlignment, DistributionAlignment) switch
            {
                // TopEdge
                (BubbleMenuAlignmentType.TopEdge, DistributionAlignmentType.Center) => Math.PI / 2,          // 90°
                (BubbleMenuAlignmentType.TopEdge, DistributionAlignmentType.From) => 0,                    // 0°
                (BubbleMenuAlignmentType.TopEdge, DistributionAlignmentType.To) => Math.PI,              // 180°

                // TopLeftCorner
                (BubbleMenuAlignmentType.TopLeftCorner, DistributionAlignmentType.Center) => Math.PI / 4,    // 45°
                (BubbleMenuAlignmentType.TopLeftCorner, DistributionAlignmentType.From) => 0,              // 0°
                (BubbleMenuAlignmentType.TopLeftCorner, DistributionAlignmentType.To) => Math.PI / 2,    // 90°

                // LeftEdge
                (BubbleMenuAlignmentType.LeftEdge, DistributionAlignmentType.Center) => 0,                   // 0°
                (BubbleMenuAlignmentType.LeftEdge, DistributionAlignmentType.From) => 3 * Math.PI / 2,     // 270°
                (BubbleMenuAlignmentType.LeftEdge, DistributionAlignmentType.To) => Math.PI,

                // Fallback
                _ => throw new ArgumentOutOfRangeException(nameof(menuAlignment), "Unsupported alignment/type combination")
            };
        }
        public void SetMenuLevelSizes(double big, double small, double spacing)
        {
            _menuLevelSizes[0] = new MenuLevelDefinition(0, big / 2, big + spacing / 2); // Zentrum für Hauptmenu
            _menuLevelSizes[1] = new MenuLevelDefinition(_menuLevelSizes[0].End, _menuLevelSizes[0].End + small/2, _menuLevelSizes[0].End + small + spacing /2);
            _menuLevelSizes[2] = new MenuLevelDefinition(_menuLevelSizes[1].End, _menuLevelSizes[1].End + big / 2, _menuLevelSizes[1].End + big + spacing / 2);
            _menuLevelSizes[3] = new MenuLevelDefinition(_menuLevelSizes[2].End, _menuLevelSizes[2].End + big / 2, _menuLevelSizes[2].End + big + spacing / 2);
            GetMenuLevelLenght = _menuLevelSizes[3].End;
        }
        #endregion

        #region EventHandling
        public void BubbleClicked(string id)
        {
            _bubbleMenuHandler.HandleClick(id);
        }
        #endregion
    }
}
