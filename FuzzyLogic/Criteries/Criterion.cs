namespace FuzzyLogic.Criteries
{
    using System;
    using Common;

    public abstract class Criterion : MatrixAction
    {
        public abstract double GetValue(double[,] matrix, int[] clusters, double treshold);

        public abstract int CompareTo(double valueA, double valueB);
    }
}
