using System;
using System.IO;
using System.Runtime.Caching;
using System.Runtime.InteropServices;

namespace FileSystemWatcherMemoryCache
{
    /// <summary>
    /// Recreating one of the common examples of FileSystemWatcher events firing twice
    /// Simply copy a new file to the given path when this is running
    /// </summary>
    internal class ExampleFileCreatedFiringTwice
    {
        public ExampleFileCreatedFiringTwice(string demoFolderPath)
        {
            var watcher = new FileSystemWatcher()
            {
                Path = demoFolderPath,
                NotifyFilter = NotifyFilters.FileName,
                Filter = "*.*"
            };

            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Watching for writes to text files in folder: {demoFolderPath}");
        }

        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"Event {e.ChangeType} triggered for write to file {e.FullPath}");
        }
    }
}
