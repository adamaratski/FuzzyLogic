#define Parallel

namespace FuzzyLogic.Clustering
{
    using FuzzyLogic.Common;
    using FuzzyLogic.Criteries;
    using Math.Actions.ThresholdFilters;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The dafc.
    /// </summary>
    public class DAFC
    {
        public delegate bool ResultCallback(double[,] matrix, int[] destribution, double threshold, int count, ref int intersection, ref double criterion);

        public bool ValidateResult(double[,] matrix, int[] destribution, double threshold, int count, ref int intersection, ref double criterion)
        {
            return true;
        }

        public IEnumerable<KeyValuePair<int[], double>> Execute(double[,] data, out int intersections)
        {
            for (int count = 2; count < data.GetLength(0); count++)
            {
                var result = Execute(data, count, out intersections);
                if (result.Any())
                {
                    return result;
                }
            }

            intersections = 0;
            return Enumerable.Empty<KeyValuePair<int[], double>>();
        }

        public IEnumerable<KeyValuePair<int[], double>> Execute(double[,] data, int clusterCount, out int intersections)
        {
            return Execute(data, clusterCount, new All().GetFilteredValues(data).ToArray(), out intersections);
        }

        public IEnumerable<KeyValuePair<int[], double>> Execute(double[,] data, int clusterCount, double[] thresholds, out int intersections)
        {
            intersections = -1;
            var result = Enumerable.Empty<KeyValuePair<int[], double>>();
            do
            {
                intersections++;
                result = Execute(data, clusterCount, thresholds, intersections);
            } while (result == null || result.Any() == false || intersections >= data.GetLength(0));

            return result;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="thresholds">
        /// The thresholds.
        /// </param>
        /// <param name="clusterCount">
        /// The cluster count.
        /// </param>
        /// <param name="intersectionsLimit">
        /// The intersections limit.
        /// </param>
        /// <param name="criterion">
        /// The criterion.
        /// </param>
        public ConcurrentDictionary<int[], double> Execute(double[,] data, int clusterCount, double[] thresholds, int intersectionsLimit)
        {
            ConcurrentDictionary<int[], double> results = new ConcurrentDictionary<int[], double>();
            Parallel.ForEach(thresholds.OrderByDescending(item => item), threshold =>
                {
                    List<int[]> result = Clusterize(data, clusterCount, threshold, intersectionsLimit).ToList();
                    result.ToList().ForEach(item => results.TryAdd(item, threshold));
                });

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Creiterion class</typeparam>
        /// <param name="results"></param>
        /// <param name="data"></param>
        /// <param name="optimalCriteionValue"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<int[], double>> FindOptimal<T>(IEnumerable<KeyValuePair<int[], double>> results, double[,] data, out double optimalCriteionValue) where T : Criterion
        {
            Criterion criterion = Activator.CreateInstance<T>();
            optimalCriteionValue = 0;
            Dictionary<int[], double> optimalResults = new Dictionary<int[], double>();
            foreach (KeyValuePair<int[], double> result in results)
            {
                double criterionValue = criterion.GetValue(data, result.Key, result.Value);

                if (optimalResults.Count == 0)
                {
                    optimalCriteionValue = criterionValue;
                    optimalResults.Add(result.Key, result.Value);
                }
                else
                {
                    switch (criterion.CompareTo(criterionValue, optimalCriteionValue))
                    {
                        case 0:
                            optimalResults.Add(result.Key, result.Value);
                            break;
                        case 1:
                            optimalResults.Clear();
                            optimalCriteionValue = criterionValue;
                            goto case 0;
                    }
                }
            }

            return optimalResults;
        }

        /// <summary>
        /// The clusterize.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="threshold">
        /// The threshold.
        /// </param>
        /// <param name="clusterCount">
        /// The cluster count.
        /// </param>
        /// <param name="intersectionsLimit">
        /// The intersections limit.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<int[]> Clusterize(double[,] data, int clusterCount, double threshold, int intersectionsLimit)
        {
            int size = data.GetLength(0);

            // Validation of existance decision
            List<int> invalidList = new List<int>();

            // Minimal power
            int minimalPower = size;

            // Maximal power
            int maximalPower = 0;
#if Parallel
            Parallel.For(0, size, clusterIndex =>
            {
                Int32 cardigan = 0;
                for (Int32 index = 0; index < size; index++)
                {
                    if (data[clusterIndex, index] >= threshold)
                    {
                        cardigan++;
                    }
                }

                minimalPower = (minimalPower > cardigan) ? cardigan : minimalPower;
                maximalPower = (maximalPower < cardigan) ? cardigan : maximalPower;

                // Generated list of invalid indexes
                if (cardigan > size - clusterCount + 1)
                {
                    lock (invalidList)
                    {
                        invalidList.Add(clusterIndex);
                    }
                }
            });
#else
            for (int clusterIndex = 0; clusterIndex < size; clusterIndex++)
            {
                int cardigan = 0;
                for (int index = 0; index < size; index++)
                {
                    if (data[clusterIndex, index] >= threshold)
                    {
                        cardigan++;
                    }
                }

                minimalPower = minimalPower > cardigan ? cardigan : minimalPower;
                maximalPower = maximalPower < cardigan ? cardigan : maximalPower;

                // Generate list of invalid indexes
                if (cardigan > size - clusterCount + 1)
                {
                    invalidList.Add(clusterIndex);
                }
            }
#endif
            // Underflow
            if (maximalPower * clusterCount < size)
            {
                yield break;
            }

            // Overflow
            if (minimalPower * clusterCount - intersectionsLimit > size)
            {
                yield break;
            }

            // Fill variation matrix
            int[] variation = new int[clusterCount];
            for (int index = 0; index < clusterCount; index++)
            {
                variation[index] = index;
            }

            while (true)
            {
                // Checking for decision
                bool complite = false;
                int currentIntersections = 0;
                for (int x = 0; x < size; x++)
                {
                    complite = false;
                    for (int index = 0; index < clusterCount; index++)
                    {
                        if (data[x, variation[index]] >= threshold)
                        {
                            currentIntersections = complite ? ++currentIntersections : currentIntersections;
                            complite = true;
                            if (currentIntersections > intersectionsLimit)
                            {
                                break;
                            }
                        }
                    }

                    if (!complite || currentIntersections > intersectionsLimit)
                    {
                        break;
                    }
                }

                if (!complite || currentIntersections > intersectionsLimit)
                {
                    goto next;
                }

                yield return variation.Clone<int[]>();

            // Generate new sequence
            next:
                for (int index = clusterCount - 1; index > -1; index--)
                {
                    if (variation[index] == size - clusterCount + index)
                    {
                        if (index == 0)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        variation[index] = variation[index] + 1;
                        for (int variationIndex = index; variationIndex < clusterCount; variationIndex++)
                        {
                            variation[variationIndex] = variation[index] + variationIndex - index;
                        }

                        break;
                    }
                }

                if (invalidList.Count > 0)
                {
                    if (invalidList.Any(value => Array.IndexOf(variation, value) != -1))
                    {
                        goto next;
                    }
                }
            }
        }

    }
}
