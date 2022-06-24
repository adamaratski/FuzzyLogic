namespace FuzzyLogic.Criteries
{
    using System;

    public class F1 : Criterion
    {
        public override string Name => "F1";

        public override double GetValue(double[,] matrix, int[] clusters, double treshold)
        {
            double criterion = 0;
            int width = matrix.GetLength(0);
            foreach (int variator in clusters)
            {
                int cardiagan = 0;
                double summary = 0;
                for (int index = 0; index < width; index++)
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
