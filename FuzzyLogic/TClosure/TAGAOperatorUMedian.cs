namespace FuzzyLogic.TClosure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TAGAOperatorUMedian : TAGAOperatorBase
    {
        /// <summary>
        /// Gets the label.
        /// </summary>
        public override string Label => "up median";

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override double GetValue(List<double> values)
        {
            if (values.Count % 2 == 0)
            {
                values.RemoveAt(0);
            }

            if (values.Count == 1)
            {
                return values.FirstOrDefault();
            }

            return values[(values.Count + 1) / 2 - 1];
        }
    }
}
