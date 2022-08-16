namespace FuzzyLogic.Classification.FIS.Lexems
{
    public class TermLexem<ValueType> : Lexem, IAltLexem
           where ValueType : class, INamedValue
    {
        public TermLexem(ValueType term, bool input)
        {
            this.Term = term;
            this.Input = input;
        }

        public bool Input { get; private set; }

        public ValueType Term { get; set; }

        public override string Text => this.Term?.Name;

        public IAltLexem Alternative { get; set; }
    }
}
