namespace FuzzyLogic.Common
{
    public struct Size
    {
        #region Static Fields

        /// <summary>
        /// The empty.
        /// </summary>
        public static readonly Size Empty;

        #endregion

        #region Fields

        /// <summary>
        /// The height.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// The width.
        /// </summary>
        public readonly int Width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Size"/> struct.
        /// </summary>
        static Size()
        {
            Size.Empty = new Size(0, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public Size(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        #endregion

        #region Public Methods and Operators

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
        /// Return true if equal
        /// </returns>
        public static Boolean operator ==(Size a, Size b)
        {
            return !(a.Width != b.Width || a.Height != b.Height);
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
        /// Return true if not equal
        /// </returns>
        public static Boolean operator !=(Size a, Size b)
        {
            return !(a.Width == b.Width && a.Height == b.Height);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(Size other)
        {
            return this.Width == other.Width && this.Height == other.Height;
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

            return obj is Size && this.Equals((Size)obj);
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
                return (this.Width * 397) ^ this.Height;
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("[{0} x {1}]", this.Width, this.Height);
        }

        #endregion
    }
}
