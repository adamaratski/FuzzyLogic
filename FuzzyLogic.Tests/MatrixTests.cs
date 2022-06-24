using FuzzyLogic.Common;
using FuzzyLogic.Distances;
using FuzzyLogic.Normirations;
using FuzzyLogic.Tests.Helpers;

namespace FuzzyLogic.Tests
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void Composition()
        {
            Matrix a = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.Composition.a.txt"), new[] { "\t", " " });
            Matrix b = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.Composition.b.txt"), new[] { "\t", " " });
            Matrix c = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.Composition.c.txt"), new[] { "\t", " " });
            Assert.IsTrue(a * b == c);
        }

        [TestMethod]
        public void Normalization()
        {
            Matrix a = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.Normalization.a.txt"), new[] { "\t", " " });
            Matrix b = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.Normalization.b.txt"), new[] { "\t", " " });
            Assert.IsTrue(b == a.NormirateColumns<Normalization>());
        }

        [TestMethod]
        public void Unitarization()
        {
            Matrix a = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.Unitarization.a.txt"), new[] { "\t", " " });
            Matrix b = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.Unitarization.b.txt"), new[] { "\t", " " });
            Assert.IsTrue(b == a.NormirateColumns<Unitarization>());
        }

        [TestMethod]
        public void RelationMatrix()
        {
            Matrix a = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.RelationMatrix.a.txt"), new[] { "\t", " " });
            Matrix b = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.RelationMatrix.b.txt"), new[] { "\t", " " });
            Assert.IsTrue(b == a.NormirateColumns<Normalization>().GetRelationMatrixForRows<EuclidianDistance>());

            Matrix ñ = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.RelationMatrix.ñ.txt"), new[] { "\t", " " });
            Assert.IsTrue(ñ == a.NormirateColumns<Normalization>().GetRelationMatrixForRows<EuclidianNorm>());

            Matrix d = Matrix.FromText(ResourceHelper.GetText("FuzzyLogic.Tests.Data.Matrix.RelationMatrix.d.txt"), new[] { "\t", " " });
            Assert.IsTrue(d == a.NormirateColumns<Normalization>().GetRelationMatrixForRows<HammingDistance>());
        }
    }
}