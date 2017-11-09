# FileSystemWatcherMemoryCache
Lightweight example of using MemoryCache to solve the common OS/application problem with FileSystemWatcher events firing twice.

### ExampleAttributesChangedFiringTwice

Use this from Program.cs to see just one of example of multiple events. 

Change the demoPath folder location (or create it). Instantiate the class, drop a file in the folder and watch the console.

### SimpleBlockAndDelayExamples

As above to use this one.

Demonstrates using MemoryCache to swallow any events firing directly after the first (currently config @ 500ms).

The actual handling is then done on a callback as that cached event expires.


### BlockDelayAndHandleFileLockExample

Again, as above.

Adds additional logic to deal with the inveitable file lock situation if you plan to delete/alter etc. the file in any way. It does this by checking for a file lock on expiration and putting it back in the cache if there is a lock. Config value used for the number of retries before giving up.
