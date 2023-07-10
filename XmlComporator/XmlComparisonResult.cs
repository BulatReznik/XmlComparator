using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlComparator
{
    public class XmlComparisonResult
    {
        public double DifferenceCoefficient { get; set; }
        public int DifferentNodesCount { get; set; }
        public List<string>? Differences { get; set; }
    }
}
