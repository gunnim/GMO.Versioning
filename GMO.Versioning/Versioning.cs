using GMO.Versioning.Logging;
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
        private static readonly ILog Logger = LogProvider.For<Versioning>();

        /// <summary>
        /// Returns an instance of the <see cref="Versioning"/>
        /// </summary>
        public static Versioning Instance =>
            (Versioning) Settings.container.GetInstance(typeof(Versioning));

        /// <summary>
        /// Returns relative file path with file checksum as querystring.
        /// Caches the result and sets up a filesystem watcher to watch for changes.
        /// </summary>
        public static string AddChecksum(string filePath)
        {
            return Instance.PathAndChecksum(filePath);
        }

        readonly HttpContextBase _httpCtx;
        readonly IFileSystem _fs;
        readonly IFileWatcherService _fswSvc;
        readonly IMemoryCacheService _memoryCacheService;
        /// <summary>
        /// ctor
        /// </summary>
        public Versioning(
            HttpContextBase httpCtx,
            IFileSystem fileSystem,
            IFileWatcherService fswSvc,
            IMemoryCacheService memoryCacheService
        )
        {
            _httpCtx = httpCtx;
            _fs = fileSystem;
            _fswSvc = fswSvc;
            _memoryCacheService = memoryCacheService;
        }

        /// <summary>
        /// Returns relative file path with file checksum as querystring.
        /// Caches the result and sets up a filesystem watcher to watch for changes.
        /// </summary>
        private string PathAndChecksum(string filePath)
        {
            Logger.Debug($"Calculating PathAndChecksum for {filePath}");

            return $"{filePath}?v={AppendFileChecksum(filePath)}";
        }

        private string AppendFileChecksum(string filePath)
        {
            var fullFilePath = _httpCtx.Server.MapPath(filePath);

            if (_memoryCacheService.Get<FileSystemWatcherBase>(fullFilePath + "-fsw") == null)
            {
                // Ensure a file system watcher exists
                var fsw = _fswSvc.CreateFileSystemWatcher(fullFilePath);
                fsw.NotifyFilter = NotifyFilters.LastWrite;
                fsw.Changed += new FileSystemEventHandler(OnFileCreatedOrChanged);
                _memoryCacheService.Default[fullFilePath + "-fsw"] = fsw;
            }

            // Get or update
            return (string)(_memoryCacheService.Default[fullFilePath]
                ?? (_memoryCacheService.Default[fullFilePath] = CalculateFileHash(fullFilePath)));
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
            if (!FileIsReady(e.FullPath)) return; //first notification the file is arriving

            Logger.Info($"Change detected for {e.FullPath} - recalculating hash");
            _memoryCacheService.Default[e.FullPath] = CalculateFileHash(e.FullPath);
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
                using (var file = File.OpenRead(path))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                Logger.Debug($"IOException on OpenRead for path {path}, file not ready.");
                return false;
            }
        }
    }
}
