using log4net;
using System.IO;
using System.IO.Abstractions;
using System.Web;

namespace GMO.Versioning
{
    /// <summary>
    /// Returns relative file path with file checksum as querystring.
    /// Caches the result and sets up a filesystem watcher to watch for changes.
    /// </summary>
    public class Versioning
    {
        /// <summary>
        /// Returns an instance of the <see cref="Versioning"/>
        /// </summary>
        public static Versioning Instance =>
            Settings.container.GetInstance<Versioning>();

        /// <summary>
        /// Returns relative file path with file checksum as querystring.
        /// Caches the result and sets up a filesystem watcher to watch for changes.
        /// </summary>
        public static string AddChecksum(string filePath)
        {
            return Instance.PathAndChecksum(filePath);
        }

        HttpContextBase _httpCtx;
        IFileSystem _fs;
        Settings _settings;
        ILog _log;
        IFileWatcherService _fswSvc;
        /// <summary>
        /// ctor
        /// </summary>
        public Versioning(
            HttpContextBase httpCtx,
            IFileSystem fileSystem,
            Settings settings,
            ILogFactory logFac,
            IFileWatcherService fswSvc
        )
        {
            _httpCtx = httpCtx;
            _fs = fileSystem;
            _settings = settings;
            _fswSvc = fswSvc;

            _log = logFac.GetLogger(typeof(Versioning));
        }

        /// <summary>
        /// Returns relative file path with file checksum as querystring.
        /// Caches the result and sets up a filesystem watcher to watch for changes.
        /// </summary>
        string PathAndChecksum(string filePath)
        {
            return $"{filePath}?v={AppendFileChecksum(filePath)}";
        }

        string AppendFileChecksum(string filePath)
        {
            var fullFilePath = _httpCtx.Server.MapPath(filePath);

            if (_httpCtx.Cache[fullFilePath + "-fsw"] == null)
            {
                // Ensure a file system watcher exists
                var fsw = _fswSvc.CreateFileSystemWatcher(fullFilePath);
                fsw.Created += new FileSystemEventHandler(OnFileCreatedOrChanged);
                fsw.Changed += new FileSystemEventHandler(OnFileCreatedOrChanged);
                _httpCtx.Cache.Insert(fullFilePath + "-fsw", fsw);
            }

            // Get or update
            return (string)(_httpCtx.Cache[fullFilePath]
                ?? (_httpCtx.Cache[fullFilePath] = CalculateFileHash(fullFilePath)));
        }

        /// <summary>
        /// Creates SHA checksum of file
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        private string CalculateFileHash(string fullFilePath)
        {
            if (!_fs.File.Exists(fullFilePath))
            {
                throw new FileNotFoundException("File not found", fullFilePath);
            }

            using (var fileStream = _fs.File.OpenRead(fullFilePath))
            {
                return CryptoHelpers.GetSHASum(fileStream);
            }
        }

        /// <summary>
        /// Recalculate checksum on file change
        /// </summary>
        private void OnFileCreatedOrChanged(object source, FileSystemEventArgs e)
        {
            if (_settings.DetailedLogging)
            {
                _log.Info($"Change detected for {e.FullPath} - recalculating hash");
            }

            var lastWriteTime = _fs.File.GetLastWriteTime(e.FullPath).ToFileTimeUtc();

            // During file modification multiple change events can fire.
            // We use the last write time of file to squash duplicate events
            var prevWriteTime = _httpCtx.Cache[e.FullPath + "-lastWriteTime"];

            if (prevWriteTime == null || (prevWriteTime != null && lastWriteTime != (long)prevWriteTime))
            {
                _httpCtx.Cache[e.FullPath + "-lastWriteTime"] = lastWriteTime;
                _httpCtx.Cache[e.FullPath] = CalculateFileHash(e.FullPath);
            }
        }
    }
}
