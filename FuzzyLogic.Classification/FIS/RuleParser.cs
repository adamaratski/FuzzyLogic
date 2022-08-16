using FuzzyLogic.Classification.FIS.Conditions;
using FuzzyLogic.Classification.FIS.Enums;
using FuzzyLogic.Classification.FIS.FussyRules;
using FuzzyLogic.Classification.FIS.Lexems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.Classification.FIS
{
    /// <summary>
    /// Class responsible for parsing
    /// </summary>
    internal class RuleParser<RuleType, OutputVariableType, OutputValueType>
        where OutputVariableType : class, INamedVariable
        where OutputValueType : class, INamedValue
        where RuleType : class, IParsableRule<OutputVariableType, OutputValueType>
    {
        static private IDictionary<String, Lexem> BuildLexemsList(IEnumerable<FuzzyVariable> input, IEnumerable<OutputVariableType> output)
        {
            var lexems = NameHelper.Keywords.Select(keyword => new KeywordLexem(keyword)).ToDictionary<KeywordLexem, String, Lexem>(keywordLexem => keywordLexem.Text, keywordLexem => keywordLexem);

            foreach (FuzzyVariable variable in input)
            {
                BuildLexemsList<FuzzyVariable, FuzzyTerm>(variable, true, lexems);
            }

            foreach (OutputVariableType var in output)
            {
                BuildLexemsList<OutputVariableType, OutputValueType>(var, false, lexems);
            }
            
            return lexems;
        }

        static private void BuildLexemsList<VariableType, ValueType>(VariableType var, bool input, IDictionary<String, Lexem> lexems)
            where VariableType : class, INamedVariable
            where ValueType : class, INamedValue
        {
            VarLexem<VariableType> varLexem = new VarLexem<VariableType>(var, input);
            lexems.Add(varLexem.Text, varLexem);
            foreach (ValueType term in var.Values)
            {
                TermLexem<ValueType> termLexem = new TermLexem<ValueType>(term, input);

                Lexem foundLexem = null;
                if (!lexems.TryGetValue(termLexem.Text, out foundLexem))
                {
                    // There are no lexems with the same text. Just insert new lexem.
                    lexems.Add(termLexem.Text, termLexem);
                }
                else
                {
                    if (foundLexem is IAltLexem)
                    {
                        // There can be more than one terms with the same name.
                        // TODO: But only if they belong to defferent variables.
                        IAltLexem foundTermLexem = (IAltLexem)foundLexem;
                        while (foundTermLexem.Alternative != null)
                        {
                            foundTermLexem = foundTermLexem.Alternative;
                        }
                        foundTermLexem.Alternative = termLexem;
                    }
                    else
                    {
                        // Only terms of different vatiables can have the same name
                        throw new System.Exception(string.Format("Found more than one lexems with the same name: {0}", termLexem.Text));
                    }
                }
            }
        }

        static private List<IExpression> ParseLexems(string rule, IDictionary<string, Lexem> lexems)
        {
            List<IExpression> expressions = new List<IExpression>();

            string[] words = rule.Split(' ');
            foreach (string word in words)
            {
                if (lexems.TryGetValue(word, out Lexem lexem))
                {
                    expressions.Add(lexem);
                }
                else
                {
                    throw new System.Exception(string.Format("Unknown identifier: {0}", word));
                }
            }

            return expressions;
        }

        static private List<IExpression> ExtractSingleCondidtions(IList<IExpression> conditionExpression, IList<FuzzyVariable> input, IDictionary<string, Lexem> lexems)
        {
            List<IExpression> copyExpressions = conditionExpression.ToList();
            List<IExpression> expressions = new List<IExpression>();

            while (copyExpressions.Count > 0)
            {
                if (copyExpressions[0] is VarLexem<FuzzyVariable>)
                {
                    // Parse variable
                    VarLexem<FuzzyVariable> varLexem = (VarLexem<FuzzyVariable>)copyExpressions[0];
                    if (copyExpressions.Count < 3)
                    {
                        throw new Exception(string.Format("Condition strated with '{0}' is incorrect.", varLexem.Text));
                    }

                    if (varLexem.Input == false)
                    {
                        throw new Exception("The variable in condition part must be an input variable.");
                    }

                    // Parse 'is' lexem
                    Lexem exprIs = (Lexem)copyExpressions[1];
                    if (exprIs != lexems["is"])
                    {
                        throw new Exception(string.Format("'is' keyword must go after {0} identifier.", varLexem.Text));
                    }


                    // Parse 'not' lexem (if exists)
                    int cur = 2;
                    bool not = false;
                    if (copyExpressions[cur] == lexems["not"])
                    {
                        not = true;
                        cur++;

                        if (copyExpressions.Count <= cur)
                        {
                            throw new Exception("Error at 'not' in condition part of the rule.");
                        }
                    }

                    // Parse hedge modifier (if exists)
                    HedgeType hedge = HedgeType.None;
                    if (copyExpressions[cur] == lexems["slightly"])
                    {
                        hedge = HedgeType.Slightly;
                    }
                    else if (copyExpressions[cur] == lexems["somewhat"])
                    {
                        hedge = HedgeType.Somewhat;
                    }
                    else if (copyExpressions[cur] == lexems["very"])
                    {
                        hedge = HedgeType.Very;
                    }
                    else if (copyExpressions[cur] == lexems["extremely"])
                    {
                        hedge = HedgeType.Extremely;
                    }

                    if (hedge != HedgeType.None)
                    {
                        cur++;

                        if (copyExpressions.Count <= cur)
                        {
                            throw new Exception(string.Format("Error at '{0}' in condition part of the rule.", hedge.ToString().ToLower()));
                        }
                    }

                    // Parse term
                    Lexem exprTerm = (Lexem)copyExpressions[cur];
                    if (!(exprTerm is IAltLexem))
                    {
                        throw new Exception(string.Format("Wrong identifier '{0}' in conditional part of the rule.", exprTerm.Text));
                    }

                    IAltLexem altLexem = (IAltLexem)exprTerm;
                    TermLexem<FuzzyTerm> termLexem = null;
                    do
                    {
                        if (!(altLexem is TermLexem<FuzzyTerm>))
                        {
                            continue;
                        }

                        termLexem = (TermLexem<FuzzyTerm>)altLexem;
                        if (!varLexem.Var.Values.Contains(termLexem.Term))
                        {
                            termLexem = null;
                            continue;
                        }
                    }
                    while ((altLexem = altLexem.Alternative) != null && termLexem == null);

                    if (termLexem == null)
                    {
                        throw new Exception(string.Format("Wrong identifier '{0}' in conditional part of the rule.", exprTerm.Text));
                    }

                    // Add new condition expression
                    FuzzyCondition condition = new FuzzyCondition(varLexem.Var, termLexem.Term, not, hedge);
                    expressions.Add(new ConditionExpression(copyExpressions.GetRange(0, cur + 1), condition));
                    copyExpressions.RemoveRange(0, cur + 1);
                }
                else
                {
                    IExpression expr = copyExpressions[0];
                    if (expr == lexems["and"] ||
                        expr == lexems["or"] ||
                        expr == lexems["("] ||
                        expr == lexems[")"])
                    {
                        expressions.Add(expr);
                        copyExpressions.RemoveAt(0);
                    }
                    else
                    {
                        Lexem unknownLexem = (Lexem)expr;
                        throw new Exception(string.Format("Lexem '{0}' found at the wrong place in condition part of the rule.", unknownLexem.Text));
                    }
                }
            }

            return expressions;
        }

        static private Condition ParseConditions(IList<IExpression> conditionExpression, IList<FuzzyVariable> input, IDictionary<string, Lexem> lexems)
        {
            // Extract single conditions
            List<IExpression> expressions = ExtractSingleCondidtions(conditionExpression, input, lexems);

            if (expressions.Count == 0)
            {
                throw new Exception("No valid conditions found in conditions part of the rule.");
            }

            ICondition cond = ParseConditionsRecurse(expressions, lexems);

            // Return conditions
            if (cond is Condition)
            {
                return (Condition)cond;
            }
            Condition condition = new Condition();
            return condition;
        }

        static private int FindPairBracket(List<IExpression> expressions, IDictionary<string, Lexem> lexems)
        {
            // Assume that '(' stands at first place
            int bracketsOpened = 1;
            int closeBracket = -1;
            for (int i = 1; i < expressions.Count; i++)
            {
                if (expressions[i] == lexems["("])
                {
                    bracketsOpened++;
                }
                else if (expressions[i] == lexems[")"])
                {
                    bracketsOpened--;
                    if (bracketsOpened == 0)
                    {
                        closeBracket = i;
                        break;
                    }
                }
            }

            return closeBracket;
        }

        static private ICondition ParseConditionsRecurse(List<IExpression> expressions, IDictionary<string, Lexem> lexems)
        {
            if (expressions.Count < 1)
            {
                throw new Exception("Empty condition found.");
            }

            if (expressions[0] == lexems["("] && FindPairBracket(expressions, lexems) == expressions.Count)
            {
                // Remove extra brackets
                return ParseConditionsRecurse(expressions.GetRange(1, expressions.Count - 2), lexems);
            }
            if (expressions.Count == 1 && expressions[0] is ConditionExpression)
            {
                // Return single conditions
                return ((ConditionExpression)expressions[0]).Condition;
            }
            // Parse list of one level conditions connected by or/and
            List<IExpression> copyExpressions = expressions.GetRange(0, expressions.Count);
            Condition condition = new Condition();
            bool setOrAnd = false;
            while (copyExpressions.Count > 0)
            {
                ICondition cond;
                if (copyExpressions[0] == lexems["("])
                {
                    // Find pair bracket
                    int closeBracket = FindPairBracket(copyExpressions, lexems);
                    if (closeBracket == -1)
                    {
                        throw new Exception("Parenthesis error.");
                    }

                    cond = ParseConditionsRecurse(copyExpressions.GetRange(1, closeBracket - 1), lexems);
                    copyExpressions.RemoveRange(0, closeBracket + 1);
                }
                else if (copyExpressions[0] is ConditionExpression)
                {
                    cond = ((ConditionExpression)copyExpressions[0]).Condition;
                    copyExpressions.RemoveAt(0);
                }
                else
                {
                    throw new ArgumentException(string.Format("Wrong expression in condition part at '{0}'"), copyExpressions[0].Text);
                }

                // And condition to the list
                condition.ConditionsList.Add(cond);

                if (copyExpressions.Count > 0)
                {
                    if (copyExpressions[0] == lexems["and"] || copyExpressions[0] == lexems["or"])
                    {
                        if (copyExpressions.Count < 2)
                        {
                            throw new Exception(string.Format("Error at {0} in condition part.", copyExpressions[0].Text));
                        }

                        // Set and/or for conditions list
                        OperatorType newOp = (copyExpressions[0] == lexems["and"]) ? OperatorType.And : OperatorType.Or;

                        if (setOrAnd)
                        {
                            if (condition.OperatorType != newOp)
                            {
                                throw new Exception("At the one nesting level cannot be mixed and/or operations.");
                            }
                        }
                        else
                        {
                            condition.OperatorType = newOp;
                            setOrAnd = true;
                        }
                        copyExpressions.RemoveAt(0);
                    }
                    else
                    {
                        throw new Exception(string.Format("{1} cannot goes after {0}", copyExpressions[0].Text, copyExpressions[1].Text));
                    }
                }
            }

            return condition;
        }

        static private SingleCondition<VariableType, ValueType> ParseConclusion<VariableType, ValueType>(IList<IExpression> conditionExpression, IList<VariableType> output, IDictionary<string, Lexem> lexems)
            where VariableType : class, INamedVariable
            where ValueType : class, INamedValue
        {
            List<IExpression> copyExpression = conditionExpression.ToList();
            // Remove extra brackets
            while (copyExpression.Count >= 2 && (copyExpression[0] == lexems["("] && copyExpression[conditionExpression.Count - 1] == lexems[")"]))
            {
                copyExpression = copyExpression.GetRange(1, copyExpression.Count - 2);
            }

            if (copyExpression.Count != 3)
            {
                throw new Exception("Conclusion part of the rule should be in form: 'variable is term'");
            }
            // Parse variable
            Lexem exprVariable = (Lexem)copyExpression[0];
            if (!(exprVariable is VarLexem<VariableType>))
            {
                throw new Exception(string.Format("Wrong identifier '{0}' in conclusion part of the rule.", exprVariable.Text));
            }

            VarLexem<VariableType> varLexem = (VarLexem<VariableType>)exprVariable;
            if (varLexem.Input)
            {
                throw new Exception("The variable in conclusion part must be an output variable.");
            }
            // Parse 'is' lexem
            Lexem exprIs = (Lexem)copyExpression[1];
            if (exprIs != lexems["is"])
            {
                throw new Exception(string.Format("'is' keyword must go after {0} identifier.", varLexem.Text));
            }
            // Parse term
            Lexem exprTerm = (Lexem)copyExpression[2];
            if (!(exprTerm is IAltLexem))
            {
                throw new Exception(string.Format("Wrong identifier '{0}' in conclusion part of the rule.", exprTerm.Text));
            }

            IAltLexem altLexem = (IAltLexem)exprTerm;
            TermLexem<ValueType> termLexem = null;
            do
            {
                if (!(altLexem is TermLexem<ValueType>))
                {
                    continue;
                }

                termLexem = (TermLexem<ValueType>)altLexem;
                if (!varLexem.Var.Values.Contains(termLexem.Term))
                {
                    termLexem = null;
                    continue;
                }
            }
            while ((altLexem = altLexem.Alternative) != null && termLexem == null);

            if (termLexem == null)
            {
                throw new Exception(string.Format("Wrong identifier '{0}' in conclusion part of the rule.", exprTerm.Text));
            }
            // Return fuzzy rule's conclusion
            return new SingleCondition<VariableType, ValueType>(varLexem.Var, termLexem.Term, false);
        }

        static internal RuleType Parse(string rule, RuleType emptyRule, IList<FuzzyVariable> input, IList<OutputVariableType> output)
        {
            if (rule.Length == 0)
            {
                throw new ArgumentException("Rule cannot be empty.");
            }
            // Surround brakes with spaces, remove double spaces
            System.Text.StringBuilder sb = new StringBuilder();
            foreach (char ch in rule)
            {
                if (ch == ')' || ch == '(')
                {
                    if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
                    {
                        // Do not duplicate spaces
                    }
                    else
                    {
                        sb.Append(' ');
                    }

                    sb.Append(ch);
                    sb.Append(' ');
                }
                else
                {
                    if (ch == ' ' && sb.Length > 0 && sb[sb.Length - 1] == ' ')
                    {
                        // Do not duplicate spaces
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
            }
            // Remove spaces
            string prepRule = sb.ToString().Trim();
            // Build lexems dictionary
            IDictionary<string, Lexem> lexemsDict = BuildLexemsList(input, output);
            // At first we parse lexems
            List<IExpression> expressions = ParseLexems(prepRule, lexemsDict);
            if (expressions.Count == 0)
            {
                throw new System.Exception("No valid identifiers found.");
            }
            // Find condition & conclusion parts part
            if (expressions[0] != lexemsDict["if"])
            {
                throw new System.Exception("'if' should be the first identifier.");
            }

            int thenIndex = -1;
            for (int i = 1; i < expressions.Count; i++)
            {
                if (expressions[i] == lexemsDict["then"])
                {
                    thenIndex = i;
                    break;
                }
            }

            if (thenIndex == -1)
            {
                throw new System.Exception("'then' identifier not found.");
            }

            int conditionLen = thenIndex - 1;
            if (conditionLen < 1)
            {
                throw new System.Exception("Condition part of the rule not found.");
            }

            int conclusionLen = expressions.Count - thenIndex - 1;
            if (conclusionLen < 1)
            {
                throw new System.Exception("Conclusion part of the rule not found.");
            }

            List<IExpression> conditionExpressions = expressions.GetRange(1, conditionLen);
            List<IExpression> conclusionExpressions = expressions.GetRange(thenIndex + 1, conclusionLen);

            Condition conditions = ParseConditions(conditionExpressions, input, lexemsDict);
            SingleCondition<OutputVariableType, OutputValueType> conclusion = ParseConclusion<OutputVariableType, OutputValueType>(conclusionExpressions, output, lexemsDict);

            emptyRule.Condition = conditions;
            emptyRule.Conclusion = conclusion;
            return emptyRule;
        }
    }
}
