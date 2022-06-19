namespace FuzzyLogic.Distances
{
    using System;
    using FuzzyLogic.Common;

    public abstract class Distance : MatrixAction
    {
        public abstract double GetDistance(double[] arrayA, double[] arrayB);

        /// <summary>
        /// The get relation matrix for rows. Each row is one object. Columns are attributes
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public Matrix GetRelationMatrixForRows(Matrix matrix)
        {
            if (matrix.IsNullOrEmpty())
            {
                throw new ArgumentNullException("matrix");
            }

            Matrix result = new Matrix(matrix.Precision, new Region(0, 0, matrix.Region.Size.Height, matrix.Region.Size.Height));

            for (int x = 0; x < matrix.Region.Size.Height; x++)
            {
                for (int y = x; y < matrix.Region.Size.Height; y++)
                {
                    result[x, y] = this.GetDistance(matrix.GetRow(x), matrix.GetRow(y));

                    if (x != y)
                    {
                        result[y, x] = this.GetDistance(matrix.GetRow(x), matrix.GetRow(y));
                    }
                }
            }

            return result;
        }
    }
}
