using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.Common
{
    public class DAFCDecision
    {
        public Matrix Matrix { get; set; }
        public Matrix Source { get; set; }
        public IList<int> Distribution { get; init; }
        public double Alfa { get; init; }
        public int Intersection { get; init; }
        public double Criterion { get; init; }
    }
}
