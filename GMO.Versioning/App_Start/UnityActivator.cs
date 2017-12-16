[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(GMO.Versioning.UnityActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(GMO.Versioning.UnityActivator), "Shutdown")]

namespace GMO.Versioning
{
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary> 
    static class UnityActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary> 
        public static void Start()
        {
            // Register Types 
            UnityConfig.GetConfiguredContainer();
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary> 
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}
