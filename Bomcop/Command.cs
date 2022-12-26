using CommandLine;

namespace Bomcop
{
    public class Command
    {
        private readonly string[] args;

        public Command(string[] args)
        {
            this.args = args;
        }

        public bool Run()
        {
            var options = Parse();
            if (options is null)
                return false;

            var dir = GetDirectory(options);
            if (dir is null)
                return false;

            return new Tool(dir, GetExcludes(options), GetColored(options)).Run();
        }

        private Options? Parse()
        {
            return Parser.Default.ParseArguments<Options>(args).Value;
        }

        private string? GetDirectory(Options options)
        {
            string dir = options.Dir ?? Directory.GetCurrentDirectory();

            if (!Directory.Exists(dir))
            {
                Console.WriteLine($"Directory does not exist : {dir}");
                return null;
            }

            return dir;
        }

        private string[] GetExcludes(Options options)
        {
            var excludes = new List<string>(Excludes.Default);
            if (options.Exclude is not null)
                excludes.AddRange(options.Exclude);

            return excludes.ToArray();
        }

        private bool GetColored(Options options)
        {
            var envNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
            return !options.NoColor && envNoColor is null;
        }
    }
}
