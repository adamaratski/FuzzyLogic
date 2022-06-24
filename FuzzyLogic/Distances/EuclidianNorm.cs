namespace FuzzyLogic.Distances
{
    using System;
    using System.Linq;

    public class EuclidianNorm : Distance
    {
        public override string Name => "Normalized Euclidean distance";

        public override double GetDistance(double[] arrayA, double[] arrayB)
        {
            if (arrayA.Length != arrayB.Length)
            {
                throw new ArgumentException("Length of arguments is not equal");
            }

            return Math.Sqrt(arrayA.Select((t, index) => Math.Pow(Math.Abs(t - arrayB[index]), 2)).Sum() / arrayA.Length);
        }
    }
}
