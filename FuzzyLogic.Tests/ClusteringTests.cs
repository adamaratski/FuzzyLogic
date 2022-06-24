using FuzzyLogic.Common;
using FuzzyLogic.Criteries;
using FuzzyLogic.Distances;
using FuzzyLogic.Normirations;
using FuzzyLogic.Tests.Helpers;

namespace FuzzyLogic.Tests
{
    [TestClass]
    public class ClusteringTests
    {
        #region DAFC tests
        [DataTestMethod]
        [DataRow("FuzzyLogic.Tests.Data.Matrix.DAFC.The Anderson's iris data set.txt", new[] {72, 94, 97}, 0.96414903, 0.07663513, 1)]
        public void DAFC(string resourcePath, int[] cluster, double alfa, double criterion, int decisionsCount)
        {
            Matrix source = Matrix.FromText(ResourceHelper.GetText(resourcePath), new[] { "\t", " " });
            Matrix matrix = 1 - source.NormirateColumns<Normalization>().GetRelationMatrixForRows<EuclidianDistance>();
            matrix = matrix.Round(8);
            Clustering.DAFC dafc = new Clustering.DAFC();
            Int32 intersection;
            var decisions = dafc.Execute(matrix.ToArray(false), 3, out intersection);
            Double criterionValue;
            var optimal = dafc.FindOptimal<F1>(decisions, matrix.ToArray(false), out criterionValue);
            // decisions count
            Assert.AreEqual(optimal.Count(), decisionsCount);
            var result = optimal.First();
            // cluster
            Assert.IsTrue(cluster.SequenceEqual(result.Key));
            // alafa
            Assert.AreEqual(alfa, result.Value);
            // criterion
            Assert.AreEqual(System.Math.Round(criterionValue, 8), criterion);
        }

        #endregion
    }
}
