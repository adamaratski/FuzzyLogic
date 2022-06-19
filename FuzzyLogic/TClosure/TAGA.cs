namespace FuzzyLogic.TClosure
{
    using FuzzyLogic.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TAGA<T> : TAction
        where T : TAGAOperatorBase, new()
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="TAGA"/> class from being created.
        /// </summary>
        public TAGA()
        {
            Operator = new T();
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        public T Operator { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        public override string Label => $"TAGA {Operator.Label}";

        /// <summary>
        /// The process.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override double[,] Process(double[,] matrix)
        {
            Matrix source = matrix.ToMatrix();

            int size = source.Region.Size.Width;
            int[] s = new int[size];
            List<int> stack = new List<int>();
            for (int index = 0; index < s.Length; index++)
            {
                s[index] = index;
            }

            List<Tuple<int, int>> v = new List<Tuple<int, int>>();

            for (int y = 1; y < size; y++)
            {
                for (int x = 0; x < size - 1; x++)
                {
                    if (x < y)
                    {
                        v.Add(new Tuple<int, int>(x, y));
                    }
                }
            }

            while (true)
            {
                Tuple<int, int> k = v[0];
                double maxX = source[k.Item1, k.Item2];
                int m1 = 0;
                int m2 = 1;
                for (int index = 1; index < v.Count; index++)
                {
                    k = v[index];
                    if (source[k.Item1, k.Item2] > maxX)
                    {
                        maxX = source[k.Item1, k.Item2];
                        m1 = k.Item1;
                        m2 = k.Item2;
                    }
                }

                double lam = maxX;

                stack = new List<int>() { m1, m2 };
                List<int> p = new List<int>() { m1, m2 };

                int kk;

                while (true)
                {
                    kk = stack.Last();
                    stack.RemoveAt(stack.Count - 1);
                    IEnumerable<int> c = s.Where(item => p.Contains(item) == false);
                    foreach (int l in c)
                    {
                        if (Math.Abs(source[kk, l] - lam) < double.Epsilon)
                        {
                            stack.Add(l);
                            p.Add(l);
                        }
                    }

                    if (stack.Count == 0)
                    {
                        break;
                    }
                }

                Matrix pMatrix = new Matrix(new Region(0, 0, p.Count, 1));
                for (int index = 0; index < p.Count; index++)
                {
                    pMatrix[index, 0] = p[index];
                }

                Matrix t = pMatrix.Combine(pMatrix);

                List<Tuple<int, int>> m = new List<Tuple<int, int>>();

                for (int index = 0; index < t.Region.Size.Width; index++)
                {
                    if (t[index, 0] < t[index, 1] && source[(int)t[index, 0], (int)t[index, 1]] <= lam)
                    {
                        m.Add(new Tuple<int, int>((int)t[index, 0], (int)t[index, 1]));
                    }
                }

                List<double> e = new List<double>(m.Count);
                e.AddRange(m.Select(t1 => source[t1.Item1, t1.Item2]));
                e.Sort();

                double q = e.Count == 1 ? e.FirstOrDefault() : Operator.GetValue(e.ToList());

                foreach (Tuple<int, int> t1 in m)
                {
                    source[t1.Item1, t1.Item2] = q;
                    source[t1.Item2, t1.Item1] = q;
                }

                if (q == lam)
                {
                    v = v.Where(item => m.Exists(value => value.Item1 == item.Item1 && value.Item2 == item.Item2) == false).ToList();
                    List<int> K = s.Where(item => p.Exists(value => value == item) == false).ToList();

                    for (int index = 0; index < K.Count; index++)
                    {
                        kk = K[index];
                        var R = new Matrix(new Region(0, 0, 1, 1), K[index]).Combine(new Matrix(p)).Transpose();

                        List<double> f = new List<double>(R.Region.Size.Height);

                        for (int j = 0; j < R.Region.Size.Height; j++)
                        {
                            f.Add(source[(int)R[0, j], (int)R[1, j]]);
                        }

                        f.Sort();

                        double T = f.Max();
                        double S = Operator.GetValue(f.ToList());

                        if (T <= lam)
                        {
                            for (int l = 0; l < p.Count; l++)
                            {
                                source[kk, p[l]] = S;
                                source[p[l], kk] = S;
                            }
                        }
                    }
                }

                if (v.Count == 0)
                {
                    break;
                }
            }

            return source.ToArray(false);
        }
    }
}
