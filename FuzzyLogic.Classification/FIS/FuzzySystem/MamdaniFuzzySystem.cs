using FuzzyLogic.Classification.FIS.Enums;
using FuzzyLogic.Classification.FIS.FussyRules;
using FuzzyLogic.Classification.FIS.MembershipFunctions;

namespace FuzzyLogic.Classification.FIS.FuzzySystem
{
    public class MamdaniFuzzySystem : FuzzySystemBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MamdaniFuzzySystem()
        {
            ImplicationMethod = ImplicationMethod.Min;
            DefuzzificationMethod = DefuzzificationMethod.Centroid;
            AggregationMethod = AggregationMethod.Max;
            Output = new List<FuzzyVariable>();
            Rules = new List<MamdaniFuzzyRule>();
        }

        /// <summary>
        /// Output linguistic variables
        /// </summary>
        public IList<FuzzyVariable> Output { get; private set; }

        /// <summary>
        /// Fuzzy rules
        /// </summary>
        public IList<MamdaniFuzzyRule> Rules { get; private set; }

        /// <summary>
        /// Implication method
        /// </summary>
        public ImplicationMethod ImplicationMethod { get; set; }

        /// <summary>
        /// Aggregation method
        /// </summary>
        public AggregationMethod AggregationMethod { get; set; }

        /// <summary>
        /// Defuzzification method
        /// </summary>
        public DefuzzificationMethod DefuzzificationMethod { get; set; }

