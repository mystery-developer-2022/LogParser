using System;
using System.Net;
using System.Net.Http;

namespace LogParser;

/// <summary>
/// models a single log record - corresponding to one line og the logfile
/// Log mostly follows the "common log format" https://en.wikipedia.org/wiki/Common_Log_Format but with a few extra fields
/// </summary>
public class LogRecord
{
    public IPAddress IPAddress { get; set; }
    
    public string UserId { get; set; }

    public DateTime Timestamp { get; set; }

    public string HttpMethod { get; set; }

    public string Uri { get; set; }
    
    public string HttpVersion { get; set; }

    public int StatusCode { get; set; }
    
    public int Size { get; set; }

    public string UserAgent { get; set; }


}