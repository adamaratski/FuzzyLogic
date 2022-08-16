using FuzzyLogic.Classification.FIS.Enums;

namespace FuzzyLogic.Classification.FIS.Conditions
{
    /// <summary>
    /// Condition of fuzzy rule for the both Mamdani and Sugeno systems
    /// </summary>
    public class FuzzyCondition : SingleCondition<FuzzyVariable, FuzzyTerm>
    {
        public HedgeType Hedge { get; set; }

        public FuzzyCondition(FuzzyVariable var, FuzzyTerm term)
            : this(var, term, false)
        {
        }

        public FuzzyCondition(FuzzyVariable var, FuzzyTerm term, bool not)
            : this(var, term, not, HedgeType.None)
        {
        }

        public FuzzyCondition(FuzzyVariable var, FuzzyTerm term, bool not, HedgeType hedge)
            : base(var, term, not)
        {
            Hedge = hedge;
        }
    }
}
