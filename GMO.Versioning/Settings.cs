using CommonServiceLocator;

namespace GMO.Versioning
{
    /// <summary>
    /// Versioning specific settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Current dependency resolver instance
        /// </summary>
        internal static IServiceLocator container;
    }
}
