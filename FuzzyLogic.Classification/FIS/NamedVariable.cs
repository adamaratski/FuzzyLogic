namespace FuzzyLogic.Classification.FIS
{
    /// <summary>
    /// Named variable
    /// </summary>
    public abstract class NamedVariable : INamedVariable
    {
        private string name = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the variable</param>
        protected NamedVariable(string name)
        {
            if (NameHelper.IsValidName(name))
            {
                throw new ArgumentException("Invalid variable name.");
            }

            this.name = name;
        }

        /// <summary>
        /// Variable name
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                if (NameHelper.IsValidName(value))
                {
                    throw new ArgumentException("Invalid variable name.");
                }
            }
        }

        /// <summary>
        /// Named values
        /// </summary>
        public abstract IList<INamedValue> Values { get; }

        public double Tag { get; set; }
    }
}
