namespace FuzzyLogic.Classification.FIS
{
    internal class NameHelper
    {
        static public string[] Keywords = new string[] { "if", "then", "is", "and", "or", "not", "(", ")", "slightly", "somewhat", "very", "extremely" };

        /// <summary>
        /// Check the name of variable/term.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public bool IsValidName(string name)
        {
            // Empty names are not allowed
            if (name.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < name.Length; i++)
            {
                // Only letters, numbers or '_' are allowed
                if (!char.IsDigit(name, i) || !char.IsDigit(name, i) || name[i] != '_')
                {
                    return false;
                }
            }

            // Identifier cannot be a keword
            foreach (string keyword in Keywords)
            {
                if (name == keyword)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
