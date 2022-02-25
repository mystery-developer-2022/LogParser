using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Sprache;

namespace LogParser.LogRecordParser;


public class LogRecordParser
{
    public static LogRecord ParseLine(string logLine)
    {
        try
        {
            var stringValues = SpracheParser.RecordParser.Parse(logLine).ToList(); // sprache parser: chop line into string values
            if (stringValues.Count < 9) throw new ArgumentException("Failed to extract expected number of values from log line"); // we ignore unexpected extra fields but throw if expected fields are missing.
            
            var methodUrlVersion = stringValues[4].Split(" ").ToList(); // ie: GET /intranet-analytics/ HTTP/1.1
            return new LogRecord()
            {
                IPAddress = IPAddress.Parse(stringValues[0]),
                UserId = stringValues[2],
                Timestamp = DateTimeOffset.ParseExact(stringValues[3], "dd/MMM/yyyy:HH:mm:ss zzz", CultureInfo.InvariantCulture).UtcDateTime,
                HttpMethod = methodUrlVersion[0],
                Uri = methodUrlVersion[1],
                HttpVersion = methodUrlVersion[2],
                StatusCode = int.Parse(stringValues[5]),
                Size = int.Parse(stringValues[6]),
                UserAgent = stringValues[8]
            };
        }
        catch (Exception ex)
        {
            throw new LogRecordParseException(logLine, ex);
        }
        
    }
    
}


/// <summary>
/// the logfile format is basically space-delimited but with some values quoted with "" or []
/// A sprache parser was chosen as the most robust way to handle this
/// https://github.com/sprache/Sprache/
/// this is heavily based on the csv example here: https://github.com/sprache/Sprache/blob/develop/test/Sprache.Tests/Scenarios/CsvTests.cs
/// </summary>
public static class SpracheParser
{
    static readonly Parser<char> ItemSeparator = Parse.Char(' ');
    static readonly Parser<char> ItemContent =
        Parse.AnyChar.Except(ItemSeparator).Except(Parse.String(Environment.NewLine));
    
    static readonly Parser<char> QuotedItemDelimiter = Parse.Char('"');
    static readonly Parser<char> QuotedItemContent =
        Parse.AnyChar.Except(QuotedItemDelimiter);
    static readonly Parser<string> QuotedItem =
        from open in QuotedItemDelimiter
        from content in QuotedItemContent.Many().Text()
        from end in QuotedItemDelimiter
        select content;
    
    static readonly Parser<char> DateStartDelimiter = Parse.Char('[');
    static readonly Parser<char> DateEndDelimiter = Parse.Char(']');
    static readonly Parser<char> DateItemContent = Parse.AnyChar.Except(DateEndDelimiter);
    static readonly Parser<string> DateItem =
        from open in DateStartDelimiter
        from content in DateItemContent.Many().Text()
        from end in DateEndDelimiter
        select content;
    
    static readonly Parser<string> NewLine =
        Parse.String(Environment.NewLine).Text();
    
    static readonly Parser<string> RecordTerminator =
        Parse.Return("").End().XOr(
            NewLine.End()).Or(
            NewLine);
    
    static readonly Parser<string> Item =
        QuotedItem
            .XOr(DateItem)
            .XOr(ItemContent.XMany().Text());
    
    static IEnumerable<T> Cons<T>(T head, IEnumerable<T> rest)
    {
        yield return head;
        foreach (var item in rest)
            yield return item;
    }
    
    /// <summary>
    ///  this is the main entry point: parse a log line into a collection of strings
    /// </summary>
    public static readonly Parser<IEnumerable<string>> RecordParser =
        from leading in Item
        from rest in ItemSeparator.Then(_ => Item).Many()
        from terminator in RecordTerminator
        select Cons(leading, rest);
    
}