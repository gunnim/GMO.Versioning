using System.IO.Abstractions;

namespace GMO.Versioning
{
    /// <summary>
    /// Creates a file system watcher for the given file path
    /// </summary>
    public interface IFileWatcherService
    {
        /// <summary>
        /// Creates a file system watcher for the given file path
        /// </summary>
        FileSystemWatcherBase CreateFileSystemWatcher(string fullFilePath);
    }
}
