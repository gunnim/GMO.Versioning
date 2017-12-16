using log4net;
using System.IO;
using System.IO.Abstractions;
using System.Web;

namespace Versioning
{
    class FileWatcherService : IFileWatcherService
    {
        Settings _settings;
        ILog _log;
        HttpContextBase _httpCtx;
        IFileSystem _fs;
        public FileWatcherService(HttpContextBase httpCtx, IFileSystem fileSystem, Settings settings, ILogFactory logFac)
        {
            _httpCtx = httpCtx;
            _fs = fileSystem;
            _settings = settings;
            _log = logFac.GetLogger(typeof(FileWatcherService));
        }

        /// <summary>
        /// Recalculate checksum on file change
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
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
