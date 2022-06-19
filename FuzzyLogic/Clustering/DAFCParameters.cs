namespace Math.Clustering
{
    using System;
    using FuzzyLogic.Criteries;
    using Math.Actions;

    public class DAFCParameters
    {
        public Double[,] Matrix { get; set; }

        public Int32? Count { get; set; }

        public Int32? IntersectionsLimit { get; set; }

        public Criterion Criterion { get; set; }

    }
}
