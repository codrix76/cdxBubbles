using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleControlls.Models
{
    public class MenuLevelDefinition
    {
        public double Start { get; set; }
        public double End { get; set; }
        public double Center { get; set; } 

        public MenuLevelDefinition()
        {
            Start = 0;
            Center = 0.5;
            End = 1;
        }

        public MenuLevelDefinition(double start, double center, double end)
        {
            Start = start;
            End = end;
            Center = center;
        }
    }
}
