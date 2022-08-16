namespace FuzzyLogic.Classification.FIS.MembershipFunctions
{
    /// <summary>
    /// Normal membership function
    /// </summary>
    public class NormalMembershipFunction : IMembershipFunction
    {
        double _b = 0.0, _sigma = 1.0;

        public string Label
        {
            get { return "NormalMembershipFunction"; }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public NormalMembershipFunction()
        { }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="b">Parameter b (center of MF)</param>
        /// <param name="sigma">Sigma</param>
        public NormalMembershipFunction(double b, double sigma)
        {
            _b = b;
            _sigma = sigma;
        }

        /// <summary>
        /// Parameter b (center of MF)
        /// </summary>
        public double B
        {
            get { return _b; }
            set { _b = value; }
        }

        /// <summary>
        /// Sigma
        /// </summary>
        public double Sigma
        {
            get { return _sigma; }
            set { _sigma = value; }
        }

        /// <summary>
        /// Evaluate value of the membership function
        /// </summary>
        /// <param name="x">Argument (x axis value)</param>
        /// <returns></returns>
        public double GetValue(double x)
        {
            return System.Math.Exp(-(x - _b) * (x - _b) / (2.0 * _sigma * _sigma));
        }
    }
}
