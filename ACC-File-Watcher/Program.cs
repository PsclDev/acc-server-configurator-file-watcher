using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ACC_File_Watcher {
    class Program {
        private static Dictionary<string, string> ProjectVariables;
        
        static void Main() {
            LoadEnv();

            var dirToWatch = ProjectVariables["DIRECTORY_TO_WATCH"];
            var dirWatcher = new FileSystemWatcher($"{dirToWatch}");
            dirWatcher.Changed += dirWatcherOnChange;
            dirWatcher.Filter = "*.json";
            dirWatcher.IncludeSubdirectories = true;
            dirWatcher.EnableRaisingEvents = true;

            Console.WriteLine("Watching for file changes");
            Console.ReadLine();
        }
        
        private static void dirWatcherOnChange(object sender, FileSystemEventArgs e) {
            WriteColored(ConsoleColor.Blue, "File changes detected");
            foreach (var process in Process.GetProcessesByName(ProjectVariables["ProcessName"])) {
                WriteColored(ConsoleColor.Red, "Killing running Process");
                process.Kill();
            }

            Process accServer = new Process();
            accServer.StartInfo.WorkingDirectory = $"{ProjectVariables["ProcessPath"]}";
            var processExe = $"{ProjectVariables["ProcessPath"]}\\\\{ProjectVariables["ProcessName"]}.exe"; 
            accServer.StartInfo.FileName = processExe;
            accServer.StartInfo.UseShellExecute = true;
            accServer.Start();
            WriteColored(ConsoleColor.Green, "Started new Process of ACC Server");
        }
        
        static void LoadEnv() {
            var basePath = new FileInfo(System.Reflection.Assembly.GetEntryAssembly()?.Location!);
            var envPath = Path.Join(basePath.DirectoryName, ".env");

            ProjectVariables = DotEnvFile.DotEnvFile.LoadFile(envPath);
        }

        static void WriteColored(ConsoleColor foregroundColor, string text) {
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}