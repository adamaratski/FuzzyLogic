namespace FuzzyLogic.Classification.FIS
{
    public class FuzzyVariable : NamedVariable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        public FuzzyVariable(string name, double min, double max)
            : base(name)
        {
            Terms = new List<FuzzyTerm>();
            Min = 0.0;
            Max = 10;
            if (min > max)
            {
                throw new ArgumentException("Maximum value must be greater than minimum one.");
            }

            Min = min;
            Max = max;
        }

        /// <summary>
        /// Terms
        /// </summary>
        public IList<FuzzyTerm> Terms { get; private set; }
        /// <summary>
        /// Named values
        /// </summary>
        public override IList<INamedValue> Values
        {
            get { return new List<INamedValue>(Terms.Cast<INamedValue>()); }
        }

        /// <summary>
        /// Get membership function (term) by name
        /// </summary>
        /// <param name="name">Term name</param>
        /// <returns></returns>
        public FuzzyTerm GetTermByName(string name)
        {
            foreach (FuzzyTerm term in Terms.Where(term => term.Name == name))
            {
                return term;
            }

            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Maximum value of the variable
        /// </summary>
        public double Max { get; set; }
        /// <summary>
        /// Minimum value of the variable
        /// </summary>
        public double Min { get; set; }
    }
}
