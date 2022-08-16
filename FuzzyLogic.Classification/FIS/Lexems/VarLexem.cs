namespace FuzzyLogic.Classification.FIS.Lexems
{
    public class VarLexem<VariableType> : Lexem
            where VariableType : class, INamedVariable
    {
        public VarLexem(VariableType var, bool input)
        {
            Var = var;
            Input = input;
        }

        public VariableType Var { get; set; }

        public override string Text => this.Var.Name;

        public bool Input { get; set; } = true;
    }
}
