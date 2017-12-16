using System.IO.Abstractions;

namespace Versioning
{
    public interface IFileWatcherService
    {
        FileSystemWatcherBase CreateFileSystemWatcher(string fullFilePath);
    }
}
