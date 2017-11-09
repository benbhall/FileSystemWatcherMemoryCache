using System;
using System.IO;
using System.Runtime.Caching;
using System.Runtime.InteropServices;

namespace FileSystemWatcherMemoryCache
{
    /// <summary>
    /// Recreating one of the common examples of FileSystemWatcher events firing twice
    /// Simply save changes to a text file in the given path when this is running
    /// </summary>
    internal class ExampleAttributesChangedFiringTwice
    {
        public ExampleAttributesChangedFiringTwice(string demoFolderPath)
        {
            var watcher = new FileSystemWatcher()
            {
                Path = demoFolderPath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.txt"
            };

            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Watching for writes to text files in folder: {demoFolderPath}");
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"Event {e.ChangeType} triggered for write to file {e.FullPath}");
        }
    }
}
