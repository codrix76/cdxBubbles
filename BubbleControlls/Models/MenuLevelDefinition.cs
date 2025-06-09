using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleControlls.Models
{
    public class MenuLevelDefinition
    {
        public double Start { get; set; } = 0;
        public double End { get; set; } = 1;
        public double Center { get; set; } = 0.5;

        public MenuLevelDefinition(double start, double center, double end)
        {
            Start = start;
            End = end;
            Center = center;
        }
    }
}
