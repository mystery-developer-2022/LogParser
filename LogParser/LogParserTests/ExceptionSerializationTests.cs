using LogParser.LogRecordParser;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace LogParserTests;

public class ExceptionSerializationTests
{
    
    /// <summary>
    /// test our ability to serialize/deserialize LogRecordParseException  for our exception store
    /// </summary>
    [Fact]
    public void ShouldSerializeIpParseException()
    {
        var badLine =  @"256.44.32.10 - - [09/Jul/2018:15:48:07 +0200] ""GET / HTTP/1.1"" 200 3574 ""-"" ""Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0"" junk extra";
        var exception = Should.Throw<LogRecordParseException>(() => LogRecordParser.ParseLine(badLine));
        
        var serialized = JsonConvert.SerializeObject(exception, JsonSettings);
        serialized.ShouldNotBeEmpty();
        var deserialized = JsonConvert.DeserializeObject<LogRecordParseException>(serialized, JsonSettings);
        deserialized.ShouldNotBeNull();
        deserialized.LogLine.ShouldBe(badLine);
        deserialized.Message.ShouldBe(exception.Message);
        deserialized.InnerException.ShouldNotBeNull()
            .GetType().ShouldBe(exception.InnerException?.GetType());
    }
    
    
    
    [Fact]
    public void ShouldSerializeIntParseException()
    {
        var badLine =  @"12.44.32.10 - - [09/Jul/2018:15:48:07 +0200] ""GET / HTTP/1.1"" 200 35KB ""-"" ""Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0"" junk extra";
        var exception = Should.Throw<LogRecordParseException>(() => LogRecordParser.ParseLine(badLine));
        
        var serialized = JsonConvert.SerializeObject(exception, JsonSettings);
        serialized.ShouldNotBeEmpty();
        var deserialized = JsonConvert.DeserializeObject<LogRecordParseException>(serialized, JsonSettings);
        deserialized.ShouldNotBeNull();
        deserialized.LogLine.ShouldBe(badLine);
        deserialized.Message.ShouldBe(exception.Message);
        deserialized.InnerException.ShouldNotBeNull()
            .GetType().ShouldBe(exception.InnerException?.GetType());
    }
    

    private JsonSerializerSettings JsonSettings => new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.All
    };
}