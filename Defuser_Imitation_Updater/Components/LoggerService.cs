using System.IO;
using System.Text;

namespace Defuser_Imitation_Updater.Components
{
    public sealed class LoggerSerivce
    {
        private static object _sync = new object();
        public static void Write(Exception exception)
        {
            try
            {
                string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(pathToLog))
                {
                    Directory.CreateDirectory(pathToLog);
                }
                string fileName = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log",
                AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}.{2}()] {3}\r\n",
                DateTime.Now, exception.TargetSite?.DeclaringType, exception.TargetSite?.Name, exception,exception.Message);
                lock (_sync)
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    File.AppendAllText(fileName, fullText, Encoding.GetEncoding("windows-1251"));
                }
            }
            catch
            {

            }
        }
    }
}
