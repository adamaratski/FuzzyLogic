using FuzzyLogic.Classification.FIS.Conditions;

namespace FuzzyLogic.Classification.FIS.FussyRules
{
    /// <summary>
    /// Fuzzy rule for Mamdani fuzzy system
    /// </summary>
    public class MamdaniFuzzyRule : FuzzyRuleBase, IParsableRule<FuzzyVariable, FuzzyTerm>
    {
        /// <summary>
        /// Constructor. NOTE: a rule cannot be created directly, only via MamdaniFuzzySystem::EmptyRule or MamdaniFuzzySystem::ParseRule
        /// </summary>
        internal MamdaniFuzzyRule()
        {
            this.Conclusion = new SingleCondition<FuzzyVariable, FuzzyTerm>();
            this.Weight = 1;
        }

        /// <summary>
        /// Conclusion (THEN) part of the rule
        /// </summary>
        public SingleCondition<FuzzyVariable, FuzzyTerm> Conclusion { get; set; }

        /// <summary>
        /// Weight of the rule
        /// </summary>
        public Double Weight { get; set; }
    }
}
