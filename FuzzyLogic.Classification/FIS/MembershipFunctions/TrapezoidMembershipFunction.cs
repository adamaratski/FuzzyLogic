namespace FuzzyLogic.Classification.FIS.MembershipFunctions
{
    /// <summary>
    /// Trapezoid membership function
    /// </summary>
    public class TrapezoidMembershipFunction : IMembershipFunction
    {
        private double _x1, _x2, _x3, _x4;

        public string Label
        {
            get { return string.Format("TrapezoidMembershipFunction [{0:F04}, {1:F04}, {2:F04}, {3:F04}]", _x1, _x2, _x3, _x4); }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public TrapezoidMembershipFunction()
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x1">Point 1</param>
        /// <param name="x2">Point 2</param>
        /// <param name="x3">Point 3</param>
        /// <param name="x4">Point 4</param>
        public TrapezoidMembershipFunction(double x1, double x2, double x3, double x4)
        {
            if (!(x1 <= x2 && x2 <= x3 && x3 <= x4))
            {
                throw new ArgumentException();
            }

            _x1 = x1;
            _x2 = x2;
            _x3 = x3;
            _x4 = x4;
        }

        /// <summary>
        /// Point 1
        /// </summary>
        public double X1
        {
            get { return _x1; }
            set { _x1 = value; }
        }

        /// <summary>
        /// Point 2
        /// </summary>
        public double X2
        {
            get { return _x2; }
            set { _x2 = value; }
        }

        /// <summary>
        /// Point 3
        /// </summary>
        public double X3
        {
            get { return _x3; }
            set { _x3 = value; }
        }

        /// <summary>
        /// Point 4
        /// </summary>
        public double X4
        {
            get { return _x4; }
            set { _x4 = value; }
        }

        /// <summary>
        /// Evaluate value of the membership function
        /// </summary>
        /// <param name="x">Argument (x axis value)</param>
        /// <returns></returns>
        public double GetValue(double x)
        {
            double result = 0;

            if (x == _x1 && x == _x2)
                result = 1.0;
            else if (x == _x3 && x == _x4)
                result = 1.0;
            else if (x <= _x1 || x >= _x4)
                result = 0;
            else if (x >= _x2 && x <= _x3)
                result = 1;
            else if (x > _x1 && x < _x2)
                result = x / (_x2 - _x1) - _x1 / (_x2 - _x1);
            else
                result = -x / (_x4 - _x3) + _x4 / (_x4 - _x3);

            return result;
        }
    }
}
