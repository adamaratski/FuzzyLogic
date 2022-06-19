namespace Math.Clustering
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Actions;
    using Actions.Criteries;
    using FuzzyLogic.Clustering;
    using FuzzyLogic.Common;
    using FuzzyLogic.Criteries;

    public class DAFCu : DAFC
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Squared matrix of data</param>
        /// <param name="thresholds">List of alowable relations between any two objects</param>
        /// <param name="weight">Maximal weight of clusters</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<Int32[], Double>> Execute(Double[,] data, Double[] thresholds, Int32 weight)
        {
            Trace.Assert(data.GetLength(0) == data.GetLength(1));

            // Number of objects
            Int32 size = data.GetLength(0);

            // u - number of maximal cluster power
            for (Int32 u = weight; u >= 1; u--)
            {
                // w - number of maximal intersections
                for (Int32 w = 0; w <= size; w++)
                {
                    // iterations for all thresholds
                    foreach (Double threshold in thresholds.OrderBy(item => item))
                    {
                        for (Int32 count = 2; count < size; count++)
                        {
                            // Get all possible decisons
                            List<Int32[]> clusters = this.Clusterize(data, count, threshold, w).ToList();

                            if (clusters.Any())
                            {
                                // Filter all decisions by: power, intersections and criterion
                                List<KeyValuePair<Int32[], Double>> filterClusters = this.FindOptimal(clusters, data, new F1(), threshold, u, w).ToList();

                                if (filterClusters.Any())
                                {
                                    // Return the found value
                                    return filterClusters;
                                }
                            }
                        }
                    }
                }
            }

            // Return empty decision
            return new List<KeyValuePair<Int32[], Double>>();
        }

        public IEnumerable<KeyValuePair<Int32[], Double>> FindOptimal(IEnumerable<Int32[]> results, Double[,] data, Criterion criterion, Double threshold, Int32 weight, Int32 intersectionsLimit)
        {
            Dictionary<Int32[], Double> optimalResults = new Dictionary<int[], double>();
            Double optimalCriteionValue = 0;
            foreach (int[] result in results)
            {
                // Current criterion
                Double criterionValue = criterion.GetValue(data, result, threshold);

                // Current intersections
                Int32 intersections = data.GetIntersections(result, threshold);

                // Vector of power of clusters
                Int32[] power = data.GetPowerVector(result, threshold);

                if (power.Max() > weight || intersections > intersectionsLimit)
                {
                    continue;
                }

                if (optimalResults.Count == 0)
                {
                    optimalCriteionValue = criterionValue;
                    optimalResults.Add(result, threshold);
                }
                else
                {
                    switch (criterion.CompareTo(criterionValue, optimalCriteionValue))
                    {
                        case 0:
                            // Add new decision with same criterion
                            optimalResults.Add(result, threshold);
                            break;
                        default:
                            // Replace with new decision
                            optimalResults.Clear();
                            optimalCriteionValue = criterionValue;
                            goto case 0;
                    }
                }
            }

            return optimalResults;
        }
    }
}
