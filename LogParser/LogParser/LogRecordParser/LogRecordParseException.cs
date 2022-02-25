using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LogParser.LogRecordParser;

public class LogRecordParseException: Exception
{
    public string LogLine { get; private set; }

    public LogRecordParseException(string logLine, Exception inner)
        : base("Exception parsing log line: "+inner.Message, inner)
    {
        LogLine = logLine;
    }


    /// <summary>
    /// constructor to support json deserialization
    /// </summary>
    /// <param name="logLine"></param>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    [JsonConstructor]
    public LogRecordParseException(string logLine, string message, Exception innerException) // param names must match target readonly properties
        : base(message, innerException)
    {
        LogLine = logLine;
    }
   
}