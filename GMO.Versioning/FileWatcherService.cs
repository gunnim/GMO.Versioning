using GMO.Versioning.Logging;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Abstractions;

namespace GMO.Versioning
{
    /// <summary>
    /// Recalculate checksum on file change
    /// </summary>
    class FileWatcherService : IFileWatcherService
    {
        readonly ILogger _logger;
        readonly IFileSystem _fs;
        /// <summary>
        /// ctor
        /// </summary>
        public FileWatcherService(
            IFileSystem fileSystem,
            ILogger logger)
        {
            _fs = fileSystem;
            _logger = logger;
        }

        /// <summary>
        /// Creates a file system watcher for the given file path
        /// </summary>
        public FileSystemWatcherBase CreateFileSystemWatcher(string fullFilePath)
        {
            _logger.LogInformation($"Created new filesystem watcher for {fullFilePath}");

            var dirName = _fs.Path.GetDirectoryName(fullFilePath);
            var fileName = _fs.Path.GetFileName(fullFilePath);

            var fsw = new FileSystemWatcher
            {
                Path = dirName,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = fileName,
                EnableRaisingEvents = true,
            };

            return new FileSystemWatcherWrapper(fsw);
        }
    }
}
