namespace FuzzyLogic.TClosure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class TAction
    {
        static TAction()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            IEnumerable<TAction> tActions = assembly.GetTypes().Where(type => type != null && type.BaseType == typeof(TAction) && type.IsAbstract == false && type.IsGenericType == false).Select(type => Activator.CreateInstance(type) as TAction);
            IEnumerable<TAction> tagaActions = new TAction[]
                                                   {
                                                       new TAGA<TAGAOperatorMaximum>(),
                                                       new TAGA<TAGAOperatorMinimum>(),
                                                       new TAGA<TAGAOperatorMean>(),
                                                       new TAGA<TAGAOperatorMedian>(),
                                                       new TAGA<TAGAOperatorUMedian>(),
                                                       new TAGA<TAGAOperatorDMedian>()
                                                   };
            TActions = tActions.Union(tagaActions);
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        public abstract string Label { get; }

        /// <summary>
        /// The t actions.
        /// </summary>
        public static IEnumerable<TAction> TActions;

        /// <summary>
        /// The process.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public abstract double[,] Process(double[,] source);
    }
}
