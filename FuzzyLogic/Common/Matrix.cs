#define Unckeck
#define Parallel

using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace FuzzyLogic.Common
{
    public class Matrix : ICloneable
    {
        #region Static Fields

        /// <summary>
        /// The empty.
        /// </summary>
        public static readonly Matrix Empty;

        #endregion

        #region Fields

        /// <summary>
        /// Gets the epsilon.
        /// </summary>
        public readonly Double Epsilon;

        /// <summary>
        /// The precision.
        /// </summary>
        public readonly Int32 Precision;

        /// <summary>
        /// The size.
        /// </summary>
        public readonly Region Region;

        /// <summary>
        /// The Matrix.
        /// </summary>
        private readonly Double[,] data;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Matrix"/> class.
        /// </summary>
        static Matrix()
        {
            Matrix.Empty = new Matrix(new Region(0, 0, 0, 0));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="precision">
        /// The precision.
        /// </param>
        /// <param name="region">
        /// The size.
        /// </param>
        public Matrix(Int32 precision, Region region)
        {
            this.Precision = precision;
            this.Epsilon = System.Math.Pow(10, -precision);
            this.Region = region;
            this.data = new Double[region.Size.Width, region.Size.Height];
        }

        public Matrix(Int32 precision, Region region, Double defaultValue)
            : this(precision, region)
        {
            this.Fill(region, defaultValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class. 
        /// Initializes a new instance of the <see cref="Matrix"/>. Default precision is 8 class.
        /// </summary>
        public Matrix()
            : this(new Region(0, 0, 0, 0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="region">
        /// The size.
        /// </param>
        public Matrix(Region region)
            : this(8, region)
        {
        }

        public Matrix(Region region, Double defaultValue)
            : this(8, region, defaultValue)
        {
        }

        public Matrix(List<Int32> source)
            : this(new Region(0, 0, source.Count, 1))
        {
            for (Int32 index = 0; index < source.Count; index++)
            {
                this[index, 0] = source[index];
            }
        }

        public Matrix(List<Double[]> source, Int32 precision)
            : this(precision, new Region(0, 0, source.First().Length, source.Count))
        {
            for (Int32 index = 0; index < source.Count; index++)
            {
                this.SetRow(source[index], index);
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public Double this[Int32 x, Int32 y]
        {
            get
            {
#if Unckeck
                return this.data[x, y];
#else
                if (x < this._size.Width && y < this._size.Height && x > -1 && y > -1)
                    return this._values[x, y];
                throw new IndexOutOfRangeException();
#endif
            }

            set
            {
#if Unckeck
                this.data[x, y] = value;
#else
                if (x < this._size.Width && y < this._size.Height && x > -1 && y > -1)
                    this._values[x, y] = value;
                else
                    throw new IndexOutOfRangeException();
#endif
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The from file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public static Matrix FromFile(String path, FileType type)
        {
            if (File.Exists(Path.GetFullPath(path)))
            {
                switch (type)
                {
                    case FileType.TabDelimetedText:
                        return Matrix.FromText(File.ReadAllText(path, Encoding.UTF8), new[] { "\t", " " });
                    case FileType.CommaDelimetedText:
                        return Matrix.FromText(File.ReadAllText(path, Encoding.UTF8), new[] { "," });
                }
            }

            return Matrix.Empty;
        }

        /// <summary>
        /// The from text.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public static Matrix FromText(String content, String[] delimeters)
        {
            // Set cultures format
            List<CultureInfo> cultureList = new List<CultureInfo>
                                                {
                                                    CultureInfo.GetCultureInfoByIetfLanguageTag("ru"),
                                                    CultureInfo.GetCultureInfoByIetfLanguageTag("en")
                                                };

            // Parse data
            String[] strings = content.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Int32 height = strings.Length;
            Int32 width = -1;
            String[][] data = new String[strings.Length][];
            for (Int32 row = 0; row < strings.LongLength; row++)
            {
                data[row] = strings[row].Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
                if (width == -1)
                {
                    width = data[row].Length;
                }
                else
                {
                    if (width != data[row].Length)
                    {
                        return Matrix.Empty;
                    }
                }
            }

            Matrix matrix = new Matrix(8, new Region(0, 0, width, height));

            foreach (CultureInfo culture in cultureList)
            {
                Boolean valid = true;
                try
                {
                    for (Int32 row = 0; row < height; row++)
                    {
                        for (Int32 column = 0; column < width; column++)
                        {
                            matrix[column, row] = Convert.ToDouble(data[row][column], culture);
                        }
                    }
                }
                catch (Exception)
                {
                    valid = false;
                }

                if (valid)
                {
                    break;
                }
            }

            return matrix;
        }

        /// <summary>
        /// The +.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// Return sum of matrixes
        /// </returns>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            Trace.Assert(a != null && b != null, "All operands should be not equal null");
            Trace.Assert(a.Precision == b.Precision, "Matrixes with different precision can not be compared.");
            Trace.Assert(a.Region == b.Region, "Matrixes with different size can not be processed.");

            Matrix c = new Matrix(a.Precision, a.Region);

            for (int x = 0; x < a.Region.Size.Width; x++)
            {
                for (int y = 0; y < a.Region.Size.Height; y++)
                {
                    c.data[x, y] = a.data[x, y] + b.data[x, y];
                }
            }

            return c;
        }

        /// <summary>
        /// The +.
        /// </summary>
        /// <param name="matrix">
        /// The a.
        /// </param>
        /// <param name="value">
        /// The b.
        /// </param>
        /// <returns>
        /// Add value to each element of Matrix
        /// </returns>
        public static Matrix operator +(Matrix matrix, Double value)
        {
            Matrix result = new Matrix(matrix.Precision, matrix.Region);
            for (int x = 0; x < matrix.Region.Size.Width; x++)
            {
                for (int y = 0; y < matrix.Region.Size.Height; y++)
                {
                    result.data[x, y] = matrix.data[x, y] + value;
                }
            }

            return matrix;
        }

        /// <summary>
        /// The +.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="matrix">
        /// The Matrix.
        /// </param>
        /// <returns>
        /// Return value + Matrix
        /// </returns>
        public static Matrix operator +(Double value, Matrix matrix)
        {
            Matrix result = new Matrix(matrix.Precision, matrix.Region);
            for (int x = 0; x < matrix.Region.Size.Width; x++)
            {
                for (int y = 0; y < matrix.Region.Size.Height; y++)
                {
                    result.data[x, y] = matrix.data[x, y] + value;
                }
            }

            return matrix;
        }

        /// <summary>
        /// The /.
        /// </summary>
        /// <param name="matrix">
        /// The a.
        /// </param>
        /// <param name="value">
        /// The x.
        /// </param>
        /// <returns>
        /// Return Matrix / value
        /// </returns>
        public static Matrix operator /(Matrix matrix, Double value)
        {
            return (1 / value) * matrix;
        }

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// Return true if matrixes differ lower then epsilon
        /// </returns>
        public static bool operator ==(Matrix a, Matrix b)
        {
            if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
            {
                if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b, null))
                {
                    return true;
                }

                return false;
            }

            Trace.Assert(a.Precision == b.Precision, "Matrixes with different precision can not be compared.");

            if (a.Region != b.Region)
            {
                return false;
            }

            for (int x = 0; x < a.Region.Size.Width; x++)
            {
                for (int y = 0; y < a.Region.Size.Height; y++)
                {
                    if (System.Math.Abs(a.data[x, y] - b.data[x, y]) > a.Epsilon)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The op_ explicit.
        /// </summary>
        /// <param name="matrix">
        /// The a.
        /// </param>
        /// <returns>
        /// Return Matrix as double[,]
        /// </returns>
        public static explicit operator Double[,](Matrix matrix)
        {
            Trace.Assert(matrix != null, "Matrix should be not equal null");
            Double[,] result = new Double[matrix.Region.Size.Width, matrix.Region.Size.Height];
            for (Int32 x = 0; x < matrix.Region.Size.Width; x++)
            {
                for (Int32 y = 0; y < matrix.Region.Size.Height; y++)
                {
                    result[x, y] = matrix.data[x, y];
                }
            }

            return result;
        }

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// Return true if matrixes differ more then epsilon
        /// </returns>
        public static bool operator !=(Matrix a, Matrix b)
        {
            if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
            {
                return true;
            }

            Trace.Assert(a.Precision == b.Precision, "Matrixes with different precision can not be compared.");

            if (a.Region != b.Region)
            {
                return true;
            }

            for (int x = 0; x < a.Region.Size.Width; x++)
            {
                for (int y = 0; y < a.Region.Size.Height; y++)
                {
                    if (System.Math.Abs(a.data[x, y] - b.data[x, y]) > a.Epsilon)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The *.
        /// </summary>
        /// <param name="matrix">
        /// The a.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// Return Matrix * value
        /// </returns>
        public static Matrix operator *(Matrix matrix, Double value)
        {
            Matrix result = new Matrix(matrix.Precision, matrix.Region);

            for (int x = 0; x < matrix.Region.Size.Width; x++)
            {
                for (int y = 0; y < matrix.Region.Size.Height; y++)
                {
                    result.data[x, y] = matrix.data[x, y] * value;
                }
            }

            return result;
        }

        /// <summary>
        /// The *.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="matrix">
        /// The a.
        /// </param>
        /// <returns>
        /// Return value * Matrix
        /// </returns>
        public static Matrix operator *(Double value, Matrix matrix)
        {
            Matrix result = new Matrix(matrix.Precision, matrix.Region);

            for (int x = 0; x < matrix.Region.Size.Width; x++)
            {
                for (int y = 0; y < matrix.Region.Size.Height; y++)
                {
                    result.data[x, y] = matrix.data[x, y] * value;
                }
            }

            return result;
        }

        /// <summary>
        /// The *.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// Return a * b
        /// </returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            Trace.Assert(a != null && b != null, "All operands should be not equal null");
            Trace.Assert(a.Precision == b.Precision, "Matrixes with different precision can not be processed.");
            Trace.Assert(a.Region.Size.Width == b.Region.Size.Height, "Width of a Matrix should be equal height of b Matrix.");

            Matrix c = new Matrix(a.Precision, new Region(0, 0, b.Region.Size.Width, a.Region.Size.Height));

            Func<Int32, Int32, Double> productFunc = delegate (Int32 x, Int32 y)
            {
                Double result = 0;
                for (Int32 index = 0; index < a.Region.Size.Width; index++)
                {
                    result += a.data[index, y] * b.data[x, index];
                }

                return result;
            };

#if Parallel
            Parallel.For(
                0,
                b.Region.Size.Width,
                x =>
                    {
#else
            for (int x = 0; x < b.Size.Width; x++)
            {
#endif
                for (int y = 0; y < a.Region.Size.Height; y++)
                {
                    c.data[x, y] = productFunc(x, y);
                }
#if Parallel
                    });
#else
            }
#endif

            return c;
        }

        /// <summary>
        /// The -.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// Return Matrix equals a - b
        /// </returns>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            Trace.Assert(a != null && b != null, "All operands should be not equal null");
            Trace.Assert(a.Precision == b.Precision, "Matrixes with different precision can not be compared.");
            Trace.Assert(a.Region == b.Region, "Matrixes with different size can not be processed.");

            Matrix c = new Matrix(a.Precision, a.Region);

            for (int x = 0; x < a.Region.Size.Width; x++)
            {
                for (int y = 0; y < a.Region.Size.Height; y++)
                {
                    c.data[x, y] = a.data[x, y] - b.data[x, y];
                }
            }

            return c;
        }

        /// <summary>
        /// The -.
        /// </summary>
        /// <param name="matrix">
        /// The a.
        /// </param>
        /// <param name="value">
        /// The b.
        /// </param>
        /// <returns>
        /// Return Matrix decreased on value
        /// </returns>
        public static Matrix operator -(Matrix matrix, Double value)
        {
            Matrix result = new Matrix(matrix.Precision, matrix.Region);
            for (int x = 0; x < matrix.Region.Size.Width; x++)
            {
                for (int y = 0; y < matrix.Region.Size.Height; y++)
                {
                    result.data[x, y] = matrix.data[x, y] - value;
                }
            }

            return result;
        }

        /// <summary>
        /// The -.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="matrix">
        /// The Matrix.
        /// </param>
        /// <returns>
        /// Return value - Matrix
        /// </returns>
        public static Matrix operator -(Double value, Matrix matrix)
        {
            Matrix result = new Matrix(matrix.Precision, matrix.Region);
            for (int x = 0; x < matrix.Region.Size.Width; x++)
            {
                for (int y = 0; y < matrix.Region.Size.Height; y++)
                {
                    result.data[x, y] = value - matrix.data[x, y];
                }
            }

            return result;
        }

        /// <summary>
        /// The -.
        /// </summary>
        /// <param name="matrix">
        /// The a.
        /// </param>
        /// <returns>
        /// Return minus 0 - Matrix
        /// </returns>
        public static Matrix operator -(Matrix matrix)
        {
            Matrix result = new Matrix(matrix.Precision, matrix.Region);
            for (int x = 0; x <= matrix.Region.Size.Width; x++)
            {
                for (int y = 0; y <= matrix.Region.Size.Height; y++)
                {
                    result.data[x, y] = -matrix.data[x, y];
                }
            }

            return matrix;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((Matrix)obj);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Epsilon.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Precision;
                hashCode = (hashCode * 397) ^ this.Region.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.data != null ? this.data.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool Equals(Matrix other)
        {
            return this.Epsilon.Equals(other.Epsilon) && this.Precision == other.Precision && this.Region.Equals(other.Region) && this.data == other.data;
        }

        /// <summary>
        /// The get row.
        /// </summary>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public Double[] GetRow(Int32 rowIndex)
        {
            Double[] row = new Double[this.Region.Size.Width];

            for (Int32 index = this.Region.Left; index <= this.Region.Right; index++)
            {
                row[index] = this.data[index, rowIndex];
            }

            return row;
        }

        /// <summary>
        /// The set row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public void SetRow(Double[] row, Int32 rowIndex)
        {
            if (row.Length != this.Region.Size.Width)
            {
                throw new ArgumentOutOfRangeException("Array is out of bound of matrix");
            }

            for (Int32 index = this.Region.Left; index <= this.Region.Right; index++)
            {
                this.data[index, rowIndex] = row[index];
            }
        }

        /// <summary>
        /// The get column.
        /// </summary>
        /// <param name="columnIndex">
        /// The column index.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public Double[] GetColumn(Int32 columnIndex)
        {
            Double[] column = new Double[this.Region.Size.Height];

            for (Int32 index = this.Region.Top; index <= this.Region.Bottom; index++)
            {
                column[index] = this.data[columnIndex, index];
            }

            return column;
        }

        /// <summary>
        /// The set column.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="columnIndex">
        /// The column index.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public void SetColumn(Double[] column, Int32 columnIndex)
        {
            if (column.Length != this.Region.Size.Height)
            {
                throw new ArgumentOutOfRangeException("Array is out of bound of matrix");
            }

            for (Int32 index = this.Region.Top; index <= this.Region.Bottom; index++)
            {
                this.data[columnIndex, index] = column[index];
            }
        }

        public Matrix Combine(Matrix matrix)
        {
            Matrix result = new Matrix(16, new Region(0, 0, this.Region.Size.Width * matrix.Region.Size.Width, this.Region.Size.Height + matrix.Region.Size.Height));

            for (Int32 column = 0; column < this.Region.Size.Width; column++)
            {
                for (Int32 row = 0; row < this.Region.Size.Height; row++)
                {
                    for (Int32 index = 0; index < matrix.Region.Size.Width; index++)
                    {
                        Int32 x = index * this.Region.Size.Width + column;
                        Int32 y = row;
                        result[x: x, y: y] = this[column, row];
                    }
                }
            }

            for (Int32 column = 0; column < matrix.Region.Size.Width; column++)
            {
                for (Int32 row = 0; row < matrix.Region.Size.Height; row++)
                {
                    for (Int32 index = 0; index < this.Region.Size.Width; index++)
                    {
                        Int32 x = index + column * this.Region.Size.Width;
                        Int32 y = row + this.Region.Size.Height;
                        result[x: x, y: y] = matrix[column, row];
                    }
                }
            }

            return result;
        }

        public void Fill(Region region, Double value)
        {
            for (Int32 x = region.Left; x <= region.Right; x++)
            {
                for (Int32 y = region.Top; y <= region.Bottom; y++)
                {
                    this[x, y] = value;
                }
            }
        }

        public Matrix Round(Int32 precision)
        {
            Matrix result = new Matrix(precision, this.Region);

            for (Int32 x = this.Region.Left; x <= this.Region.Right; x++)
            {
                for (Int32 y = this.Region.Top; y <= this.Region.Bottom; y++)
                {
                    result[x, y] = System.Math.Round(this[x, y], precision);
                }
            }

            return result;
        }

        #endregion

        #region IClonable

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            return this.Clone<Matrix>();
        }

        #endregion
    }
}
