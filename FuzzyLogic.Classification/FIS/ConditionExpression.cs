using FuzzyLogic.Classification.FIS.Conditions;
using FuzzyLogic.Classification.FIS.Lexems;

namespace FuzzyLogic.Classification.FIS
{
    public class ConditionExpression : IExpression
    {
        public ConditionExpression(List<IExpression> expressions, FuzzyCondition condition)
        {
            this.Expressions = expressions;
            this.Condition = condition;
        }

        public List<IExpression> Expressions { get; set; }

        public FuzzyCondition Condition { get; set; }

        public string Text => string.Concat(this.Expressions.Select(x => x.ToString()));
    }
}
