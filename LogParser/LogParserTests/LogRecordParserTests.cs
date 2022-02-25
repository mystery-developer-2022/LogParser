using System;
using LogParser.LogRecordParser;
using Shouldly;
using Xunit;

namespace LogParserTests;

public class LogRecordParserTests
{
    [Fact]
    public void ParseLine1()
    {
        var data =
            @"177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] ""GET /intranet-analytics/ HTTP/1.1"" 200 3574 ""-"" ""Mozilla/5.0 (X11; U; Linux x86_64; fr-FR) AppleWebKit/534.7 (KHTML, like Gecko) Epiphany/2.30.6 Safari/534.7""";
        var record = LogRecordParser.ParseLine(data);

        record.ShouldNotBeNull();
        record.IPAddress.ToString().ShouldBe("177.71.128.21");
        record.Timestamp.ShouldBe(new DateTime(2018, 7, 10, 20, 21, 28, DateTimeKind.Utc));
        record.Uri.ShouldBe("/intranet-analytics/");
    }
    
    [Fact]
    public void ParseLine13()
    {
        var data = @"72.44.32.10 - - [09/Jul/2018:15:48:07 +0200] ""GET / HTTP/1.1"" 200 3574 ""-"" ""Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0"" junk extra";        
        var record = LogRecordParser.ParseLine(data);

        record.ShouldNotBeNull();
        record.IPAddress.ToString().ShouldBe("72.44.32.10");
        record.Timestamp.ShouldBe(new DateTime(2018, 7, 9, 13, 48, 07, DateTimeKind.Utc));
        record.Uri.ShouldBe("/");
    }
    
    
    [Fact]
    public void ShouldThrowWhenMissingExpectedFields()
    {
        var data = @"72.44.32.10 - - [09/Jul/2018:15:48:07 +0200] ""GET / HTTP/1.1"" ";

        var expectedException = Should.Throw<LogRecordParseException>(() => LogRecordParser.ParseLine(data));
        expectedException.LogLine.ShouldBe(data);
    }
    
    [Fact]
    public void ShouldThrowForInvalidIp()
    {
        var data = @"256.44.32.10 - - [09/Jul/2018:15:48:07 +0200] ""GET / HTTP/1.1"" 200 3574 ""-"" ""Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0"" junk extra";        
       
        var expectedException = Should.Throw<LogRecordParseException>(() => LogRecordParser.ParseLine(data));
        expectedException.LogLine.ShouldBe(data);
    }
    
    [Fact]
    public void ShouldThrowForInvalidDate()
    {
        var data = @"72.44.32.10 - - [2018-07-09] ""GET / HTTP/1.1"" 200 35KB ""-"" ""Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0"" junk extra";

        var expectedException = Should.Throw<LogRecordParseException>(() => LogRecordParser.ParseLine(data));
        expectedException.LogLine.ShouldBe(data);
    }
    
    [Fact]
    public void ShouldThrowForInvalidStatusCode()
    {
        var data = @"72.44.32.10 - - [09/Jul/2018:15:48:07 +0200] ""GET / HTTP/1.1"" OK 3574 ""-"" ""Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0"" junk extra";

        var expectedException = Should.Throw<LogRecordParseException>(() => LogRecordParser.ParseLine(data));
        expectedException.LogLine.ShouldBe(data);
    }
    
    
    [Fact]
    public void ShouldThrowForInvalidSize()
    {
        var data = @"72.44.32.10 - - [09/Jul/2018:15:48:07 +0200] ""GET / HTTP/1.1"" 200 35KB ""-"" ""Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0"" junk extra";

        var expectedException = Should.Throw<LogRecordParseException>(() => LogRecordParser.ParseLine(data));
        expectedException.LogLine.ShouldBe(data);
    }
}