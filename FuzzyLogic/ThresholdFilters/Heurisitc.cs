namespace Math.Actions.ThresholdFilters
{
    using FuzzyLogic.ThresholdFilters;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Heurisitc : ThresholdFilter
    {
        public override string Name => "Heurisitc [MIN(X(i) / X(i-1))]";

        public override ThresholdFilterType Type
        {
            get
            {
                return ThresholdFilterType.Auto;
            }
        }

        public override IEnumerable<double> GetFilteredValues(double[,] data, params object[] parameters)
        {
            Double maximal = Double.MinValue;

            List<Double> values = new List<double>();
            Int32 width = data.GetLength(0);
            Int32 height = data.GetLength(1);
            for (Int32 x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    values.Add(data[x, y]);
                    if (maximal < data[x, y])
                    {
                        maximal = data[x, y];
                    }
                }
            }

            values.Sort();
            Double value = Double.NaN;
            Double minimalHeuristic = Double.NaN;
            for (Int32 index = 1; index < values.Count; index++)
            {
                Double currentHeuristic = values[index - 1] / values[index];
                if (Double.IsNaN(minimalHeuristic) || currentHeuristic < minimalHeuristic)
                {
                    value = values[index];
                    minimalHeuristic = currentHeuristic;
                }
            }

            yield return value;
        }
    }
}
