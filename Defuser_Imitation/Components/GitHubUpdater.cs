using Octokit;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace Defuser_Imitation.Components
{
    public class GitHubUpdater
    {
        private readonly string _repoOwner;
        private readonly string _repoName;

        public readonly string _currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.1.0.0";
        public string _lastestGitHubVersion;
        
        public bool _canUpdate;
        private string? _appDirectory;
        private string? _updateFileUrl;
        private string? _updateFileName;
        private string? _updateZipFilePath;
        private string? _appExecutableFileName;


        public GitHubUpdater(string repoOwner, string repoName)
        {
            _repoOwner = repoOwner;
            _repoName = repoName;
            _lastestGitHubVersion = _currentVersion;
            _appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _appExecutableFileName = Assembly.GetEntryAssembly().Location;
        }
        public bool CheckInternetConnection()
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
        public async Task CheckUpdate()
        {
            _canUpdate = false;
            if (CheckInternetConnection() == false)
            {
                return;
            }
            string currentVersion = _currentVersion;
            GitHubClient GitClient = new(new ProductHeaderValue("Defuser_Imitation"));
            var lastestRelease = await GitClient.Repository.Release.GetLatest(_repoOwner, _repoName);
            string LatestGitHubVersion = lastestRelease.TagName;
            _lastestGitHubVersion = LatestGitHubVersion;

            if (Convert.ToDouble(currentVersion.Replace(".", ""), CultureInfo.InvariantCulture) < Convert.ToDouble(LatestGitHubVersion.Replace(".", ""), CultureInfo.InvariantCulture))
            {        
                _updateFileName = Assembly.GetExecutingAssembly().GetName().Name + "-" + lastestRelease.TagName + ".zip";
                _updateFileUrl = lastestRelease.Assets.First(x => x.Name == _updateFileName).BrowserDownloadUrl;
                _canUpdate = true;
            }
        }
        public void OpenUpdater()
        {
            string updaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Defuser_Imitation_Updater.exe");
            Process.Start(updaterPath, $"--update \"{true}\" --appExecutableFileName \"{_appExecutableFileName}\" --updateFileUrl \"{_updateFileUrl}\" --updateFileName \"{_updateFileName}\" --appDirectory \"{_appDirectory}\" --processId {Process.GetCurrentProcess().Id}");
        }
    }
}
