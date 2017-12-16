using log4net;
using System.IO;
using System.IO.Abstractions;
using System.Web;

namespace Versioning
{
    public class Versioning
    {
        public static Versioning Instance =>
            Settings.container.GetService(typeof(Versioning))
            as Versioning;

        public static string PathAndChecksum(string filePath)
        {
            return Instance.PathNChecksum(filePath);
        }

        HttpContextBase _httpCtx;
        IFileSystem _fs;
        Settings _settings;
        ILog _log;
        IFileWatcherService _fswSvc;
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

        string PathNChecksum(string filePath)
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
