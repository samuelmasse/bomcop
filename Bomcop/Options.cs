using CommandLine;

namespace Bomcop
{
    public class Options
    {
        [Value(0, HelpText = "Directory to check for BOM", MetaName = "dir")]
        public string? Dir { get; set; }

        [Option]
        public IEnumerable<string>? Exclude { get; set; }

        [Option("no-color")]
        public bool NoColor { get; set; }
    }
}
