using FuzzyLogic.Classification.FIS.Conditions;

namespace FuzzyLogic.Classification.FIS.FussyRules
{
    /// <summary>
    /// Interface used by rule parser
    /// </summary>
    interface IParsableRule<OutputVariableType, OutputValueType>
        where OutputVariableType : class, INamedVariable
        where OutputValueType : class, INamedValue
    {
        /// <summary>
        /// Condition (IF) part of the rule
        /// </summary>
        Condition Condition { get; set; }

        /// <summary>
        /// Conclusion (THEN) part of the rule
        /// </summary>
        SingleCondition<OutputVariableType, OutputValueType> Conclusion { get; set; }
    }
}
