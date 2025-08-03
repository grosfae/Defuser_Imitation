using CommandLine;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace Defuser_Imitation_Updater.Components
{
    public class Updater
    {
        [Option('s', "appExecutableFileName", Required = true)]
        private string? _mainAppExecutableFileName { get; set; }
        [Option('s', "updateFileUrl", Required = true)]
        private string? _updateFileUrl { get; set; }

        [Option('s', "updateFileName", Required = true)]
        private string? _updateFileName { get; set; }

        [Option('t', "appDirectory", Required = true)]
        private string? _mainAppDirectory { get; set; }

        [Option('p', "processId", Required = false)]
        private int _parentProcessId { get; set; }
        [Option('p', "update", Required = false)]
        private bool _canUpdate { get; set; }

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
        public async Task DownloadAndInstallUpdate(ProgressBar progressBar, TextBlock textBlock)
        {
            if (_updateFileName == null || _canUpdate == false)
            {
                App.Current.Dispatcher.Invoke(() => progressBar.Visibility = Visibility.Collapsed);
                return;
            }
            string[] args = { _mainAppExecutableFileName, _updateFileUrl, _updateFileName, _mainAppDirectory, _parentProcessId.ToString(), _canUpdate.ToString()};
            Parser.Default.ParseArguments<Updater>(args)
            .WithParsed(x =>
            {
                if (x._parentProcessId > 0)
                {
                    var process = Process.GetProcessById(x._parentProcessId);
                    process.WaitForExit();
                }
            });
            try
            { 
                string tempZipPath = Path.Combine(Path.GetTempPath(), _updateFileName);
                string appDirectory = _mainAppDirectory;

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(_updateFileUrl);
                    using (var fileStream = new FileStream(tempZipPath, System.IO.FileMode.Create))
                        await response.Content.CopyToAsync(fileStream);
                }
                using (ZipArchive archive = ZipFile.OpenRead(tempZipPath))
                {
                    App.Current.Dispatcher.Invoke(() => progressBar.Maximum = archive.Entries.Count);
                    double percentUnit = 100 / archive.Entries.Count;
                    double generalProgress = 0;
                    foreach (var archiveEntry in archive.Entries)
                    {
                        string fullPath = Path.Combine(appDirectory, archiveEntry.FullName);
                        if (archiveEntry.Name == "")
                        {
                            Directory.CreateDirectory(fullPath);
                        }
                        else
                        {
                            archiveEntry.ExtractToFile(fullPath, true);  
                        }
                        generalProgress += percentUnit;
                        string updateText = $"Прогресс обновления: {generalProgress}% ({archiveEntry.Name})";
                        App.Current.Dispatcher.Invoke(() => textBlock.Text = updateText);
                        App.Current.Dispatcher.Invoke(() => progressBar.Value += 1);
                    }
                }                
                File.Delete(tempZipPath);
            }
            catch (Exception exception)
            {
                LoggerSerivce.Write(exception);
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                CloseUpdater();
            }
            CloseUpdater();
        }
        private void CloseUpdater()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = _mainAppExecutableFileName
            });
            App.Current.Shutdown();
        }
    }
}
