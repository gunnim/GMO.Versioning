[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(GMO.Versioning.LightInjectActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(GMO.Versioning.LightInjectActivator), "Shutdown")]

namespace GMO.Versioning
{
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary> 
    static class LightInjectActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary> 
        public static LightInject.ServiceContainer Start()
        {
            // Register Types 
            var container = new LightInject.ServiceContainer();
            LightInjectConfig.RegisterTypes(container);
            Settings.container = container;

            return container;
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary> 
        public static void Shutdown()
        {
            Settings.container?.Dispose();
        }
    }
}
