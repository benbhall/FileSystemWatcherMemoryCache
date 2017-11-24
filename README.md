# FileSystemWatcherMemoryCache

Lightweight example of using MemoryCache to solve the common OS/application problem with FileSystemWatcher events firing twice.

With some occurrences, we can safely take the final event e.g. with a NotifyFilters.LastWrite, it will fire one when the change is made and then again when the file has finished saving  - easy to recreate when editing a huge file. However, MS identify a broader issue and I wnated to explore a more predictable solution.

Change the demoPath folder location (or create it). Instantiate the class, drop a file in the folder and watch the console.

### ExampleAttributesChangedFiringTwice.cs

Example of multiple events when a file is edited.

### SimpleBlockAndDelayExamples.cs

Demonstrates using MemoryCache to swallow any events firing directly after the first (currently config @ 500ms).

The actual handling is then done on a callback as that cached event expires.

### BlockDelayAndHandleFileLockExample.cs

Adds additional logic to deal with the inveitable file lock situation if you plan to delete/alter etc. the file in any way. It does this by checking for a file lock on expiration and putting it back in the cache if there is a lock. Config value used for the number of retries before giving up.
