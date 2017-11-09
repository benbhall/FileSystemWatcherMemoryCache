using System;
using System.IO;
using System.Runtime.Caching;

namespace FileSystemWatcherMemoryCache
{
    /// <summary>
    /// AddOrGetExisting a file event to MemoryCache, to block/swallow multiple events
    /// Actually 'handle' event inside callback for removal from cache on event expiring
    /// </summary>
    internal class SimpleBlockAndDelayExample
    {
        private readonly MemoryCache _memCache;
        private readonly CacheItemPolicy _cacheItemPolicy;
        private const int CacheTimeMilliseconds= 500;

        // Setup a FileSystemWatcher and cache item policy shared settings
        public SimpleBlockAndDelayExample(string demoFolderPath)
        {
            _memCache = MemoryCache.Default;

            var watcher = new FileSystemWatcher()
            {
                Path = demoFolderPath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.txt"
            };

            _cacheItemPolicy = new CacheItemPolicy()
            {
                RemovedCallback = OnRemovedFromCache
            };

            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Watching for writes to text files in folder: {demoFolderPath}");
        }

        // Handle cache item expiring 
        private void OnRemovedFromCache(CacheEntryRemovedArguments args)
        {
            if (args.RemovedReason != CacheEntryRemovedReason.Expired) return;

            var e = (FileSystemEventArgs)args.CacheItem.Value;

            Console.WriteLine($"Let's now respond to the event {e.ChangeType} on {e.FullPath}");
        }

        // Add file event to cache (won't add if already there so assured of only one occurance)
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            _cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(CacheTimeMilliseconds);
            _memCache.AddOrGetExisting(e.Name, e, _cacheItemPolicy);
        }      
    }
}
