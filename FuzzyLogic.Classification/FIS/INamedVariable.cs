namespace FuzzyLogic.Classification.FIS
{
    /// <summary>
    /// This interface must be implemented by variable in parsable rules
    /// </summary>
    public interface INamedVariable
    {
        /// <summary>
        /// Name of the variable
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// List of values that belongs to the variable
        /// </summary>
        IList<INamedValue> Values { get; }
    }
}
