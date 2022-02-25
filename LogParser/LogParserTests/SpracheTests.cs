using System;
using System.Collections.Generic;
using System.Linq;
using LogParser;
using LogParser.LogRecordParser;
using Shouldly;
using Sprache;
using Xunit;

namespace LogParserTests;

public class SpracheTests
{
    
    
    [Fact]
    public void SpracheTest_Line1()
    {
        var data = @"177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] ""GET /intranet-analytics/ HTTP/1.1"" 200 3574 ""-"" ""Mozilla/5.0 (X11; U; Linux x86_64; fr-FR) AppleWebKit/534.7 (KHTML, like Gecko) Epiphany/2.30.6 Safari/534.7""";
        var record = SpracheParser.RecordParser.Parse(data).ToList();
        
        record[0].ShouldBe("177.71.128.21");
        record[3].ShouldBe("10/Jul/2018:22:21:28 +0200");
        record[4].ShouldBe("GET /intranet-analytics/ HTTP/1.1");
        record[8].ShouldBe("Mozilla/5.0 (X11; U; Linux x86_64; fr-FR) AppleWebKit/534.7 (KHTML, like Gecko) Epiphany/2.30.6 Safari/534.7");
    }
    
    [Fact]
    public void SpracheTest_Line13()
    {
        var data = @"72.44.32.10 - - [09/Jul/2018:15:48:07 +0200] ""GET / HTTP/1.1"" 200 3574 ""-"" ""Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0"" junk extra";
        var record = SpracheParser.RecordParser.Parse(data).ToList();
        
        record[0].ShouldBe("72.44.32.10");
        record[4].ShouldBe("GET / HTTP/1.1");
        record[8].ShouldBe("Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0");
        // I think we can ignore 'junk extra' from unlucky line 13 
        
    }
}