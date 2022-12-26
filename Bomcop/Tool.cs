using System.Diagnostics;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Bomcop
{
    public class Tool
    {
        private readonly string dir;
        private readonly string[] excludes;
        private readonly bool colored;

        public Tool(string dir, string[] excludes, bool colored)
        {
            this.dir = dir;
            this.excludes = excludes;
            this.colored = colored;
        }

        public bool Run()
        {
            var files = GetFiles();
            var filtered = FilterFiles(files);
            int invalid = 0;

            foreach (var file in filtered)
            {
                if (!CheckBomOnFile(file))
                    invalid++;
            }

            if (invalid > 0)
            {
                Console.WriteLine();
                Print($"{invalid} bom missing", ConsoleColor.Red);
            }

            return invalid == 0;
        }

        private string[] GetFiles()
        {
            var proc = RunGitListFiles();
            var output = GetProcOutput(proc);

            if (proc.ExitCode == 0)
                return GetLines(output);
            else return ListFilesRecursively(dir);
        }
        private Process RunGitListFiles()
        {
            Process proc = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = dir,
                    FileName = "git",
                    Arguments = "ls-files --cached --others --modified --exclude-standard"
                }
            };

            proc.Start();
            return proc;
        }
        private string GetProcOutput(Process proc)
        {
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output;
        }
        private string[] GetLines(string output)
        {
            HashSet<string> dupes = new();
            List<string> lines = new();

            using StringReader sr = new(output);
            string? line;

            while ((line = sr.ReadLine()) != null)
            {
                if (!dupes.Contains(line))
                {
                    lines.Add(line);
                    dupes.Add(line);
                }
            }

            return lines.ToArray();
        }
        private string[] ListFilesRecursively(string dir)
        {
            return Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                .Select(x => Path.GetRelativePath(dir, x).Replace('\\', '/'))
                .ToArray();
        }

        private string[] FilterFiles(string[] files)
        {
            var matcher = new Matcher();
            matcher.AddInclude("**");
            matcher.AddExcludePatterns(excludes);
            return matcher.Match(files).Files.Select(x => x.Path).ToArray();
        }

        private bool CheckBomOnFile(string file)
        {
            using var fs = File.OpenRead(Path.Join(dir, file));

            Span<byte> bom = stackalloc byte[3];
            fs.Read(bom);

            if (bom[0] != 0xef || bom[1] != 0xbb || bom[2] != 0xbf)
            {
                Print($"{file}:", ConsoleColor.Yellow);
                Print($"    UTF-8 BOM missing at start of file", ConsoleColor.Red);

                return false;
            }
            else return true;
        }

        private void Print(string str, ConsoleColor color)
        {
            var prev = Console.ForegroundColor;
            if (colored)
                Console.ForegroundColor = color;
            Console.WriteLine(str);
            if (colored)
                Console.ForegroundColor = prev;
        }
    }
}
