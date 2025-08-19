using Defuser_Imitation.Properties;
using Octokit;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Windows.Networking.Connectivity;

namespace Defuser_Imitation.Components
{
    public sealed class GitHubUpdater
    {
        private static readonly string repositoryOwner = Settings.Default.RepositoryOwner;
        private static readonly string repositoryName = Settings.Default.RepositoryName;
        private static string? updateFileUrl;
        private static string? updateFileName;

        private static readonly string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string appExecutableFileName = Assembly.GetExecutingAssembly().GetName().Name + ".exe";

        private static Dictionary<string, string> arguments = new Dictionary<string, string>();

        public static readonly string CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.1.0.0";
        public static string? LastestGitHubVersion;
        public static bool CanUpdate;
        public GitHubUpdater()
        {
            LastestGitHubVersion = CurrentVersion;
            
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
                ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                if (internetConnectionProfile != null)
                {
                    if(internetConnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {  
                LoggerSerivce.Write(exception);
                MessageBox.Show(exception.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public static async Task CheckUpdate()
        {
            try
            {
                CanUpdate = false;
                if (CheckInternetConnection() == false)
                {
                    return;
                }
                GitHubClient GitClient = new GitHubClient(new ProductHeaderValue("Defuser_Imitation"));
                var lastestRelease = await GitClient.Repository.Release.GetLatest(repositoryOwner, repositoryName);
                LastestGitHubVersion = lastestRelease.TagName;
                int compareVesionResult = CurrentVersion.CompareTo(LastestGitHubVersion);
                if (compareVesionResult < 0)
                {
                    updateFileName = Assembly.GetExecutingAssembly().GetName().Name + "-" + lastestRelease.TagName + ".zip";
                    updateFileUrl = lastestRelease.Assets.First(x => x.Name == updateFileName).BrowserDownloadUrl;
                    CanUpdate = true;
                }
            }
            catch (Exception exception)
            {
                CanUpdate = false;
                LoggerSerivce.Write(exception);
            }
        }
        public static void OpenUpdater()
        {
            try
            {
                if(CanUpdate == false)
                {
                    return;
                }
                string arguments = string.Join(" ",
                $"--canUpdate {CanUpdate}",
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
