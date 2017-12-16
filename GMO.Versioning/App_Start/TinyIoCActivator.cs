using CommonServiceLocator.TinyIoCAdapter;
using TinyIoC;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(GMO.Versioning.TinyIoCActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(GMO.Versioning.TinyIoCActivator), "Shutdown")]

namespace GMO.Versioning
{
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary> 
    static class TinyIoCActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary> 
        public static TinyIoCContainer Start()
        {
            // Register Types 
            var container = TinyIoCContainer.Current;
            TinyIoCConfig.RegisterTypes(container);
            Settings.container = new TinyIoCServiceLocator(container);

            return container;
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary> 
        public static void Shutdown()
        {
            var container = TinyIoCContainer.Current;
            container.Dispose();
        }
    }
}
