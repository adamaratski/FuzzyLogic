using FuzzyLogic.Common;

namespace FuzzyLogic.Distances
{
    public static class Extensions
    {
        [Obsolete]
        public static Matrix GetRelationMatrixForRows(this Matrix matrix, Distance distance)
        {
            return distance.GetRelationMatrixForRows(matrix);
        }

        public static Matrix GetRelationMatrixForRows<T>(this Matrix matrix)
            where T : Distance
        {
            return Activator.CreateInstance<T>().GetRelationMatrixForRows(matrix);
        }
    }
}
