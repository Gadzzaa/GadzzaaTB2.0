using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace GadzzaaTB.Classes
{
    public class LogFile
    {
        private static string? _line;

        public static async Task<string?> LoadLogFile(string? description)
        {
            LogManager.SuspendLogging();
            LogManager.Shutdown();
            var directory =
                new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                  "\\GadzzaaTB\\logs\\");
            var myFile = directory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First();
            var sr = new StreamReader(myFile.FullName);
            _line = await sr.ReadLineAsync();
            description += "\n\n\n\n\nLog File:\n\n" + _line + "\n";
            _line = await sr.ReadLineAsync();
            while (_line != null)
            {
                description += _line + "\n";
                _line = await sr.ReadLineAsync();
            }

            sr.Close();
            return description;
        }
    }
}