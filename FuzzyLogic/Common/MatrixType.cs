using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.Common
{
    [FlagsAttribute]
    public enum MatrixType
    {
        Squared = 1,
        Diagonal = 2,
        Vector = 4,
        Zeros = 8,
        Ones = 16,
        HiTrianglular = 32,
        LowTriangular = 64,
        Scalar = 128
    }
}
