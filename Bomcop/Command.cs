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
            var result = Parser.Default.ParseArguments<Options>(args);
            if (result.Errors.Any())
                return false;

            string? dir = result.Value?.Dir;
            if (dir is not null && !Directory.Exists(dir))
            {
                Console.WriteLine($"Directory does not exist : {dir}");
                return false;
            }

            var excludes = new List<string>(Excludes.Default);
            if (result.Value?.Exclude is not null)
                excludes.AddRange(result.Value.Exclude);

            return new Tool(dir ?? Directory.GetCurrentDirectory(), excludes.ToArray()).Run();
        }
    }
}
