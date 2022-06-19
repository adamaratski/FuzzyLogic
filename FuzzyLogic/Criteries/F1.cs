namespace Math.Actions.Criteries
{
    using FuzzyLogic.Criteries;
    using System;

    public class F1 : Criterion
    {
        public override string Name => "F1";

        public override double GetValue(double[,] matrix, int[] clusters, double treshold)
        {
            Double criterion = 0;
            Int32 width = matrix.GetLength(0);
            foreach (int variator in clusters)
            {
                Int32 cardiagan = 0;
                Double summary = 0;
                for (Int32 index = 0; index < width; index++)
                {
                    if (matrix[variator, index] >= treshold)
                    {
                        cardiagan++;
                        summary += matrix[variator, index];
                    }
                }

                criterion += summary / cardiagan;
            }

            criterion -= treshold * clusters.Length;
            return criterion;
        }

        public override int CompareTo(double valueA, double valueB)
        {
            return valueA.CompareTo(valueB);
        }
    }
}
