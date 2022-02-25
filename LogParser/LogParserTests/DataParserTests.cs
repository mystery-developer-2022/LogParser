using System;
using System.Globalization;
using System.Net;
using Shouldly;
using Xunit;

namespace LogParserTests;


/// <summary>
/// low level tests for the data types chosen for our LogRecord model
/// </summary>
public class DataParserTests
{
    [Fact]
    public void ShouldParseIpAddressString()
    {
        var ip = IPAddress.Parse("177.71.128.21");
        ip.ShouldNotBeNull();
        ((int)ip.GetAddressBytes()[0]).ShouldBe(177);
    }
    
    [Fact]
    public void ShouldThrowBadIpAddress()
    {
        Should.Throw<FormatException>(() =>
        {
            var ip = IPAddress.Parse("257.71.128.21");
        });
    }
    
    [Fact]
    public void ParseDateFormat()
    {
        var dateString = "10/Jul/2018:22:21:28 +0200";
        var dateTime = DateTimeOffset.ParseExact(dateString, "dd/MMM/yyyy:HH:mm:ss zzz", CultureInfo.InvariantCulture);
        dateTime.ShouldBe(new DateTimeOffset(2018, 7, 10, 22, 21, 28, TimeSpan.FromHours(2)));
    }
}