using FuzzyLogic.Common;

namespace FuzzyLogic.Normirations
{
    public static class Extensions
    {
        public static Matrix NormirateRows<T>(this Matrix matrix)
            where T : Normiration
        {
            return Activator.CreateInstance<T>().NormirateRows(matrix);
        }

        public static Matrix NormirateColumns<T>(this Matrix matrix)
            where T : Normiration
        {
            return Activator.CreateInstance<T>().NormirateColumns(matrix);
        }
    }
}
