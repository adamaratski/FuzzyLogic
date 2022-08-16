namespace FuzzyLogic.Classification.FIS.Lexems
{
    public abstract class Lexem : IExpression
    {
        public abstract string Text { get; }
        public override string ToString() => this.Text;
    }
}
