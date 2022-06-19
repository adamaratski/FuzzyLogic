namespace Math.Actions.Distances
{
    using FuzzyLogic.Distances;
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

            return System.Math.Sqrt(arrayA.Select((t, index) => System.Math.Pow(System.Math.Abs(t - arrayB[index]), 2)).Sum() / arrayA.Length);
        }
    }
}
