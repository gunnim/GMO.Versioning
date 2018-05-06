using GMO.Versioning.LightInject;
using System.IO.Abstractions;
using System.Web;

namespace GMO.Versioning
{
    /// <summary> 
    /// Specifies the configuration for the main container. 
    /// </summary> 
    static class LightInjectConfig
    {
        /// <summary>Registers the type mappings with the container.</summary> 
        /// <param name="container">The container to configure.</param> 
        public static void RegisterTypes(LightInject.ServiceContainer container)
        {
            container.Register<HttpContextBase>(c => new HttpContextWrapper(HttpContext.Current), new PerRequestLifeTime());
            container.Register<Versioning>(new PerRequestLifeTime());

            container.Register<Settings>(new PerContainerLifetime());
            container.Register<IFileSystem, FileSystem>(new PerContainerLifetime());
            container.Register<IFileWatcherService, FileWatcherService>(new PerContainerLifetime());
            container.Register<IMemoryCacheService, MemoryCacheService>(new PerContainerLifetime());
        }
    }
}
