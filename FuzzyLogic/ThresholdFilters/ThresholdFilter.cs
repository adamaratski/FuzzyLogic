namespace FuzzyLogic.ThresholdFilters
{
    using FuzzyLogic.Common;
    using System.Collections.Generic;

    public abstract class ThresholdFilter : MatrixAction
    {
        public abstract ThresholdFilterType Type { get; }

        public abstract IEnumerable<double> GetFilteredValues(double[,] data, params object[] parameters);
    }
}
