using System;
using System.Runtime.InteropServices;
using CommandLine;

namespace Settings
{
    public class Options
    {
        [Option('q', "nQueens", Required = false, HelpText = "Number of Queens on the board to generate.")]
        public int N { get; set; } = 8;

        [Option("AStar", Required = false, HelpText = "Use A* algorithm to solve the problem.")]
        public bool AStar { get; set; } = false;

        [Option("BFS", Required = false, HelpText = "Use BFS algorithm to solve the problem.")]
        public bool BFS { get; set; } = false;

        [Option('m', "MemoryGB", Required = false, HelpText = "RAM limit for program.")]
        public int MemoryGB { get; set; } = 4;
    }
}
