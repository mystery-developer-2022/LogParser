using System;
using System.Threading.Tasks;
using CommandLine;
using LogParser.ExceptionStore;
using LogParser.Processors;

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
                var exceptionStore = new FileExceptionStore($"Exceptions-{DateTime.Now:yyyy-MM-dd-HHmmss}");
                var parser = new DomProcessor(exceptionStore);
                var result = await parser.ParseFile(options.File ?? "programming-task-example-data.log");
                await result.PrintResult(Console.Out);

            });
        }
        
    }
    
}
