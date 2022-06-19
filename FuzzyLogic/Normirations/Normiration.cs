namespace FuzzyLogic.Normirations
{
    using FuzzyLogic.Common;

    public abstract class Normiration : MatrixAction
    {
        public abstract Matrix Normirate(Matrix matrix);

        public abstract Matrix NormirateRows(Matrix matrix);

        public abstract Matrix NormirateColumns(Matrix matrix);
    }
}
