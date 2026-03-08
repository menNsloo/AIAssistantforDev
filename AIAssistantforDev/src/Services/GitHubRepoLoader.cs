using System.Diagnostics;
using Models.CodeFile;

namespace root.Services
{
    public class GitHubRepoLoader
    {
        public async Task<List<CodeFile>> LoadRepositoryAsync(string repourl)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            await CloneRepo(repourl, tempPath);

            var files = ReadFiles(tempPath);

            return files;
        }

        private async Task CloneRepo(string repoUrl, string targetPath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"clone {repoUrl} \"{targetPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception("Git clone failed");
            }
        }

        private List<CodeFile> ReadFiles(string FolderPath)
        {
            var files = Directory.GetFiles(FolderPath, "*.*", SearchOption.AllDirectories);

            var codeFiles = new List<CodeFile>();

            foreach (var file in files)
            {
                if (file.Contains(".git"))
                    continue;

                var content = File.ReadAllText(file);

                codeFiles.Add(new CodeFile
                {
                    FileName = Path.GetFileName(file),
                    FilePath = file,
                    Content = content
                });
            }

            return codeFiles;
        }
    }
}
