using System.Diagnostics;

namespace Bomcop
{
    public class Tool
    {
        private readonly string dir;

        public Tool(string dir)
        {
            this.dir = dir;
        }

        public bool Run()
        {
            var files = GetFiles();
            int invalid = 0;

            foreach (var file in files)
            {
                if (!CheckBomOnFile(file))
                    invalid++;
            }

            if (invalid > 0)
            {
                Console.WriteLine();
                Print($"{invalid} bom missing", ConsoleColor.Red);
            }

            return invalid != 0;
        }

        private string[] GetFiles()
        {
            var proc = RunGitListFiles();
            var output = GetProcOutput(proc);

            if (proc.ExitCode == 0)
                return ResolvePaths(GetLines(output));
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
            List<string> lines = new();

            using StringReader sr = new(output);
            string? line;

            while ((line = sr.ReadLine()) != null)
                lines.Add(line);

            return lines.ToArray();
        }
        private string[] ResolvePaths(string[] lines)
        {
            foreach (ref var line in new Span<string>(lines))
                line = Path.Join(dir, line);

            return lines;
        }
        private string[] ListFilesRecursively(string dir)
        {
            return Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
        }

        private bool CheckBomOnFile(string file)
        {
            using var fs = File.OpenRead(file);

            Span<byte> bom = stackalloc byte[3];
            fs.Read(bom);

            if (bom[0] != 0xef || bom[1] != 0xbb || bom[2] != 0xbf)
            {
                string path = Path.GetRelativePath(dir, file);

                Print($"{path}:", ConsoleColor.Yellow);
                Print($"    UTF-8 BOM missing at start of file", ConsoleColor.Red);

                return false;
            }
            else return true;
        }

        private void Print(string str, ConsoleColor color)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = prev;
        }
    }
}
