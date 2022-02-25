using System;
using System.IO;
using System.Threading.Tasks;
using LogParser.ExceptionStore;
using LogParser.LogRecordParser;
using LogParser.Processors;
using Shouldly;
using Xunit;

namespace LogParserTests;


/// <summary>
/// "Repeatable Execution" - as coined by Mark Seaman
/// https://blog.ploeh.dk/2020/03/23/repeatable-execution/
/// Test the ability to replay exceptions thrown during log parsing. 
/// </summary>
public class RepeatableExecutionTests
{

    [Fact]
    public async Task GivenBadDataShouldCaptureExceptionAndAllowReplay()
    {
        var logFolder = $"RepeatableExecutionTests-{Guid.NewGuid()}";
        try
        {
            var fileExceptionStore = new FileExceptionStore(logFolder);
            var processor = new DomProcessor(fileExceptionStore);

            // this file contains an unexpected value on the first line
            var parseResult = await processor.ParseFile("Files/example-unexpected-size.log");

            // bad line reported in result 
            parseResult.CountFailedLines.ShouldBe(1);

            // we can retrieve the exception details later and replay from a unit test
            var errors = fileExceptionStore.FetchExceptions().GetAsyncEnumerator();
            (await errors.MoveNextAsync()).ShouldBeTrue("expect to fetch exception from exception store");
            var exceptionFromStore = errors.Current;
            exceptionFromStore.ShouldNotBeNull();
            exceptionFromStore.LogLine.ShouldNotBeNull();

            // replay parsing the log line - should throw same exception details
            var replayedException = Should.Throw<LogRecordParseException>(() => LogRecordParser.ParseLine(exceptionFromStore.LogLine));

            replayedException.Message.ShouldBe(exceptionFromStore.Message);
            replayedException.InnerException?.Message
                .ShouldNotBeNull()
                .ShouldBe(exceptionFromStore.InnerException?.Message);
            replayedException.InnerException?.GetType()
                .ShouldNotBeNull()
                .ShouldBe(exceptionFromStore.InnerException?.GetType());
        }
        finally
        {
            if (Directory.Exists(logFolder)) Directory.Delete(logFolder, true);
        }



    }

}