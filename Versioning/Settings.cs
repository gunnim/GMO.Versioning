using CommonServiceLocator;
using System.Configuration;

namespace Versioning
{
    public class Settings
    {
        /// <summary>
        /// Current dependency resolver instance
        /// </summary>
        internal static IServiceLocator container;

        public virtual bool DetailedLogging { get; } = ConfigurationManager.AppSettings["GMO.Versioning.DetailedLogging"].ConvertToBool();
    }
}
