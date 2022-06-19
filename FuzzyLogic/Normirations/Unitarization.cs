using System;
using System.Linq;
using FuzzyLogic.Common;

namespace FuzzyLogic.Normirations
{
    public class Unitarization : Normiration
    {
        public override string Name => "Unitarization";

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
            double max = matrix.Max();
            double min = matrix.Min();
            return (matrix - min) / (max - min);
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

            for (int rowIndex = matrix.Region.Top; rowIndex <= matrix.Region.Bottom; rowIndex++)
            {
                double[] row = matrix.GetRow(rowIndex);

                double max = row.Max();
                double min = row.Min();

                for (int index = 0; index < row.Length; index++)
                {
                    row[index] = (row[index] - min) / (max - min);
                }

                result.SetRow(row, rowIndex);
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

            for (int columnIndex = matrix.Region.Left; columnIndex <= matrix.Region.Right; columnIndex++)
            {
                double[] column = matrix.GetColumn(columnIndex);

                double max = column.Max();
                double min = column.Min();

                for (int index = 0; index < column.Length; index++)
                {
                    column[index] = (column[index] - min) / (max - min);
                }

                result.SetColumn(column, columnIndex);
            }

            return result;
        }
    }
}
