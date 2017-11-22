using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace FileSystemWatcherMemoryCache
{


    /// <summary>
    /// Add files found to MemoryCache using filename as key using AddOrGetExisting so that only the first event
    /// will be added. Store in the cache for CacheTimeSeconds and fire a callback when the timeout expires
    /// Second fix is on file locks and retry mechanism
    /// BLOG could demo simple block then adding the file lock capabilities
    /// then discus alternative sliding?
    /// MINI BLOG on how timeouts work too remember
    /// Could make memory cache key unique on a composite of file name and event type easily enough
    /// </summary>
    internal class ExampleFuncIn
    {
        private readonly MemoryCache _memCache;
        private readonly CacheItemPolicy _cacheItemPolicy;
        private const int CacheTimeSeconds = 2;
        private const int MaxRetries = 3;
        private Func<int, string> _testFunc;

        public ExampleFuncIn(string demoFolderPath, Func<int,string> testFunc)
        {
            _testFunc = testFunc;

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

        private void OnRemovedFromCache(CacheEntryRemovedArguments args)
        {
            // Checking if expired, for a bit of future-proofing
            if (args.RemovedReason != CacheEntryRemovedReason.Expired) return;

            var fileData = (CacheItemValue)args.CacheItem.Value;

            if (fileData.RetryCount > MaxRetries) return;

            // If file is locked send, it back into the cache again
            // Could make the expiration period scale exponentially on retries
            if (IsFileLocked(fileData.FilePath))
            {
                fileData.RetryCount++;
                _cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(CacheTimeSeconds);
                
                _memCache.Add(fileData.FileName, fileData, _cacheItemPolicy); 
            }

            var test = _testFunc(MaxRetries);
            Console.WriteLine(test);

            Console.WriteLine($"Now is a safe(ish) time to complete actions on file: {fileData.FileName}");
        }

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
