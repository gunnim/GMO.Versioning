using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;

namespace GMO.Versioning
{
    /// <summary> 
    /// Specifies the configuration for the main container. 
    /// </summary> 
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the type mappings with the container.</summary> 
        /// <param name="services">The container to configure.</param> 
        public static void AddVersioning(this IServiceCollection services)
        {
            services.AddScoped<Versioning>();

            services.AddScoped<IFileSystem, FileSystem>();
            services.AddScoped<IFileWatcherService, FileWatcherService>();
        }
    }
}
