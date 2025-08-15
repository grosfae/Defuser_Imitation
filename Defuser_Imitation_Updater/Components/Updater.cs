using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace Defuser_Imitation_Updater.Components
{
    public sealed class Updater
    {
        private static Dictionary<string, string> arguments = new Dictionary<string, string>();
        private static void LoadCommandLineArgs(Dictionary <string, string> argumentsDictionary)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i += 2)
            {
                string arg = args[i].Replace("--", "");
                argumentsDictionary.Add(arg, args[i + 1]);
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
        private static void GetMainAppProcess()
        {
            try
            {
                Process mainAppProcess = Process.GetProcessById(int.Parse(arguments["mainAppProcessId"]));
                if (mainAppProcess != null && mainAppProcess.ProcessName == arguments["mainAppProcessName"])
                {
                    MessageBox.Show(mainAppProcess.ProcessName);
                    mainAppProcess.WaitForExit();
                }
            }
            catch (Exception exteption)
            {
                LoggerSerivce.Write(exteption);
            }
        }
        private static bool CheckUpdateAvailable()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(arguments["canUpdate"]) || bool.Parse(arguments["canUpdate"]) == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        private static int ConvertBytesToMegabytes(long? bytes)
        {
            if (bytes != null)
            {
                return (int)(bytes / 1024f / 1024f);
            }
            else
            {
                return 0;
            }
        }
        private static string FormatSpeed(double bytesPerSecond)
        {
            if (bytesPerSecond >= 1024 * 1024) // > 1 МБ/с
                return $"{bytesPerSecond / (1024 * 1024):F2} МБ/с";
            else if (bytesPerSecond >= 1024) // > 1 КБ/с
                return $"{bytesPerSecond / 1024:F2} КБ/с";
            else
                return $"{bytesPerSecond:F2} Б/с";
        }
        private static async Task DownloadFileUpdateFileAsync(string fileUrl, string destinationPath, ProgressBar progressBar, TextBlock progressTextBlock)
        {
            using (HttpClient client = new HttpClient())
            {
                using (var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, fileUrl)))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return;
                    }
                    long? totalBytes = response.Content.Headers.ContentLength;
                    int? totalMegabytes = ConvertBytesToMegabytes(totalBytes);

                    using (var downloadStream = await client.GetStreamAsync(fileUrl))
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        long totalBytesRead = 0;

                        Stopwatch stopwatch = Stopwatch.StartNew();
                        long lastBytesRead = 0;
                        double lastSpeedUpdateTime = 0;
                        string downloadSpeedFromated = "";
                        while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;

                            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                            if (elapsedSeconds - lastSpeedUpdateTime >= 0.3)
                            {
                                double downloadSpeed = (totalBytesRead - lastBytesRead) / (elapsedSeconds - lastSpeedUpdateTime);
                                lastBytesRead = totalBytesRead;
                                lastSpeedUpdateTime = elapsedSeconds;
                                downloadSpeedFromated = FormatSpeed(downloadSpeed);
                            }
                            if (totalMegabytes.HasValue)
                            {
                                int totalMegabytesRead = ConvertBytesToMegabytes(totalBytesRead);
                                int progressPercentage = (int)((double)totalMegabytesRead / totalMegabytes.Value * 100);
                                App.Current.Dispatcher.Invoke(() => progressTextBlock.Text = $"({downloadSpeedFromated}) {totalMegabytesRead} / {totalMegabytes} МБ");
                                App.Current.Dispatcher.Invoke(() => progressBar.Value = progressPercentage);
                            }
                        }
                    }
                }
            }
        }
        public static void CheckAndClearTempFolder(string pathToTempFolder)
        {
            if (!Directory.Exists(pathToTempFolder))
            {
                return;
            }
            var tempFilesArray = Directory.EnumerateFileSystemEntries(pathToTempFolder);
            if (tempFilesArray.Count() > 0)
            {

                foreach (var entry in tempFilesArray)
                {
                    if (entry.Contains(arguments["updateFileName"]))
                    {
                        continue;
                    }
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
        }
        private static void MoveFilesToTemp(string pathToTempFolder, ProgressBar progressBar, TextBlock ratioTextBlock)
        {
            CheckAndClearTempFolder(pathToTempFolder);

            int totalFilesMoved = 0;
            var filesPathsList = Directory.EnumerateFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory);
            foreach (var entry in filesPathsList)
            {
                string destPath = Path.Combine(pathToTempFolder, Path.GetFileName(entry));
                if (entry.Contains(pathToTempFolder))
                {
                    totalFilesMoved++;
                    continue;
                }
                if (File.Exists(entry))
                {
                    File.Move(entry, destPath, true);
                }
                else
                {
                    if (Directory.Exists(entry))
                    {
                        Directory.Move(entry, destPath);
                    }
                }
                totalFilesMoved++;
                App.Current.Dispatcher.Invoke(() => ratioTextBlock.Text = $"{totalFilesMoved} / {filesPathsList.Count()}");
                App.Current.Dispatcher.Invoke(() => progressBar.Value = (double)totalFilesMoved / filesPathsList.Count() * 100);
            }
        }
        private static void ExtractZipUpdateFiles(string appMainDirectory, string tempZipPath, ProgressBar progressBar, TextBlock ratioTextBlock)
        {
            using (ZipArchive archive = ZipFile.OpenRead(tempZipPath))
            {
                int totalFilesExtracted = 0;
                foreach (var archiveEntry in archive.Entries)
                {
                    string fullPath = Path.Combine(appMainDirectory, archiveEntry.FullName);
                    if (archiveEntry.Name == "")
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        archiveEntry.ExtractToFile(fullPath, true);
                    }
                    totalFilesExtracted++;
                    App.Current.Dispatcher.Invoke(() => ratioTextBlock.Text = $"{totalFilesExtracted} / {archive.Entries.Count}");
                    App.Current.Dispatcher.Invoke(() => progressBar.Value = (double)totalFilesExtracted / archive.Entries.Count * 100);
                }
            }
        }
        private static void CloseUpdater()
        {
            string argumentToMainApp = $"--needToClearTemp {true}";
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(arguments["appDirectory"], arguments["appExecutableFileName"]),
                Arguments = argumentToMainApp,
                UseShellExecute = false
            });
            App.Current.Shutdown();
        }
        public static async Task DownloadAndInstallUpdate(ProgressBar progressBar, TextBlock statusTextBlock, TextBlock ratioTextBlock)
        {
            try
            {
                LoadCommandLineArgs(arguments);
                GetMainAppProcess();
                if (CheckUpdateAvailable() == false)
                {
                    return;
                }
                App.Current.Dispatcher.Invoke(() => progressBar.Visibility = Visibility.Visible);
                string pathToTempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
                if (!Directory.Exists(pathToTempFolder))
                {
                    Directory.CreateDirectory(pathToTempFolder);
                }
                string tempZipPath = Path.Combine(pathToTempFolder, arguments["updateFileName"]);
                string appMainDirectory = arguments["appDirectory"];
                App.Current.Dispatcher.Invoke(() => statusTextBlock.Text = "Загрузка обновления");
                await DownloadFileUpdateFileAsync(arguments["updateFileUrl"], tempZipPath, progressBar, ratioTextBlock);

                App.Current.Dispatcher.Invoke(() => statusTextBlock.Text = "Перемещение файлов в временную папку");
                MoveFilesToTemp(pathToTempFolder, progressBar, ratioTextBlock);

                App.Current.Dispatcher.Invoke(() => statusTextBlock.Text = $"Установка обновления");
                ExtractZipUpdateFiles(appMainDirectory, tempZipPath, progressBar, ratioTextBlock);

                App.Current.Dispatcher.Invoke(() => ratioTextBlock.Text = "");
                if(File.Exists(tempZipPath))
                {
                    App.Current.Dispatcher.Invoke(() => statusTextBlock.Text = $"Удаление скачанного архива обновления");
                    File.Delete(tempZipPath);
                }

                App.Current.Dispatcher.Invoke(() => statusTextBlock.Text = $"Обновление завершено");
                App.Current.Dispatcher.Invoke(CloseUpdater);                   
            }

            catch (Exception exception)
            {
                LoggerSerivce.Write(exception);
                App.Current.Dispatcher.Invoke(() => statusTextBlock.Text = $"При обновлении возникла ошибка");
                MessageBox.Show(exception.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
