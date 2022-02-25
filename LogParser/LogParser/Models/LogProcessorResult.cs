using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace LogParser.Models;

public class LogProcessorResult
{
    public LogProcessorResult(string file)
    {
        File = file;
    }

    public string File  { get; set; }

    public int CountUniqueIpAddresses { get; set; } = 0;

    public IEnumerable<string> MostVisitedUrls { get; set; } = new List<string>();

    public IEnumerable<IPAddress> MostActiveIps { get; set; } = new List<IPAddress>();

    public int CountFailedLines { get; set; } = 0;

    public async Task PrintResult(TextWriter writer)
    {
        await writer.WriteLineAsync($"Processing File {File}...");
        await writer.WriteLineAsync("");
        if (CountFailedLines > 0)
        {
            await writer.WriteLineAsync($"Warning: {CountFailedLines} log line{(CountFailedLines > 1 ? "s" : "")} failed to parse and are excluded from the result");
            await writer.WriteLineAsync("These errors have been logged to an exception store");
            await writer.WriteLineAsync("");
        }

        await writer.WriteLineAsync($"Count Unique Ip Addresses: {CountUniqueIpAddresses}");
        await writer.WriteLineAsync($"Most Visited Urls:");
        foreach (var url in MostVisitedUrls)
        {
            await writer.WriteLineAsync($"\t - {url}");
        }
        await writer.WriteLineAsync($"Most Active Ips:");
        foreach (var url in MostActiveIps)
        {
            await writer.WriteLineAsync($"\t - {url}");
        }
        
    }
}