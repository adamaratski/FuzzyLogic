namespace FuzzyLogic.Normirations
{
    using System;
    using System.Linq;
    using FuzzyLogic.Common;

    /// <summary>
    /// The normalization.
    /// </summary>
    public class Normalization : Normiration
    {
        public override string Name => "Normalization";

        /// <summary>
        /// The normirate.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public override Matrix Normirate(Matrix matrix)
        {
            return matrix / matrix.Max();
        }

        /// <summary>
        /// The normirate rows.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public override Matrix NormirateRows(Matrix matrix)
        {
            Matrix result = new Matrix(matrix.Precision, matrix.Region);

            for (int index = matrix.Region.Top; index <= matrix.Region.Bottom; index++)
            {
                double[] row = matrix.GetRow(index);

                result.SetColumn(ArrayMath.DivideArrayConst(row, row.Max()), index);
            }

            return result;
        }

        /// <summary>
        /// The normirate columns.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public override Matrix NormirateColumns(Matrix matrix)
        {
            Matrix result = new Matrix(matrix.Precision, matrix.Region);

            for (int index = matrix.Region.Left; index <= matrix.Region.Right; index++)
            {
                double[] column = matrix.GetColumn(index);

                result.SetColumn(ArrayMath.DivideArrayConst(column, column.Max()), index);
            }

            return result;
        }
    }
}
