# FileSystemWatcherMemoryCache

Lightweight example of using MemoryCache to solve the common OS/application problem with FileSystemWatcher events firing twice.

Change the demoPath folder location (or create it). Instantiate the class, drop a file in the folder and watch the console.

### ExampleAttributesChangedFiringTwice.cs

Example of multiple events when a file is edited.

### ExampleFileCreatedFiringTwice.cs

Example of multiple events when a new file is copied into a folder.

### SimpleBlockAndDelayExamples.cs

Demonstrates using MemoryCache to swallow any events firing directly after the first (currently config @ 500ms).

The actual handling is then done on a callback as that cached event expires.

### BlockDelayAndHandleFileLockExample.cs

Adds additional logic to deal with the inveitable file lock situation if you plan to delete/alter etc. the file in any way. It does this by checking for a file lock on expiration and putting it back in the cache if there is a lock. Config value used for the number of retries before giving up.
