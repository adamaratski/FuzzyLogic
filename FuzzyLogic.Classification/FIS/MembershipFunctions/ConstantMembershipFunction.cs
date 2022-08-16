namespace FuzzyLogic.Classification.FIS.MembershipFunctions
{
    /// <summary>
    /// Constant membership function
    /// </summary>
    public class ConstantMembershipFunction : IMembershipFunction
    {
        double _constValue;

        public string Label
        {
            get { return "ConstantMembershipFunction"; }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="constValue">Constant value</param>
        public ConstantMembershipFunction(double constValue)
        {
            if (constValue < 0.0 || constValue > 1.0)
            {
                throw new ArgumentException();
            }

            _constValue = constValue;
        }

        /// <summary>
        /// Evaluate value of the membership function
        /// </summary>
        /// <param name="x">Argument (x axis value)</param>
        /// <returns></returns>
        public double GetValue(double x)
        {
            return _constValue;
        }
    }
}
