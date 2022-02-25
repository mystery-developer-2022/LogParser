using System.Threading.Tasks;
using LogParser.Models;

namespace LogParser.Processors;

public interface ILogfileProcessor
{
    public Task<LogProcessorResult> ParseFile(string file);
}