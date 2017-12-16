using CommonServiceLocator;
using System.Configuration;

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

        /// <summary>
        /// Enable detailed logging for Versioning.
        /// Control in appSettings with GMO.Versioning.DetailedLogging
        /// </summary>
        public virtual bool DetailedLogging { get; } = ConfigurationManager.AppSettings["GMO.Versioning.DetailedLogging"].ConvertToBool();
    }
}
