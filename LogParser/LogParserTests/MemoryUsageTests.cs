using System;
using System.IO;
using System.Threading.Tasks;
using LogParser.Processors;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace LogParserTests;

public class MemoryUsageTests
{
    
    private readonly ITestOutputHelper _output;

    public MemoryUsageTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact (Skip="Run memory tests in isolation")]
    //[Fact]
    public async Task BaseMemoryUsage()
    {
        // measure memory
        System.Diagnostics.Process.GetCurrentProcess().WorkingSet64.ShouldBeLessThan(50 * 1024 * 1024);
    }
    
    [Fact (Skip="Run memory tests in isolation")]
    //[Fact]
    public async Task GivenLargeFileDomProcessorShouldUseLotsOfMemory()
    {
        // make big file
        var fileName = Guid.NewGuid().ToString();
        string testFileContent = await File.ReadAllTextAsync("Files/programming-task-example-data.log");

        do
        {
            for (var i = 0; i < 1000; i++)
            {
                await File.AppendAllTextAsync(fileName, testFileContent);
            }
        } while (new FileInfo(fileName).Length < 100 * 1024 * 1024); // 100MB 

        // process file
        var processor = new DomProcessor();
        var result = await processor.ParseFile(fileName);

        // measure memory
        System.Diagnostics.Process.GetCurrentProcess().WorkingSet64.ShouldBeGreaterThan(100 * 1024 * 1024);
        
        File.Delete(fileName);
    }
    
    
    
    [Fact (Skip="Run memory tests in isolation")]
    //[Fact]
    public async Task GivenLargeFileStreamProcessorShouldUseLessMemory()
    {
        // make big file
        var fileName = Guid.NewGuid().ToString();
        string testFileContent = await File.ReadAllTextAsync("Files/programming-task-example-data.log");

        do
        {
            for (var i = 0; i < 1000; i++)
            {
                await File.AppendAllTextAsync(fileName, testFileContent);
            }
        } while (new FileInfo(fileName).Length < 100 * 1024 * 1024); // 100MB 

        // process file
        var processor = new DomProcessor();
        var result = await processor.ParseFile(fileName);

        // measure memory
        System.Diagnostics.Process.GetCurrentProcess().WorkingSet64.ShouldBeGreaterThan(100 * 1024 * 1024);
        
        File.Delete(fileName);
    }
}