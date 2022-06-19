namespace FuzzyLogic.Clustering
{
    using FuzzyLogic.Common;
    using FuzzyLogic.Criteries;
    using FuzzyLogic.Distances;
    using FuzzyLogic.Normirations;
    using FuzzyLogic.TClosure;
    using Math.Actions.Criteries;
    using Math.Actions.ThresholdFilters;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;

    public class DAFCTAGA
    {
        public IEnumerable<KeyValuePair<int[], double>> Execute(double[,] source, Distance distance, Normiration normiration, Criterion criterion, TAction action, out double[,] data)
        {
            Matrix matrix = source.ToMatrix();

            matrix = 1 - distance.GetRelationMatrixForRows(normiration == null ? matrix : normiration.NormirateColumns(matrix));
            data = action.Process(matrix.ToArray(false));

            int objectsCount = source.GetLength(0);

            double threshold = new Heurisitc().GetFilteredValues(data).Distinct().First();

            DAFC dafc = new DAFC();

            for (int count = 2; count <= objectsCount; count++)
            {
                IEnumerable<KeyValuePair<int[], double>> result = dafc.Execute(data, count, new[] { threshold }, 0);
                result = FindOptimal(result, source, data, criterion, distance);

                if (result.Any())
                {
                    return result;
                }
            }

            return null;
        }

        public IEnumerable<KeyValuePair<int[], double>> FindOptimal(IEnumerable<KeyValuePair<int[], double>> results, double[,] source, double[,] data, Criterion criterion, Distance distance)
        {
            Dictionary<int[], double> optimalResults = new Dictionary<int[], double>();
            double optimalCriteionValue = double.MinValue;
            double optimalCoreDeviation = double.NaN;
            foreach (KeyValuePair<int[], double> result in results)
            {
                double criterionValue = criterion.GetValue(data, result.Key, result.Value);

                switch (criterion.CompareTo(criterionValue, optimalCriteionValue))
                {
                    case 0:
                        goto case 1;
                    case 1:

                        // Geometric centers
                        double[,] geometricCenters;
                        double coreDeviation = GetCoreDeviation(source, data, result.Key, distance, result.Value, out geometricCenters);

                        if (double.IsNaN(optimalCoreDeviation) || coreDeviation < optimalCoreDeviation)
                        {
                            optimalCriteionValue = criterionValue;
                            optimalCoreDeviation = coreDeviation;

                            optimalResults.Clear();
                            optimalResults.Add(result.Key, result.Value);
                        }

                        break;
                    case -1:
                        break;
                }
            }

            return optimalResults;
        }

        public double GetCoreDeviation(double[,] source, double[,] data, int[] destribution, Distance distance, double threshold, out double[,] geometricCenters)
        {
            double[,] centers = new double[source.GetLength(0), destribution.Length];
            for (int index = 0; index < destribution.Length; index++)
            {
                for (int x = 0; x < source.GetLength(0); x++)
                {
                    int memberCount = 0;
                    for (int y = 0; y < source.GetLength(1); y++)
                    {
                        if (data[destribution[index], y] >= threshold)
                        {
                            centers[x, index] += source[x, y];
                            memberCount++;
                        }
                    }

                    if (memberCount > 1)
                    {
                        centers[x, index] /= memberCount;
                    }
                }
            }

            geometricCenters = centers;

            return destribution.Select((item, index) => distance.GetDistance(source.GetRow(item), centers.GetRow(index))).Sum();
        }

        public XmlDocument ToXmlDocument(double[,] source, double[,] data, double threshold, int[] distribution, Distance distance)
        {
            XmlDocument document = new XmlDocument();

            document.AppendChild(document.CreateElement("workset"));

            // Add matrix
            XmlNode sourceNode = document.DocumentElement.AppendChild(document.CreateElement("source"));
            for (int attribute = 0; attribute < source.GetLength(0); attribute++)
            {
                for (int index = 0; index < source.GetLength(1); index++)
                {
                    XmlNode itemNode = sourceNode.AppendChild(document.CreateElement("item"));
                    itemNode.Attributes.Append(document.CreateAttribute("attribute")).Value = attribute.ToString(CultureInfo.InvariantCulture);
                    itemNode.Attributes.Append(document.CreateAttribute("index")).Value = index.ToString(CultureInfo.InvariantCulture);
                    itemNode.Attributes.Append(document.CreateAttribute("value")).Value = source[attribute, index].ToString(CultureInfo.InvariantCulture);
                }
            }

            // Add matrix
            XmlNode matrixNode = document.DocumentElement.AppendChild(document.CreateElement("matrix"));
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for (int y = 0; y < data.GetLength(1); y++)
                {
                    XmlNode itemNode = matrixNode.AppendChild(document.CreateElement("item"));
                    itemNode.Attributes.Append(document.CreateAttribute("x")).Value = x.ToString(CultureInfo.InvariantCulture);
                    itemNode.Attributes.Append(document.CreateAttribute("y")).Value = y.ToString(CultureInfo.InvariantCulture);
                    itemNode.Attributes.Append(document.CreateAttribute("value")).Value = data[x, y].ToString(CultureInfo.InvariantCulture);
                }
            }

            // Add class count
            XmlNode decisionNode = document.DocumentElement.AppendChild(document.CreateElement("decision"));
            decisionNode.Attributes.Append(document.CreateAttribute("algorithm")).Value = "DAFC-TAGA";
            decisionNode.Attributes.Append(document.CreateAttribute("threshold")).Value = threshold.ToString(CultureInfo.InvariantCulture);
            decisionNode.Attributes.Append(document.CreateAttribute("count")).Value = distribution.Length.ToString(CultureInfo.InvariantCulture);
            decisionNode.Attributes.Append(document.CreateAttribute("criterion")).Value = "F1";
            decisionNode.Attributes.Append(document.CreateAttribute("criterion_value")).Value = new F1().GetValue(data, distribution, threshold).ToString(CultureInfo.InvariantCulture);

            // Add distribution
            XmlNode distributionNode = decisionNode.AppendChild(document.CreateElement("distribution"));
            for (int index = 0; index < distribution.Length; index++)
            {
                XmlNode itemNode = distributionNode.AppendChild(document.CreateElement("item"));
                itemNode.Attributes.Append(document.CreateAttribute("index")).Value = index.ToString(CultureInfo.InvariantCulture);
                itemNode.Attributes.Append(document.CreateAttribute("value")).Value = distribution[index].ToString(CultureInfo.InvariantCulture);

                // Add members
                XmlNode membersNode = itemNode.AppendChild(document.CreateElement("members"));
                for (int y = 0; y < data.GetLength(1); y++)
                {
                    if (data[distribution[index], y] >= threshold)
                    {
                        XmlNode memberNode = membersNode.AppendChild(document.CreateElement("item"));
                        memberNode.Attributes.Append(document.CreateAttribute("index")).Value = y.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            // Add geometric centers
            double[,] geometricCenters;
            double coreDeviation = GetCoreDeviation(source, data, distribution, distance, threshold, out geometricCenters);
            XmlNode geometricCentersNode = decisionNode.AppendChild(document.CreateElement("geometric_centers"));
            geometricCentersNode.Attributes.Append(document.CreateAttribute("core_deviation")).Value = coreDeviation.ToString(CultureInfo.InvariantCulture);

            for (int index = 0; index < geometricCenters.GetLength(0); index++)
            {
                XmlNode geometricCenterNode = geometricCentersNode.AppendChild(document.CreateElement("item"));
                geometricCenterNode.Attributes.Append(document.CreateAttribute("index")).Value = index.ToString(CultureInfo.InvariantCulture);
                for (int attribute = 0; attribute < geometricCenters.GetLength(1); attribute++)
                {
                    XmlNode attributeNode = geometricCenterNode.AppendChild(document.CreateElement("item"));
                    attributeNode.Attributes.Append(document.CreateAttribute("index")).Value = attribute.ToString(CultureInfo.InvariantCulture);
                    attributeNode.Attributes.Append(document.CreateAttribute("value")).Value = geometricCenters[index, attribute].ToString(CultureInfo.InvariantCulture);
                }
            }

            return document;
        }
    }
}
