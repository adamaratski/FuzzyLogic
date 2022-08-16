namespace FuzzyLogic.Classification.FIS
{
    /// <summary>
    /// This interface must be implemented by values in parsable rules
    /// </summary>
    public interface INamedValue
    {
        /// <summary>
        /// Name of the value
        /// </summary>
        string Name { get; set; }
    }
}
