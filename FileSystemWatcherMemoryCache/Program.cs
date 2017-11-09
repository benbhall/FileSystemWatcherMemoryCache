using System;

namespace FileSystemWatcherMemoryCache
{    
    internal class Program
    {      
        // Create demoPath then run
        // Copy files into the folder and watch result in console
        private static void Main(string[] args)
        {
            const string demoPath = @"c:\temp\demo";

            var demo = new ExampleAttributesChangedFiringTwice(demoPath);
            //var demo = new SimpleBlockAndDelayExample(demoPath);
            //var demo = new BlockDelayAndHandleFileLockExample(demoPath);

            Console.Read();
        }
    }
}
