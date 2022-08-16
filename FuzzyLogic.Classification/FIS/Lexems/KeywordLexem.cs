namespace FuzzyLogic.Classification.FIS.Lexems
{
    public class KeywordLexem : Lexem
    {
        private readonly string _name;

        public KeywordLexem(string name)
        {
            _name = name;
        }

        public override string Text
        {
            get { return _name; }
        }
    }
}
