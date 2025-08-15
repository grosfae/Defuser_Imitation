using Defuser_Imitation.Properties;
using Octokit;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

namespace Defuser_Imitation.Components
{
    public sealed class GitHubUpdater
    {
        private static readonly string repoOwner = "grosfae";
        private static readonly string repoName = "Defuser_Imitation";
        public static readonly string currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.1.0.0";
        public static string? lastestGitHubVersion;
        private static string? updateFileUrl;
        private static string? updateFileName;

        private static readonly string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string appExecutableFileName = Assembly.GetExecutingAssembly().GetName().Name + ".exe";

        private static Dictionary<string, string> arguments = new Dictionary<string, string>();

        public static bool canUpdate;
        public GitHubUpdater()
        {
            lastestGitHubVersion = currentVersion;
            
        }
        public static void LoadCommandLineArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i += 2)
            {
                string arg = args[i].Replace("--", "");
                arguments.Add(arg, args[i + 1]);
            }
        }
        public static void CheckAndClearTempFolder()
        {
            LoadCommandLineArgs();
            string pathToTempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            if (!Directory.Exists(pathToTempFolder))
            {
                return;
            }
            string value = "";
            if (arguments.TryGetValue("needToClearTemp", out value) == true)
            {
                Settings.Default.NeedToClearTemp = bool.Parse(arguments["needToClearTemp"]);
                Settings.Default.Save();
            }
            if (Settings.Default.NeedToClearTemp == true)
            {
                var tempFilesArray = Directory.EnumerateFileSystemEntries(pathToTempFolder);
                if (tempFilesArray.Count() > 0)
                {
                    foreach (var entry in tempFilesArray)
                    {
                        if (Directory.Exists(entry))
                        {
                            Directory.Delete(entry, true);
                            continue;
                        }
                        if (File.Exists(entry))
                        {
                            File.Delete(entry);
                        }
                    }
                }
                Settings.Default.NeedToClearTemp = false;
                Settings.Default.Save();
            }
        }
        public static bool CheckInternetConnection()
        {
            try
            {
                Dns.GetHostEntry("ya.ru");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task CheckUpdate()
        {
            try
            {
                canUpdate = false;
                if (CheckInternetConnection() == false)
                {
                    return;
                }
                GitHubClient GitClient = new GitHubClient(new ProductHeaderValue("Defuser_Imitation"));
                var lastestRelease = await GitClient.Repository.Release.GetLatest(repoOwner, repoName);
                lastestGitHubVersion = lastestRelease.TagName;
                int compareVesionResult = currentVersion.CompareTo(lastestGitHubVersion);
                if (compareVesionResult < 0)
                {
                    updateFileName = Assembly.GetExecutingAssembly().GetName().Name + "-" + lastestRelease.TagName + ".zip";
                    updateFileUrl = lastestRelease.Assets.First(x => x.Name == updateFileName).BrowserDownloadUrl;
                    canUpdate = true;
                }
            }
            catch (Exception exception)
            {
                canUpdate = false;
                LoggerSerivce.Write(exception);
            }
        }
        public static void OpenUpdater()
        {
            try
            {
                if(canUpdate == false)
                {
                    return;
                }
                string arguments = string.Join(" ",
                $"--canUpdate {canUpdate}",
                $"--appExecutableFileName \"{appExecutableFileName}\"",
                $"--updateFileUrl \"{updateFileUrl}\"",
                $"--updateFileName \"{updateFileName}\"",
                $"--mainAppProcessId \"{Environment.ProcessId}\"",
                $"--mainAppProcessName \"{Process.GetCurrentProcess().ProcessName}\"",
                $"--appDirectory \"{appDirectory}"
                );
                string updaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Defuser_Imitation_Updater.exe");
                var updateProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = updaterPath,
                    Arguments = arguments,
                    UseShellExecute = false
                });
                App.Current.Dispatcher.Invoke(App.Current.Shutdown);
            }
            catch(Exception exception)
            {
                LoggerSerivce.Write(exception);
                MessageBox.Show(exception.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);  
            }
        }
    }
}
