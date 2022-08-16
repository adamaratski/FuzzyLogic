using FuzzyLogic.Classification.FIS.Enums;

namespace FuzzyLogic.Classification.FIS.Conditions
{
    /// <summary>
    /// Several conditions linked by or/and operators
    /// </summary>
    public class Condition : ICondition, IEqualityComparer<Condition>, IEquatable<Condition>
    {
        /// <summary>
        /// Is MF inverted
        /// </summary>
        public bool Not { get; set; }

        /// <summary>
        /// Operator that links expressions (and/or)
        /// </summary>
        public OperatorType OperatorType { get; set; }

        /// <summary>
        /// A list of conditions (single or multiples)
        /// </summary>
        public List<ICondition> ConditionsList { get; private set; }

        public string Name => "";

        public Condition()
        {
            Not = false;
            OperatorType = OperatorType.And;
            ConditionsList = new List<ICondition>();
        }

        public bool Equals(Condition x, Condition y) => throw new NotImplementedException();

        public int GetHashCode(Condition obj) => throw new NotImplementedException();

        public bool Equals(Condition other) => throw new NotImplementedException();
    }
}
