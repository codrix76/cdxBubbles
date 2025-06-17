using System.Windows;
using BubbleControlls.Models;

namespace BubbleControlls.ViewModels
{
    public class BubbleMenuViewModel
    {
        #region Variablen
        private BubbleMenuHandler _bubbleMenuHandler = new BubbleMenuHandler();
        #endregion

        #region Properties
        public BubbleMenuHandler BubbleMenuHandler { get => _bubbleMenuHandler; }
        public int MenuDepth => _bubbleMenuHandler.GetMaxDepth();
        public double GetMenuLevelLenght { get; private set; }
        public DistributionAlignmentType DistributionAlignment { get; set; } = DistributionAlignmentType.Center;
        public List<BubbleMenuItem> CurrentMenuPath { get; } = new List<BubbleMenuItem>();
        public List<BubbleMenuItem> CurrentMenuSelection { get; } = new List<BubbleMenuItem>();
        public MenuLevelDefinition[] MenuLevelSizes { get; private set; } = new MenuLevelDefinition[0];

        #endregion

        #region Methods
        public void SetMenuLevelSizes(double[] sizes, double spacing)
        {
            MenuLevelSizes = new MenuLevelDefinition[sizes.Length];
            for (int i = 0; i < sizes.Length; i++)
            {
                MenuLevelSizes[i] = new MenuLevelDefinition();
                if (i == 0)
                {
                    MenuLevelSizes[i].Start = 0;
                }
                else
                {
                    MenuLevelSizes[i].Start = MenuLevelSizes[i-1].End;
                }
                MenuLevelSizes[i].End = MenuLevelSizes[i].Start + sizes[i] + spacing;
                MenuLevelSizes[i].Center = (MenuLevelSizes[i].Start + MenuLevelSizes[i].End) / 2;
            }
            
            GetMenuLevelLenght = MenuLevelSizes[sizes.Length-1].End;
        }

        public BubbleAlignmentValues UpdateAlignmentValues(BubbleMenuAlignmentType alignmentType,
            double minHeight, double minLength, double menuHeight, double menuWidth, double mainMenuSize)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.WorkArea.Height;//SystemParameters.PrimaryScreenHeight;
            
            BubbleAlignmentValues values = new BubbleAlignmentValues();
            if (alignmentType == BubbleMenuAlignmentType.TopLeftCorner)
            {
                values.MenuHeight = menuHeight;
                values.MenuWidth = menuWidth;
                values.WindowTop = 0;
                values.WindowLeft = 0;
                values.MenuCenter = new Point(mainMenuSize / 2, mainMenuSize / 2);
                values.RingCenter = new Point(0, 0);
                values.StartAngle = 0;
                values.EndAngle = 90;
                values.IsCentered = true;
                values.IsInverted = false;
            }
            if (alignmentType == BubbleMenuAlignmentType.TopEdge)
            {
                values.MenuWidth = menuWidth * 2;
                values.MenuHeight = menuHeight;
                values.WindowTop = 0;
                values.WindowLeft = (screenWidth - values.MenuWidth) / 2;
                values.MenuCenter = new Point(values.MenuWidth / 2, mainMenuSize / 2);
                values.RingCenter = new Point(values.MenuWidth / 2, 0);
                values.StartAngle = 0;
                values.EndAngle = 180;
                values.IsCentered = true;
                values.IsInverted = true;
            }
            if (alignmentType == BubbleMenuAlignmentType.LeftEdge)
            {
                values.MenuHeight = menuHeight * 2;
                values.MenuWidth = menuWidth;
                values.WindowTop = (screenHeight - values.MenuHeight) / 2;
                values.WindowLeft = 0;
                values.MenuCenter = new Point(mainMenuSize / 2, values.MenuHeight / 2);
                values.RingCenter = new Point(0, values.MenuHeight  / 2);
                values.StartAngle = 270;
                values.EndAngle = 90;
                values.IsCentered = true;
                values.IsInverted = false;
            }
            if (alignmentType == BubbleMenuAlignmentType.RightEdge)
            {
                values.MenuHeight = menuHeight * 2;
                values.MenuWidth = menuWidth;
                values.WindowTop = (screenHeight - values.MenuHeight) / 2;
                values.WindowLeft = (screenWidth - menuWidth);
                values.MenuCenter = new Point(menuWidth - (mainMenuSize / 2), values.MenuHeight / 2);
                values.RingCenter = new Point(menuWidth, values.MenuHeight / 2);
                values.StartAngle = 90;
                values.EndAngle = 270;
                values.IsCentered = true;
                values.IsInverted = true;
            }
            if (alignmentType == BubbleMenuAlignmentType.BottomEdge)
            {
                values.MenuHeight = menuHeight;
                values.MenuWidth = menuWidth * 2;
                values.WindowTop = screenHeight - menuHeight;
                values.WindowLeft = (screenWidth - values.MenuWidth) / 2;
                values.MenuCenter = new Point(values.MenuWidth / 2, menuHeight - (mainMenuSize / 2));
                values.RingCenter = new Point(values.MenuWidth / 2, menuHeight);
                values.StartAngle = 180;
                values.EndAngle = 0;
                values.IsCentered = true;
                values.IsInverted = false;
            }
            if (alignmentType == BubbleMenuAlignmentType.Free)
            {
                values.MenuHeight = menuHeight * 2;
                values.MenuWidth = menuWidth * 2;
                values.WindowTop = (screenHeight - values.MenuHeight) / 2;
                values.WindowLeft = (screenWidth - values.MenuWidth) / 2;
                values.MenuCenter = new Point(values.MenuWidth / 2, values.MenuHeight / 2);
                values.RingCenter = new Point(values.MenuWidth / 2, values.MenuHeight / 2);
                values.StartAngle = 0;
                values.EndAngle = 359.9;
                values.IsCentered = false;
                values.IsInverted = false;
            }
            return values;
        }
        #endregion

        #region EventHandling
        public void BubbleClicked(string id)
        {
            _bubbleMenuHandler.HandleClick(id);
        }
        #endregion
    }

    public class BubbleAlignmentValues
    {
        public double WindowTop { get; set; }
        public double WindowLeft { get; set; }
        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
        public double MenuHeight { get; set; }
        public double MenuWidth { get; set; }
        public Point MenuCenter { get; set; }
        public Point RingCenter { get; set; }
        public bool IsCentered { get; set; }
        public bool IsInverted { get; set; }
    }
}
