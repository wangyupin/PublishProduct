using Velopack;
using Velopack.Sources;

namespace LocalPortService.Core.ProgramUpdate
{
    public static class VeloPackService
    {
        public static async Task UpdateMyApp()
        {
            try
            {
                var mgr = new UpdateManager(new GithubSource("https://github.com/GBRD2021/CityPlan/", "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", false));

                // check for new version
                var newVersion = await mgr.CheckForUpdatesAsync();
                if (newVersion == null)
                    return; // no update available

                // download new version
                await mgr.DownloadUpdatesAsync(newVersion);

                // install new version and restart app
                mgr.ApplyUpdatesAndRestart(newVersion);
            }
            catch (Exception e)
            {
                string executableLocation = AppContext.BaseDirectory;
                executableLocation = executableLocation.TrimEnd('\\');
                string parentDirectory = Directory.GetParent(executableLocation).FullName;
                File.WriteAllText($"{parentDirectory}/AutoUpdateError.txt", e.ToString());
            }
            
        }
    }
}
