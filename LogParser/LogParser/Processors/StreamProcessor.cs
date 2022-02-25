using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LogParser.ExceptionStore;
using LogParser.LogRecordParser;
using LogParser.Models;

namespace LogParser.Processors;

/// <summary>
/// steam processor - instead of loading all records into memory, we process them in a stream, collecting metrics as we go
/// </summary>
public class StreamProcessor: ILogfileProcessor
{
    
    private readonly IExceptionStore? _exceptionStore = null;
    
    public StreamProcessor()
    {
    }

    public StreamProcessor(IExceptionStore? exceptionStore)
    {
        _exceptionStore = exceptionStore;
    }
    
    public async Task<LogProcessorResult> ParseFile(string file)
    {
        var data = new List<LogRecord>();
        int countFailedLines = 0;
        string? line;
        using (var reader = new StreamReader(File.OpenRead(file)))
        {
            while ((line = await reader.ReadLineAsync()) != null)
            {
                try
                {
                    var logRecord = LogRecordParser.LogRecordParser.ParseLine(line);
                    foreach (var metricCollector in _metricCollectors)
                    {
                        metricCollector.ProcessRecord(logRecord);
                    }
                }
                catch (LogRecordParseException ex)
                {
                    countFailedLines++;
                    if (_exceptionStore != null) await _exceptionStore.StoreException(ex);
                }
            }
        }

        var result = new LogProcessorResult(file)
        {
            CountFailedLines = countFailedLines
        };
        foreach (var metricCollector in _metricCollectors)
        {
            metricCollector.ApplyResult(result);
        }

        return result;
    }


    private readonly IList<IMetricCollector> _metricCollectors = new List<IMetricCollector>()
    {
        new UniqueIpCollector(),
        new MostPopularIpsCollector(),
        new MostVisitedUrlsCollector()
    };

}


public interface IMetricCollector
{
    /// <summary>
    /// apply each log record to the metric collector
    /// </summary>
    /// <param name="logRecord"></param>
    public void ProcessRecord(LogRecord logRecord);

    /// <summary>
    /// apply totalled metric to the result.
    /// </summary>
    public void ApplyResult(LogProcessorResult result);
}

public class UniqueIpCollector: IMetricCollector
{
    private List<IPAddress> _uniqueIps = new List<IPAddress>();
    public void ProcessRecord(LogRecord logRecord)
    {
        if (!_uniqueIps.Contains(logRecord.IPAddress))
        {
            _uniqueIps.Add(logRecord.IPAddress);
        }
    }
    public void ApplyResult(LogProcessorResult result)
    {
        result.CountUniqueIpAddresses = _uniqueIps.Count;
    }
}

public class MostPopularIpsCollector : IMetricCollector
{
    private Dictionary<IPAddress, int> _dictionary = new Dictionary<IPAddress, int>();
    public void ProcessRecord(LogRecord logRecord)
    {
        if (!_dictionary.ContainsKey(logRecord.IPAddress)) _dictionary[logRecord.IPAddress] = 0;
        _dictionary[logRecord.IPAddress]++;
    }

    public void ApplyResult(LogProcessorResult result)
    {
        result.MostActiveIps = _dictionary.OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .Take(3);
    }
}

public class MostVisitedUrlsCollector : IMetricCollector
{
    private Dictionary<string, int> _dictionary = new Dictionary<string, int>();
    public void ProcessRecord(LogRecord logRecord)
    {
        if (!_dictionary.ContainsKey(logRecord.Uri)) _dictionary[logRecord.Uri] = 0;
        _dictionary[logRecord.Uri]++;
    }

    public void ApplyResult(LogProcessorResult result)
    {
        result.MostVisitedUrls = _dictionary.OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .Take(3);
    }
}
