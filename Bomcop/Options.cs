using CommandLine;

namespace Bomcop
{
    public class Options
    {
        [Value(0, HelpText = "Directory to check for BOM", MetaName = "dir")]
        public string? Dir { get; set; }

        [Option(HelpText = "List of file regex patterns to be excluded")]
        public IEnumerable<string>? Exclude { get; set; }

        [Option("no-color", HelpText = "Turn off colors for output")]
        public bool NoColor { get; set; }
    }
}
