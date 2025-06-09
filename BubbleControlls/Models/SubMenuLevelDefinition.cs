using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BubbleControlls.Models
{
    public class SubMenuLevelDefinition
    {
        public int Level { get; set; }
        public double Rx { get; set; }
        public double Ry { get; set; }
        public Point Center { get; set; }
        public double DistributionDistance { get; set; }
    }
}
