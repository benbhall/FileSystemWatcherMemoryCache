using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace FileSystemWatcherMemoryCache
{
    /// <summary>
    /// Could also store event type
    /// </summary>
    internal class CacheItemValue
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int RetryCount { get; set; }
    }

    /// <summary>
    /// AddOrGetExisting a file event to MemoryCache, to block/swallow multiple events
    /// Actually 'handle' event inside callback for removal from cache on event expiring
    /// 
    /// Add to cache for a longer period i.e. 10 seconds and on the callback, attempt to get a file lock
    /// If file in use then add it back to the cache. Retry MaxRetries times or until lock on file is possible
    /// 
    /// Could make memory cache key unique on a composite of file name and event type easily enough
    /// </summary>/// 
    internal class BlockDelayAndHandleFileLockExample
    {
        private readonly MemoryCache _memCache;
        private readonly CacheItemPolicy _cacheItemPolicy;
        private const int CacheTimeSeconds = 10;
        private const int MaxRetries = 3;

        // Setup a FileSystemWatcher and cache item policy shared settings
        public BlockDelayAndHandleFileLockExample(string demoFolderPath)
        {
            _memCache = MemoryCache.Default;

            var watcher = new FileSystemWatcher()
            {
                Path = demoFolderPath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.txt"
            };

            _cacheItemPolicy = new CacheItemPolicy
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
            // Checking if expired, for a bit of future-proofing
            if (args.RemovedReason != CacheEntryRemovedReason.Expired) return;

            var cacheItemValue = (CacheItemValue)args.CacheItem.Value;

            if (cacheItemValue.RetryCount > MaxRetries) return;

            // If file is locked send, it back into the cache again
            // Could make the expiration period scale exponentially on retries
            if (IsFileLocked(cacheItemValue.FilePath))
            {
                cacheItemValue.RetryCount++;
                _cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(CacheTimeSeconds);
                
                _memCache.Add(cacheItemValue.FileName, cacheItemValue, _cacheItemPolicy); 
            }

            Console.WriteLine($"Now is a safe(ish) time to complete actions on file: {cacheItemValue.FileName}");
        }

        // Add file event to cache (won't add if already there so assured of only one occurance)
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            _cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(CacheTimeSeconds);

            var fileData = new CacheItemValue()
            {
                FilePath = e.FullPath,
                RetryCount = 0,
                FileName = e.Name
            };

            _memCache.AddOrGetExisting(e.Name, fileData, _cacheItemPolicy);
        }

        protected static bool IsFileLocked(string filePath)
        {
            FileStream stream = null;
            var file = new FileInfo(filePath);

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
            return false;
        }
    }
}
