using FuzzyLogic.Classification.FIS.MembershipFunctions;

namespace FuzzyLogic.Classification.FIS
{
    /// <summary>
    /// Linguistic term
    /// </summary>
    public class FuzzyTerm : NamedValue
    {
        private readonly IMembershipFunction _membershipFunction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Term name</param>
        /// <param name="membershipFunction">Membership function initially associated with the term</param>
        public FuzzyTerm(string name, IMembershipFunction membershipFunction)
            : base(name)
        {
            _membershipFunction = membershipFunction;
        }

        /// <summary>
        /// Membership function initially associated with the term
        /// </summary>
        public IMembershipFunction MembershipFunction
        {
            get { return _membershipFunction; }
        }
    }
}
