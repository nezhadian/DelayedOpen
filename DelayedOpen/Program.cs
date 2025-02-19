using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DelayedOpen
{
    internal class Program
    {
        private static string _filepath = "time.txt";
        public static void Main(string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                Console.WriteLine("Please specify a file path");
                CloseTimeout();
                return;
            }
            
            _filepath = Path.ChangeExtension(Path.GetFileName(args[0]), ".time.txt");
            
            Console.WriteLine("time file: " + _filepath);
            
            
            if (!IsRiderRunning())
            {
                Console.WriteLine("Rider is not running");
                CloseTimeout();
                return;
            }
            
            Console.WriteLine("Rider is running");


            var savedTime = GetSavedTime();
            var elapsed = DateTime.Now - savedTime;

            if (elapsed < TimeSpan.FromSeconds(30))
            {
                Console.WriteLine("Waiting...");
                CloseTimeout();
                return;
            }

            if (elapsed > TimeSpan.FromSeconds(90))
            {
                Console.WriteLine("90 seconds elapsed. reset timer...");
                SaveDateAndTime();
                CloseTimeout();
                return;
            }
            
            Console.WriteLine("Launching...");
            StartProgram(args[0]);
            SaveDateAndTime();
            CloseTimeout();

        }

        private static void StartProgram(string appPath)
        {
            if (!File.Exists(appPath))
            {
                Console.WriteLine("invalid app path ({0})",appPath);
                return;
            }

            try
            {
                Process.Start(appPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in starting app: " + ex.Message);
            }
        }

        private static DateTime GetSavedTime()
        {
            if (!File.Exists(_filepath))
            {
                SaveDateAndTime();
                return DateTime.Now;
            }
            
            return File.GetLastWriteTime(_filepath);
        }

        private static void SaveDateAndTime()
        {
            File.WriteAllText(_filepath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private static void CloseTimeout()
        {
            Thread.Sleep(2000);
        }

        static bool IsRiderRunning()
        {
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    if (proc.ProcessName.Equals("rider64", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
                catch
                {
                    // ignored
                }
            }
            return false;
        }
    }
}