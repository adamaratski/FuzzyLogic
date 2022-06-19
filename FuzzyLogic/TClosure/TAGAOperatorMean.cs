namespace FuzzyLogic.TClosure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TAGAOperatorMean : TAGAOperatorBase
    {
        /// <summary>
        /// Gets the label.
        /// </summary>
        public override string Label => "mean";

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
            return values.Average();
        }
    }
}
