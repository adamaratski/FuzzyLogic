namespace FuzzyLogic.Classification.FIS.MembershipFunctions
{
    public interface IMembershipFunction
    {
        string Label { get; }
        /// <summary>
        /// Evaluate value of the membership function
        /// </summary>
        /// <param name="x">Argument (x axis value)</param>
        /// <returns></returns>
        double GetValue(double x);
    }
}
