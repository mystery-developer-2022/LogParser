using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogParser.ExceptionStore;
using LogParser.LogRecordParser;
using LogParser.Models;

namespace LogParser.Processors;

/// <summary>
/// DOM Processor loads the file into an in-memory Document Object Model
/// Pro: easier to understand. Easy to transform or export data after parsing.
/// Con: high memory usage for large files 
/// </summary>
public class DomProcessor: ILogfileProcessor
{
    private readonly IExceptionStore? _exceptionStore = null;
    
    public DomProcessor()
    {
    }

    public DomProcessor(IExceptionStore? exceptionStore)
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
                    data.Add(LogRecordParser.LogRecordParser.ParseLine(line));
                }
                catch (LogRecordParseException ex)
                {
                    countFailedLines++;
                    if (_exceptionStore != null) await _exceptionStore.StoreException(ex);
                }
            }
        }

        return new LogProcessorResult(file)
        {
            CountFailedLines = countFailedLines,
            
            CountUniqueIpAddresses = data.Select(d => d.IPAddress)
                .Distinct()
                .Count(),
            
            MostActiveIps = data
                .GroupBy(l => l.IPAddress)
                .Select(g => new
                {
                    IPAddress = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(3)
                .Select(x => x.IPAddress),
            
            MostVisitedUrls = data.GroupBy(l => l.Uri)
                .Select(g => new
                {
                    Uri = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(3)
                .Select(x => x.Uri)
        };
    }
}