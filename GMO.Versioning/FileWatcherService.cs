using Common.Logging;
using System.IO;
using System.IO.Abstractions;
using System.Web;

namespace GMO.Versioning
{
    /// <summary>
    /// Recalculate checksum on file change
    /// </summary>
    class FileWatcherService : IFileWatcherService
    {
        Settings _settings;
        ILog _log;
        HttpContextBase _httpCtx;
        IFileSystem _fs;
        /// <summary>
        /// ctor
        /// </summary>
        public FileWatcherService(
            HttpContextBase httpCtx, 
            IFileSystem fileSystem, 
            Settings settings,
            ILogManager logMgr
        )
        {
            _httpCtx = httpCtx;
            _fs = fileSystem;
            _settings = settings;
            _log = logMgr.GetLogger<FileWatcherService>();
        }

        /// <summary>
        /// Creates a file system watcher for the given file path
        /// </summary>
        public FileSystemWatcherBase CreateFileSystemWatcher(string fullFilePath)
        {
            if (_settings.DetailedLogging)
            {
                _log.Info($"Created new filesystem watcher for {fullFilePath}");
            }

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
