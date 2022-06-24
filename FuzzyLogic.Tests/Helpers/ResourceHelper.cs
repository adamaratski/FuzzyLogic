namespace FuzzyLogic.Tests.Helpers
{
    static class ResourceHelper
    {
        /// <summary>
        /// The get text.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string GetText(string resourceName)
        {
            using (StreamReader reader = new StreamReader(GetStream(resourceName)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// The get stream.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        private static Stream GetStream(string resourceName)
        {
            return typeof(ResourceHelper).Assembly.GetManifestResourceStream(resourceName);
        }
    }
}
