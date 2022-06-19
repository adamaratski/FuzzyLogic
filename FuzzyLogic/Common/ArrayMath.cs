namespace FuzzyLogic.Common
{
    using System.Linq;

    public static class ArrayMath
    {
        #region SupportMethods
        /// <summary>
        /// subtraction of arrays
        /// </summary>
        public static double[] SubtractArray(double[] A, double[] B)
        {
            double[] ret = (double[])A.Clone();
            for (int i = 0; i < (A.Length > B.Length ? A.Length : B.Length); i++)
            {
                ret[i] -= B[i];
            }

            return ret;
        }

        /// <summary>
        /// subtraction of arrays
        /// </summary>
        public static int[] SubtractArray(int[] A, int[] B)
        {
            int[] ret = (int[])A.Clone();
            for (int i = 0; i < (A.Length > B.Length ? A.Length : B.Length); i++)
            {
                ret[i] -= B[i];
            }

            return ret;
        }

        /// <summary>
        /// division of arrays
        /// </summary>
        public static double[] DivideArrayConst(double[] array, double number)
        {
            double[] ret = (double[])array.Clone();
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] /= number;
            }

            return ret;
        }

        /// <summary>
        /// division of arrays
        /// </summary>
        public static int[] DivideArrayConst(int[] array, double number)
        {
            int[] ret = (int[])array.Clone();
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (int)(ret[i] / number);
            }

            return ret;
        }

        /// <summary>
        /// addition constant to arrays
        /// </summary>
        public static double[] SumArrayConst(double[] array, double number)
        {
            double[] ret = (double[])array.Clone();
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] += number;
            }

            return ret;
        }

        /// <summary>
        /// addition constant to arrays
        /// </summary>
        public static int[] SumArrayConst(int[] array, double number)
        {
            int[] ret = (int[])array.Clone();
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (int)(ret[i] + number);
            }

            return ret;
        }

        /// <summary>
        /// multiplication of arrays
        /// </summary>
        public static double[] MultiplicationArrayConst(double[] array, double number)
        {
            double[] ret = (double[])array.Clone();
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] *= number;
            }

            return ret;
        }

        /// <summary>
        /// multiplication of arrays
        /// </summary>
        public static int[] MultiplicationArrayConst(int[] array, double number)
        {
            int[] ret = (int[])array.Clone();
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (int)(ret[i] * number);
            }

            return ret;
        }

        /// <summary>
        /// comparing of arrays
        /// </summary>
        public static bool Equals(int[] A, int[] B)
        {
            if (A.Length != B.Length)
            {
                return false;
            }

            return !A.Where((t, i) => t != B[i]).Any();
        }

        #endregion
    }
}
