using FuzzyLogic.Classification.FIS.Enums;

namespace FuzzyLogic.Classification.FIS.MembershipFunctions
{
    /// <summary>
    /// Composition of several membership functions represened as single membership function
    /// </summary>
    internal class CompositeMembershipFunction : IMembershipFunction
    {
        private readonly List<IMembershipFunction> _membershipFunctions = new List<IMembershipFunction>();
        private MembershipFunctionCompositionType _composType;

        public string Label => "CompositeMembershipFunction";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="composType">Membership functions composition type</param>
        public CompositeMembershipFunction(MembershipFunctionCompositionType composType)
        {
            _composType = composType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="composType">Membership functions composition type</param>
        /// <param name="membershipFunction1">Membership function 1</param>
        /// <param name="membershipFunction2">Membership function 2</param>
        public CompositeMembershipFunction(MembershipFunctionCompositionType composType, IMembershipFunction membershipFunction1, IMembershipFunction membershipFunction2)
            : this(composType)
        {
            _membershipFunctions.Add(membershipFunction1);
            _membershipFunctions.Add(membershipFunction2);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="composType">Membership functions composition type</param>
        /// <param name="membershipFunctions">Membership functions</param>
        public CompositeMembershipFunction(MembershipFunctionCompositionType composType, List<IMembershipFunction> membershipFunctions)
            : this(composType)
        {
            _membershipFunctions = membershipFunctions;
        }

        /// <summary>
        /// List of membership functions
        /// </summary>
        public List<IMembershipFunction> MembershipFunctions => _membershipFunctions;

        /// <summary>
        /// Membership functions composition type
        /// </summary>
        public MembershipFunctionCompositionType CompositionType
        {
            get => _composType;
            set => _composType = value;
        }

        /// <summary>
        /// Evaluate value of the membership function
        /// </summary>
        /// <param name="x">Argument (x axis value)</param>
        /// <returns></returns>
        public double GetValue(double x)
        {
            if (_membershipFunctions.Count == 0)
            {
                return 0.0;
            }

            if (_membershipFunctions.Count == 1)
            {
                return _membershipFunctions[0].GetValue(x);
            }

            double result = _membershipFunctions[0].GetValue(x);
            for (int index = 1; index < _membershipFunctions.Count; index++)
            {
                result = Compose(result, _membershipFunctions[index].GetValue(x));
            }
            return result;
        }

        double Compose(double value1, double value2)
        {
            switch (_composType)
            {
                case MembershipFunctionCompositionType.Max:
                    return System.Math.Max(value1, value2);
                case MembershipFunctionCompositionType.Min:
                    return System.Math.Min(value1, value2);
                case MembershipFunctionCompositionType.Prod:
                    return value1 * value2;
                case MembershipFunctionCompositionType.Sum:
                    return value1 + value2;
                default:
                    throw new Exception("Internal exception.");
            }
        }
    }
}
