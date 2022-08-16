namespace FuzzyLogic.Classification.FIS
{
    /// <summary>
    /// Named value of variable
    /// </summary>
    public abstract class NamedValue : INamedValue
    {
        private string _name;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the value</param>
        protected NamedValue(string name)
        {
            if (NameHelper.IsValidName(name))
            {
                throw new ArgumentException("Invalid term name.");
            }

            _name = name;
        }

        /// <summary>
        /// Name of the term
        /// </summary>
        public string Name
        {
            set
            {
                if (NameHelper.IsValidName(value))
                {
                    throw new ArgumentException("Invalid term name.");
                }

                _name = value;
            }
            get { return _name; }
        }

    }
}
