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
            
            [Option('s', "stream", Required = false, HelpText = "parse file in memory-efficient stream mode instead of DOM mode")]
            public bool StreamMode { get; set; }
        }
        
        public static async Task Main(params string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async options =>
            {
                var exceptionStore = new FileExceptionStore($"Exceptions-{DateTime.Now:yyyy-MM-dd-HHmmss}");
                ILogfileProcessor processor = options.StreamMode 
                    ? new StreamProcessor(exceptionStore)
                    : new DomProcessor(exceptionStore);
                var result = await processor.ParseFile(options.File ?? "programming-task-example-data.log");
                await result.PrintResult(Console.Out);
            });
        }
        
    }
    
}
