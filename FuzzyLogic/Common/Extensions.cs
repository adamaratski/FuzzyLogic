using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.Common
{
    public static class Extensions
    {
        #region Matrix

        public static bool IsNullOrEmpty(this Matrix matrix)
        {
            return matrix == null || matrix == Matrix.Empty;
        }

        public static Matrix DistinctRows(this Matrix matrix, out Int32[] restoreIndexes)
        {
            Int32 rowsCount = matrix.Region.Size.Height;

            Int32[] indexes = new Int32[rowsCount];
            for (Int32 index = 0; index < indexes.Length; index++)
            {
                indexes[index] = -1;
            }

            List<Double[]> rows = new List<Double[]>();

            for (Int32 row = 0; row < rowsCount; row++)
            {
                if (indexes[row] != -1)
                {
                    continue;
                }

                Double[] currentRow = matrix.GetRow(row);

                rows.Add(currentRow);
                indexes[row] = rows.Count - 1;

                if (row == rowsCount - 1)
                {
                    break;
                }

                Parallel.For(row + 1, rowsCount, index =>
                {
                    if (currentRow.IsEquals(matrix.GetRow(index), matrix.Epsilon))
                    {
                        indexes[index] = rows.Count - 1;
                    }
                });
            }

            restoreIndexes = indexes;

            return new Matrix(rows, matrix.Precision);
        }

        public static Double Max(this Matrix matrix)
        {
            Double result = Double.MinValue;

            for (Int32 x = matrix.Region.Left; x <= matrix.Region.Right; x++)
            {
                for (Int32 y = matrix.Region.Top; y < matrix.Region.Bottom; y++)
                {
                    result = System.Math.Max(result, matrix[x, y]);
                }
            }

            return result;
        }

        public static Double Min(this Matrix matrix)
        {
            Double result = Double.MaxValue;

            for (Int32 x = matrix.Region.Left; x <= matrix.Region.Right; x++)
            {
                for (Int32 y = matrix.Region.Top; y < matrix.Region.Bottom; y++)
                {
                    result = System.Math.Min(result, matrix[x, y]);
                }
            }

            return result;
        }

        #endregion

        public static Boolean IsEquals(this Double[] a, Double[] b, Double epsilon)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            for (Int32 index = 0; index < a.Length; index++)
            {
                if (System.Math.Abs(a[index] - b[index]) > epsilon)
                {
                    return false;
                }
            }

            return true;
        }

        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static Matrix CreateEmpty(this Matrix matrix)
        {
            return new Matrix(matrix.Precision, matrix.Region);
        }

        public static Matrix ToMatrix(this Double[,] matrix)
        {
            Matrix result = new Matrix(new Region(0, 0, matrix.GetLength(0), matrix.GetLength(1)));

            for (Int32 x = 0; x < result.Region.Size.Width; x++)
            {
                for (Int32 y = 0; y < result.Region.Size.Height; y++)
                {
                    result[x, y] = matrix[x, y];
                }
            }

            return result;
        }

        public static Double[,] ToArray(this Matrix matrix, Boolean round)
        {
            Double[,] result = new double[matrix.Region.Size.Height, matrix.Region.Size.Width];

            for (Int32 x = 0; x < matrix.Region.Size.Width; x++)
            {
                for (Int32 y = 0; y < matrix.Region.Size.Height; y++)
                {
                    result[y, x] = round ? System.Math.Round(matrix[x, y], matrix.Precision) : matrix[x, y];
                }
            }

            return result;
        }

        public static MatrixType GetMatrixType(this Matrix matrix)
        {
            return MatrixType.Diagonal & MatrixType.HiTrianglular & MatrixType.LowTriangular & MatrixType.Ones & MatrixType.Scalar & MatrixType.Squared & MatrixType.Vector & MatrixType.Zeros;
        }

        public static Matrix Transpose(this Matrix source)
        {
            Matrix result = new Matrix(new Region(source.Region.Left, source.Region.Top, source.Region.Size.Width, source.Region.Size.Width));
            for (Int32 column = 0; column < source.Region.Size.Width; column++)
            {
                for (Int32 row = 0; row < source.Region.Size.Height; row++)
                {
                    result[row, column] = source[column, row];
                }
            }

            return result;
        }

        public static Double[,] Combine(this Double[,] source, Double[,] matrix)
        {
            Double[,] result = new Double[source.GetLength(0) * matrix.GetLength(0), source.GetLength(1) + matrix.GetLength(1)];

            for (Int32 column = 0; column < source.GetLength(0); column++)
            {
                for (Int32 row = 0; row < source.GetLength(1); row++)
                {
                    for (Int32 index = 0; index < matrix.GetLength(0); index++)
                    {
                        Int32 x = index * source.GetLength(0) + column;
                        Int32 y = row;
                        result[x, y] = source[column, row];
                    }
                }
            }

            for (Int32 column = 0; column < matrix.GetLength(0); column++)
            {
                for (Int32 row = 0; row < matrix.GetLength(1); row++)
                {
                    for (Int32 index = 0; index < source.GetLength(0); index++)
                    {
                        Int32 x = index + column * source.GetLength(0);
                        Int32 y = row + source.GetLength(1);
                        result[x, y] = matrix[column, row];
                    }
                }
            }

            return result;
        }

        public static Double[] GetRow(this Double[,] source, Int32 rowIndex)
        {
            Double[] result = new Double[source.GetLength(0)];

            for (Int32 index = 0; index < source.GetLength(0); index++)
            {
                result[index] = source[index, rowIndex];
            }

            return result;
        }

        public static Int32[] GetPowerVector(this Double[,] source, Int32[] vector, Double threshold)
        {
            Trace.Assert(source.GetLength(0) == source.GetLength(1));

            Int32 size = source.GetLength(0);

            Int32[] result = new Int32[vector.Length];
            for (Int32 index = 0; index < vector.Length; index++)
            {
                Int32 power = 0;
                for (Int32 row = 0; row < size; row++)
                {
                    if (source[vector[index], row] >= threshold)
                    {
                        power++;
                    }
                }

                result[index] = power;
            }

            return result;
        }

        public static Int32 GetIntersections(this Double[,] source, Int32[] vector, Double threshold)
        {
            Trace.Assert(source.GetLength(0) == source.GetLength(1));

            Int32 size = source.GetLength(0);

            Int32 result = 0;
            Boolean complite = false;
            for (Int32 x = 0; x < size; x++)
            {
                complite = false;
                for (Int32 index = 0; index < vector.GetLength(0); index++)
                {
                    if (source[x, vector[index]] >= threshold)
                    {
                        result = complite ? ++result : result;
                        complite = true;
                    }
                }
            }

            return result;
        }
    }
}
