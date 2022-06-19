namespace Math.Actions.Distances
{
    using FuzzyLogic.Distances;
    using System;
    using System.Linq;

    public class HammingDistance : Distance
    {
        public override string Name => "Normalized Hamming distance";

        public override double GetDistance(double[] arrayA, double[] arrayB)
        {
            if (arrayA.Length != arrayB.Length)
            {
                throw new ArgumentException("Length of arguments is not equal");
            }

            return arrayA.Select((t, index) => System.Math.Abs(t - arrayB[index])).Sum() / arrayA.Length;
        }
    }
}
