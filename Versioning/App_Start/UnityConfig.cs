using System;
using System.IO.Abstractions;
using System.Web;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.ServiceLocation;

namespace Versioning
{
    /// <summary> 
    /// Specifies the Unity configuration for the main container. 
    /// </summary> 
    static class UnityConfig
    {
        #region Unity Container 
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            Settings.container = new UnityServiceLocator(container);
            return container;
        });

        /// <summary> 
        /// Gets the configured Unity container. 
        /// </summary> 
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary> 
        /// <param name="container">The unity container to configure.</param> 
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to  
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks> 
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<HttpContextBase>(new InjectionFactory(c => new HttpContextWrapper(HttpContext.Current)));
            container.RegisterType<ILogFactory, LogFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFileSystem, FileSystem>();
            container.RegisterType<Settings>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFileWatcherService, FileWatcherService>();
        }
    }
}
