using LocalPortService.Model.API;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LocalPortService.BizService.ProgramLauch
{
    public class LaunchExistingFileService
    {
        private readonly IConfiguration _config;
        public LaunchExistingFileService(IConfiguration config)
        {
            _config = config;
        }

        public bool LauchProgram(OpenProgramRequest req)
        {
            try
            {
                //string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string executableLocation = AppContext.BaseDirectory;
                executableLocation = executableLocation.TrimEnd('\\');
                string parentDirectory = Directory.GetParent(executableLocation).FullName;
                string batDir = string.Empty;
                string batName = string.Empty;
                switch (req.Key.ToLower())
                {
                    case "shipping":
                        batName = _config["OtherSubSystemFilePath:shipping"];
                        break;
                    case "ecommerce":
                        batName = _config["OtherSubSystemFilePath:ecommerce"];
                        break;
                    case "financial":
                        batName = _config["OtherSubSystemFilePath:FinancialSystem"];
                        break;
                    case "report":
                        batName = _config["OtherSubSystemFilePath:bi"];
                        break;
                    default: throw new Exception();
                }
                batDir = Path.Combine(parentDirectory, "batFolder");
                string batPath = Path.Combine(batDir, batName);
                if (File.Exists(batPath))
                {

                    ProcessStartInfo info = new ProcessStartInfo();
                    info.WorkingDirectory = batDir;
                    info.FileName = batName;
                    info.UseShellExecute = true;
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    info.Verb = "runas";
                    Process.Start(info);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
            
        }
    }
}
