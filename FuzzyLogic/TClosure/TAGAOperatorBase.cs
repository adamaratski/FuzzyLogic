namespace FuzzyLogic.TClosure
{
    using System;
    using System.Collections.Generic;

    public abstract class TAGAOperatorBase
    {
        /// <summary>
        /// Gets the label.
        /// </summary>
        public abstract string Label { get; }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public abstract double GetValue(List<double> values);
    }
}
