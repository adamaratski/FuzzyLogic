namespace Math.Actions.ThresholdFilters
{
    using FuzzyLogic.ThresholdFilters;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class All : ThresholdFilter
    {
        public override string Name => "All";

        public override ThresholdFilterType Type
        {
            get
            {
                return ThresholdFilterType.Auto;
            }
        }

        public override IEnumerable<double> GetFilteredValues(double[,] data, params object[] parameters)
        {
            List<Double> values = new List<double>();
            Int32 width = data.GetLength(0);
            Int32 height = data.GetLength(1);

            for (Int32 x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    values.Add(data[x, y]);
                }
            }

            return values.Where(item => item != 0 && item != 1).Distinct().OrderByDescending(item => item);
        }
    }
}