        /// <summary>
        /// Get output linguistic variable by its name
        /// </summary>
        /// <param name="name">Variable's name</param>
        /// <returns>Found variable</returns>
        public FuzzyVariable OutputByName(string name)
        {
            FuzzyVariable variable = Output.FirstOrDefault(var => var.Name == name);
            if (variable != null)
            {
                return variable;
            }

            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Create new empty rule
        /// </summary>
        /// <returns></returns>
        public MamdaniFuzzyRule EmptyRule()
        {
            return new MamdaniFuzzyRule();
        }

        /// <summary>
        /// Parse rule from the string
        /// </summary>
        /// <param name="rule">String containing the rule</param>
        /// <returns></returns>
        public MamdaniFuzzyRule ParseRule(string rule)
        {
            return RuleParser<MamdaniFuzzyRule, FuzzyVariable, FuzzyTerm>.Parse(rule, EmptyRule(), Input, Output);
        }

        /// <summary>
        /// Calculate output values
        /// </summary>
        /// <param name="inputValues">Input values (format: variable - value)</param>
        /// <returns>Output values (format: variable - value)</returns>
        public IDictionary<FuzzyVariable, double> Calculate(IDictionary<FuzzyVariable, double> inputValues)
        {
            // There should be one rule as minimum
            if (Rules.Count == 0)
            {
                throw new Exception("There should be one rule as minimum.");
            }

            // Fuzzification step
            IDictionary<FuzzyVariable, IDictionary<FuzzyTerm, double>> fuzzifiedInput = Fuzzify(inputValues);

            // Evaluate the conditions
            IDictionary<MamdaniFuzzyRule, double> evaluatedConditions = EvaluateConditions(fuzzifiedInput);

            // Do implication for each rule
            IDictionary<MamdaniFuzzyRule, IMembershipFunction> implicatedConclusions = Implicate(evaluatedConditions);

            // Aggrerate the results
            IDictionary<FuzzyVariable, IMembershipFunction> fuzzyResult = Aggregate(implicatedConclusions);

            // Defuzzify the result
            IDictionary<FuzzyVariable, double> result = Defuzzify(fuzzyResult);

            return result;
        }

        #region Intermidiate calculations

        /// <summary>
        /// Evaluate conditions 
        /// </summary>
        /// <param name="fuzzifiedInput">Input in fuzzified form</param>
        /// <returns>Result of evaluation</returns>
        public IDictionary<MamdaniFuzzyRule, double> EvaluateConditions(IDictionary<FuzzyVariable, IDictionary<FuzzyTerm, double>> fuzzifiedInput)
        {
            return Rules.ToDictionary(rule => rule, rule => this.EvaluateCondition(rule.Condition, fuzzifiedInput));
        }

        /// <summary>
        /// Implicate rule results
        /// </summary>
        /// <param name="conditions">Rule conditions</param>
        /// <returns>Implicated conclusion</returns>
        public IDictionary<MamdaniFuzzyRule, IMembershipFunction> Implicate(IDictionary<MamdaniFuzzyRule, double> conditions)
        {
            IDictionary<MamdaniFuzzyRule, IMembershipFunction> conclusions = new Dictionary<MamdaniFuzzyRule, IMembershipFunction>();
            foreach (MamdaniFuzzyRule rule in conditions.Keys)
            {
                MembershipFunctionCompositionType compType;
                switch (ImplicationMethod)
                {
                    case ImplicationMethod.Min:
                        compType = MembershipFunctionCompositionType.Min;
                        break;
                    case ImplicationMethod.Production:
                        compType = MembershipFunctionCompositionType.Prod;
                        break;
                    default:
                        throw new Exception("Internal error.");
                }

                CompositeMembershipFunction resultMembershipFunction = new CompositeMembershipFunction(compType, new ConstantMembershipFunction(conditions[rule]), rule.Conclusion.Term.MembershipFunction);
                conclusions.Add(rule, resultMembershipFunction);
            }

            return conclusions;
        }

        /// <summary>
        /// Aggregate results
        /// </summary>
        /// <param name="conclusions">Rules' results</param>
        /// <returns>Aggregated fuzzy result</returns>
        public IDictionary<FuzzyVariable, IMembershipFunction> Aggregate(IDictionary<MamdaniFuzzyRule, IMembershipFunction> conclusions)
        {
            IDictionary<FuzzyVariable, IMembershipFunction> fuzzyResult = new Dictionary<FuzzyVariable, IMembershipFunction>();
            foreach (FuzzyVariable variable in Output)
            {
                List<IMembershipFunction> membershipFunctions = (from rule in conclusions.Keys where rule.Conclusion.Variable == variable select conclusions[rule]).ToList();

                MembershipFunctionCompositionType composType;
                switch (AggregationMethod)
                {
                    case AggregationMethod.Max:
                        composType = MembershipFunctionCompositionType.Max;
                        break;
                    case AggregationMethod.Sum:
                        composType = MembershipFunctionCompositionType.Sum;
                        break;
                    default:
                        throw new Exception("Internal exception.");
                }

                fuzzyResult.Add(variable, new CompositeMembershipFunction(composType, membershipFunctions));
            }

            return fuzzyResult;
        }

        /// <summary>
        /// Calculate crisp result for each rule
        /// </summary>
        /// <param name="fuzzyResult"></param>
        /// <returns></returns>
        public IDictionary<FuzzyVariable, double> Defuzzify(IDictionary<FuzzyVariable, IMembershipFunction> fuzzyResult)
        {
            return fuzzyResult.Keys.ToDictionary(variable => variable, variable => Defuzzify(fuzzyResult[variable], variable.Min, variable.Max));
        }

        #endregion


        #region Helpers

        private double Defuzzify(IMembershipFunction membershipFunction, double min, double max)
        {
            switch (DefuzzificationMethod)
            {
                case DefuzzificationMethod.Centroid:
                    int iterations = 1000;
                    double step = (max - min) / iterations;

                    // Calculate a center of gravity as integral
                    double rightPoint = min;

                    double numerator = 0.0;
                    double denominator = 0.0;
                    for (int index = 0; index < iterations; index++)
                    {
                        double leftPoint = rightPoint;
                        double centerPoint = min + step * (index + 0.5);
                        rightPoint = min + step * (index + 1);

                        double leftFirstValue = membershipFunction.GetValue(leftPoint);
                        double centerFirstValue = membershipFunction.GetValue(centerPoint);
                        double rightFirstValue = membershipFunction.GetValue(rightPoint);

                        double leftSecondValue = leftPoint * leftFirstValue;
                        double centerSecondValue = centerPoint * centerFirstValue;
                        double rightSecondValue = rightPoint * rightFirstValue;

                        numerator += step * (leftSecondValue + 4 * centerSecondValue + rightSecondValue) / 3.0;
                        denominator += step * (leftFirstValue + 4 * centerFirstValue + rightFirstValue) / 3.0;
                    }

                    return numerator / denominator;
                case DefuzzificationMethod.AverageMaximum:
                    // TODO:
                    throw new NotSupportedException();
                case DefuzzificationMethod.Bisector:
                    // TODO:
                    throw new NotSupportedException();
                default:
                    throw new Exception("Internal exception.");
            }
        }

        #endregion
    }
}
