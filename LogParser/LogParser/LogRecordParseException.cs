using System;

namespace LogParser;

public class LogRecordParseException: Exception
{
    public string LogLine { get; set; }

    public LogRecordParseException(string logLine, Exception inner)
        : base("Exception parsing log line: "+inner.Message, inner)
    {
        LogLine = logLine;
    }
}