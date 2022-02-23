using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace LogParser
{
    class Program
    {
        
        public class Options
        {
            [Option('f', "file", Required = false, HelpText = "log file to parse or 'programming-task-example-data.log' by default")]
            public string? File { get; set; }
            
            [Option('s', "stream", Required = false, HelpText = "parse file in stream mode")]
            public bool StreamMode { get; set; }
        }
        
        public static async Task Main(params string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async options =>
            {
                await ExecuteLogParser(options.File ?? "programming-task-example-data.log", options.StreamMode);
            });
        }

        
        private static async Task ExecuteLogParser(string fileName, bool streamMode)
        {
            await Console.Out.WriteLineAsync($"parsing {fileName} using {(streamMode ? "stream" : "DOM")} parsing mode");
        }
    }
    
}
