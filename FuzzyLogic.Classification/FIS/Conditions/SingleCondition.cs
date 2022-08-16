namespace FuzzyLogic.Classification.FIS.Conditions
{
    /// <summary>
    /// Single condition
    /// </summary>
    public class SingleCondition<VariableType, ValueType> : ICondition
        where VariableType : class, INamedVariable
        where ValueType : class, INamedValue
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        internal SingleCondition()
        {
            Term = null;
            Not = false;
            Variable = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="variable">A linguistic variable to which the condition is related</param>
        /// <param name="term">A term in expression 'var is term'</param>
        internal SingleCondition(VariableType variable, ValueType term)
        {
            Not = false;
            Variable = variable;
            Term = term;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="var">A linguistic variable to which the condition is related</param>
        /// <param name="term">A term in expression 'var is term'</param>
        /// <param name="not">Does condition contain 'not'</param>
        internal SingleCondition(VariableType var, ValueType term, bool not)
            : this(var, term)
        {
            Not = not;
        }

        /// <summary>
        /// A linguistic variable to which the condition is related
        /// </summary>
        public VariableType Variable { get; set; }

        /// <summary>
        /// Is MF inverted
        /// </summary>
        public bool Not { get; set; }

        /// <summary>
        /// A term in expression 'var is term'
        /// </summary>
        //TODO: 'Term' is bad property name here
        public ValueType Term { get; set; }
    }
}
