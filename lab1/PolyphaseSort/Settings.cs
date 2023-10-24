using CommandLine;

namespace CommandSettings
{
    // Program settings
    public class Settings
    {

        // input file path
        [Option('i', "inputPath", Required = false, HelpText = "Input file path. Default value is 'input.txt'.")]
        public string InputPath { get; set; } = "input.txt";

        // output file path
        [Option('o', "outputPath", Required = false, HelpText = "Output file path. Default value is 'output.txt'.")]
        public string OutputPath { get; set; } = "output.txt";

        // tapes count
        [Option('t', "tapesCount", Required = false, HelpText = "Number of tapes to sort the inut file. Default value is 5.")]
        public int tapesCount { get; set; } = 5;

        // size of input file
        [Option('s', "fileSizeMB", Required = false, HelpText = "Generated input file size in megabytes. If no need to generate file use default value (0).")]
        public int FileSizeMB { get; set; } = 0;

        // preSort
        [Option('p', "preSort", Required = false, HelpText = "Pre Sort the input file. Default value is 'false'.")]
        public bool preSort { get; set; } = false;
    }
}
