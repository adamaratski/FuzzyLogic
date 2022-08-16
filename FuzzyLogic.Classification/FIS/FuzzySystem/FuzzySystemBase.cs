using FuzzyLogic.Classification.FIS;
using FuzzyLogic.Classification.FIS.Conditions;
using FuzzyLogic.Classification.FIS.Enums;

namespace FuzzyLogic.Classification.FIS.FuzzySystem
{
    public class FuzzySystemBase
    {
        /// <summary>
        /// Input linguistic variables
        /// </summary>
        public IList<FuzzyVariable> Input { get; private set; }
        /// <summary>
        /// And method
        /// </summary>
        public AndMethod AndMethod { get; set; }
        /// <summary>
        /// Or method
        /// </summary>
        public OrMethod OrMethod { get; set; }
        /// <summary>
        /// Default constructor
        /// </summary>
        protected FuzzySystemBase()
        {
            AndMethod = AndMethod.Min;
            OrMethod = OrMethod.Max;
            Input = new List<FuzzyVariable>();
        }

        /// <summary>
        /// Get input linguistic variable by its name
        /// </summary>
        /// <param name="name">Variable's name</param>
        /// <returns>Found variable</returns>
        public FuzzyVariable InputByName(string name)
        {
            return Input.FirstOrDefault(variable => variable.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? throw new KeyNotFoundException();
        }

        #region Intermidiate calculations

        /// <summary>
        /// Fuzzify input
        /// </summary>
        /// <param name="inputValues"></param>
        /// <returns></returns>
        public IDictionary<FuzzyVariable, IDictionary<FuzzyTerm, double>> Fuzzify(IDictionary<FuzzyVariable, double> inputValues)
        {
            // Validate input
            if (!ValidateInputValues(inputValues, out string message))
            {
                throw new ArgumentException(message);
            }

            // Fill results list
            var result = new Dictionary<FuzzyVariable, IDictionary<FuzzyTerm, double>>();
            foreach (FuzzyVariable variable in Input)
            {
                IDictionary<FuzzyTerm, double> resultForVariables = variable.Terms.ToDictionary(term => term, term => term.MembershipFunction.GetValue(inputValues[variable]));
                result.Add(variable, resultForVariables);
            }

            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Evaluate fuzzy condition (or conditions)
        /// </summary>
        /// <param name="condition">Condition that should be evaluated</param>
        /// <param name="fuzzifiedInput">Input in fuzzified form</param>
        /// <returns>Result of evaluation</returns>
        protected double EvaluateCondition(ICondition condition, IDictionary<FuzzyVariable, IDictionary<FuzzyTerm, double>> fuzzifiedInput)
        {
            Double result = 0.0;
            switch (condition)
            {
                case Condition currentCondition:
                    if (currentCondition.ConditionsList.Count == 0)
                    {
                        throw new Exception("Inner exception.");
                    }

                    if (currentCondition.ConditionsList.Count == 1)
                    {
                        result = this.EvaluateCondition(currentCondition.ConditionsList[0], fuzzifiedInput);
                    }
                    else
                    {
                        result = this.EvaluateCondition(currentCondition.ConditionsList[0], fuzzifiedInput);
                        for (int index = 1; index < currentCondition.ConditionsList.Count; index++)
                        {
                            result = this.EvaluateConditionPair(result, EvaluateCondition(currentCondition.ConditionsList[index], fuzzifiedInput), currentCondition.OperatorType);
                        }
                    }

                    if (currentCondition.Not)
                    {
                        result = 1.0 - result;
                    }

                    return result;
                case FuzzyCondition currentCondition:
                    result = fuzzifiedInput[currentCondition.Variable][currentCondition.Term];

                    switch (currentCondition.Hedge)
                    {
                        case HedgeType.Slightly:
                            result = System.Math.Pow(result, 1.0 / 3.0); //Cube root
                            break;
                        case HedgeType.Somewhat:
                            result = System.Math.Sqrt(result);
                            break;
                        case HedgeType.Very:
                            result = result * result;
                            break;
                        case HedgeType.Extremely:
                            result = result * result * result;
                            break;
                    }

                    if (currentCondition.Not)
                    {
                        result = 1.0 - result;
                    }

                    return result;

                default:
                    throw new ArgumentException("Unknown condition type.");
            }
        }

        /// <summary>
        /// The evaluate condition pair.
        /// </summary>
        /// <param name="condition1">
        /// The condition 1.
        /// </param>
        /// <param name="condition2">
        /// The condition 2.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public double EvaluateConditionPair(double condition1, double condition2, OperatorType type)
        {
            if (type == OperatorType.And)
            {
                if (AndMethod == AndMethod.Min)
                {
                    return System.Math.Min(condition1, condition2);
                }

                if (AndMethod == AndMethod.Production)
                {
                    return condition1 * condition2;
                }

                throw new Exception("Internal error.");
            }

            if (type == OperatorType.Or)
            {
                if (OrMethod == OrMethod.Max)
                {
                    return System.Math.Max(condition1, condition2);
                }
                if (OrMethod == OrMethod.Probabilistic)
                {
                    return condition1 + condition2 - condition1 * condition2;
                }

                throw new Exception("Internal error.");
            }

            throw new Exception("Internal error.");
        }

        private bool ValidateInputValues(IDictionary<FuzzyVariable, double> inputValues, out string message)
        {
            message = string.Empty;
            if (inputValues.Count != Input.Count)
            {
                message = "Input values count is incorrect.";
                return false;
            }

            foreach (FuzzyVariable variable in Input)
            {
                if (inputValues.ContainsKey(variable))
                {
                    double value = inputValues[variable];
                    if (value < variable.Min || value > variable.Max)
                    {
                        message = string.Format("Vaulue for the '{0}' variable is out of range.", variable.Name);
                        return false;
                    }
                }
                else
                {
                    message = string.Format("Vaulue for the '{0}' variable does not present.", variable.Name);
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
