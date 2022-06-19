namespace FuzzyLogic.Common
{
    public struct Region
    {
        #region Static Fields

        /// <summary>
        /// The empty.
        /// </summary>
        public static readonly Region Empty;

        #endregion

        #region Fields

        /// <summary>
        /// The left.
        /// </summary>
        public readonly int Left;

        /// <summary>
        /// The top.
        /// </summary>
        public readonly int Top;

        /// <summary>
        /// The right.
        /// </summary>
        public readonly int Right;

        /// <summary>
        /// The bottom.
        /// </summary>
        public readonly int Bottom;

        /// <summary>
        /// The size.
        /// </summary>
        public readonly Size Size;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Region"/> struct.
        /// </summary>
        static Region()
        {
            Region.Empty = new Region(0, 0, 0, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> struct.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="top">
        /// The top.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public Region(int left, int top, int width, int height)
        {
            this.Left = left;
            this.Top = top;
            this.Right = left + width - 1;
            this.Bottom = top + height - 1;
            this.Size = new Size(width, height);
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
        public static Boolean operator ==(Region a, Region b)
        {
            return !(a.Size != b.Size || a.Top != b.Top || a.Left != b.Top);
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
        /// </returns>
        public static Boolean operator !=(Region a, Region b)
        {
            return !(a.Size == b.Size && a.Top == b.Top && a.Left == b.Left);
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
        public bool Equals(Region other)
        {
            return this.Size == other.Size && this.Top == other.Top && this.Left == other.Left;
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

            return obj is Region && this.Equals((Region)obj);
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
                return this.Size.GetHashCode() * (this.Left * 397) ^ this.Top;
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
            return $"[{this.Left} : {this.Top} - {this.Size.Width} x {Size.Height}]";
        }

        #endregion
    }
}
