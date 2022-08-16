using FuzzyLogic.Classification.FIS.Conditions;
using FuzzyLogic.Classification.FIS.Enums;

namespace FuzzyLogic.Classification.FIS.FussyRules
{
    /// <summary>
    /// Implements common functionality of fuzzy rules
    /// </summary>
    public abstract class FuzzyRuleBase
    {
        Condition condition = new Condition();

        /// <summary>
        /// Condition (IF) part of the rule
        /// </summary>
        public Condition Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }

        /// <summary>
        /// Create a single condition
        /// </summary>
        /// <param name="var">A linguistic variable to which the condition is related</param>
        /// <param name="term">A term in expression 'var is term'</param>
        /// <returns>Generated condition</returns>
        public FuzzyCondition CreateCondition(FuzzyVariable var, FuzzyTerm term)
        {
            return new FuzzyCondition(var, term);
        }

        /// <summary>
        /// Create a single condition
        /// </summary>
        /// <param name="var">A linguistic variable to which the condition is related</param>
        /// <param name="term">A term in expression 'var is term'</param>
        /// <param name="not">Does condition contain 'not'</param>
        /// <returns>Generated condition</returns>
        public FuzzyCondition CreateCondition(FuzzyVariable var, FuzzyTerm term, bool not)
        {
            return new FuzzyCondition(var, term, not);
        }

        /// <summary>
        /// Create a single condition
        /// </summary>
        /// <param name="var">A linguistic variable to which the condition is related</param>
        /// <param name="term">A term in expression 'var is term'</param>
        /// <param name="not">Does condition contain 'not'</param>
        /// <param name="hedge">Hedge modifier</param>
        /// <returns>Generated condition</returns>
        public FuzzyCondition CreateCondition(FuzzyVariable var, FuzzyTerm term, bool not, HedgeType hedge)
        {
            return new FuzzyCondition(var, term, not, hedge);
        }
    }
}
