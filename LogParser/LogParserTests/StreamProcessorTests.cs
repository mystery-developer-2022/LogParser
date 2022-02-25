using System.Linq;
using System.Threading.Tasks;
using LogParser.Processors;
using Shouldly;
using Xunit;

namespace LogParserTests;

public class StreamProcessorTests
{
    [Fact]
    public async Task StreamProcessorShouldHandleFile()
    {
        var sut = new StreamProcessor();
        var parseResult = await sut.ParseFile("Files/programming-task-example-data.log");
        
        parseResult.ShouldNotBeNull();
        parseResult.CountFailedLines.ShouldBe(0);
        parseResult.CountUniqueIpAddresses.ShouldBe(11);
        parseResult.MostActiveIps.First()?.ToString().ShouldBe("168.41.191.40");
        // note test file only has one uri with more than 1 hit
        parseResult.MostVisitedUrls.First().ShouldBe("/docs/manage-websites/");
    }
}