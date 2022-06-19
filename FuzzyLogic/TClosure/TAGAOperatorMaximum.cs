namespace FuzzyLogic.TClosure
{
    using System.Collections.Generic;
    using System.Linq;

    public class TAGAOperatorMaximum : TAGAOperatorBase
    {
        /// <summary>
        /// Gets the label.
        /// </summary>
        public override string Label => "maximum";

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public override double GetValue(List<double> values)
        {
            return values.Max();
        }
    }
}
