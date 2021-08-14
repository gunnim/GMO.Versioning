using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Abstractions;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace GMO.Versioning
{
    /// <summary>
    /// Returns relative file path with file checksum as querystring.
    /// Caches the result and sets up a filesystem watcher to watch for changes.
    /// </summary>
    public class Versioning
    {
        readonly IHostingEnvironment _env;
        readonly ILogger _logger;
        readonly IFileSystem _fs;
        readonly IFileWatcherService _fswSvc;
        readonly IMemoryCache _memoryCache;
        /// <summary>
        /// ctor
        /// </summary>
        public Versioning(
            IFileSystem fileSystem,
            IFileWatcherService fswSvc,
            IMemoryCache memoryCache,
            IHostingEnvironment env,
            ILogger logger)
        {
            _fs = fileSystem;
            _fswSvc = fswSvc;
            _memoryCache = memoryCache;
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Returns relative file path with file checksum as querystring.
        /// Caches the result and sets up a filesystem watcher to watch for changes.
        /// </summary>
        public virtual string PathAndChecksum(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("string.IsNullOrEmpty(filePath)", nameof(filePath));
            }

            _logger.LogDebug($"Calculating PathAndChecksum for {filePath}");

            return $"{filePath}?v={AppendFileChecksum(filePath)}";
        }

        private string AppendFileChecksum(string filePath)
        {
            var fullFilePath = _env.WebRootPath + filePath;

            if (_memoryCache.Get<FileSystemWatcherBase>(fullFilePath + "-fsw") == null)
            {
                // Ensure a file system watcher exists
                var fsw = _fswSvc.CreateFileSystemWatcher(fullFilePath);
                fsw.NotifyFilter = NotifyFilters.LastWrite;
                fsw.Changed += new FileSystemEventHandler(OnFileCreatedOrChanged);
                _memoryCache.Set(fullFilePath + "-fsw", fsw);
            }

            // Get or update
            return _memoryCache.GetOrCreate(
                fullFilePath,
                (entry) => CalculateFileHash(fullFilePath));
        }

        /// <summary>
        /// Creates SHA checksum of file
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        protected virtual string CalculateFileHash(string fullFilePath)
        {
            if (!_fs.File.Exists(fullFilePath))
            {
                throw new FileNotFoundException("File not found", fullFilePath);
            }

            using var fileStream = _fs.File.OpenRead(fullFilePath);
            return CryptoHelpers.GetSHASum(fileStream);
        }

        /// <summary>
        /// Recalculate checksum on file change
        /// </summary>
        private void OnFileCreatedOrChanged(object source, FileSystemEventArgs e)
        {
            if (!FileIsReady(e.FullPath)) return; //first notification the file is arriving

            _logger.LogInformation($"Change detected for {e.FullPath} - recalculating hash");
            _memoryCache.Set(e.FullPath, CalculateFileHash(e.FullPath));
        }

        /// <summary>
        /// https://www.intertech.com/Blog/avoiding-file-concurrency-using-system-io-filesystemwatcher/
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool FileIsReady(string path)
        {
            // One exception per file rather than several like in the polling pattern
            try
            {
                //If we can't open the file, it's still copying
                using var file = File.OpenRead(path);
                return true;
            }
            catch (IOException)
            {
                _logger.LogDebug($"IOException on OpenRead for path {path}, file not ready.");
                return false;
            }
        }
    }
}
