using System.IO.Abstractions;
using System.Web;
using TinyIoC;

namespace GMO.Versioning
{
    /// <summary> 
    /// Specifies the Unity configuration for the main container. 
    /// </summary> 
    static class TinyIoCConfig
    {
        /// <summary>Registers the type mappings with the Unity container.</summary> 
        /// <param name="container">The unity container to configure.</param> 
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to  
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks> 
        public static void RegisterTypes(TinyIoCContainer container)
        {
            container.Register<HttpContextBase>((c, p) => new HttpContextWrapper(HttpContext.Current));
            container.Register<Settings>(new Settings());
            container.Register<IFileSystem, FileSystem>();
            container.Register<IFileWatcherService, FileWatcherService>();
        }
    }
}
