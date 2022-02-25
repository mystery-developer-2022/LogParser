using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LogParser.LogRecordParser;
using Newtonsoft.Json;

namespace LogParser.ExceptionStore;

public class FileExceptionStore: IExceptionStore
{

    private readonly string _folder;

    public FileExceptionStore(string folder)
    {
        _folder = folder;
    }

    public async Task StoreException(LogRecordParseException exception)
    {
        if (!Directory.Exists(_folder)) Directory.CreateDirectory(_folder);
        var fileName = $"{_folder}{Path.DirectorySeparatorChar}-{Guid.NewGuid()}";
        await File.WriteAllTextAsync(fileName, JsonConvert.SerializeObject(exception, JsonSettings));
    }

    public async IAsyncEnumerable<LogRecordParseException> FetchExceptions()
    {
        var files = Directory.EnumerateFiles(_folder);
        foreach (var file in files)
        {
            var deserializedException = JsonConvert.DeserializeObject<LogRecordParseException>((await File.ReadAllTextAsync(file)), JsonSettings);
            if (deserializedException != null) yield return deserializedException;
        }
    }

    public async Task<LogRecordParseException?> LoadException(string path)
    {
        return JsonConvert.DeserializeObject<LogRecordParseException>((await File.ReadAllTextAsync(path)), JsonSettings);
    }


    private JsonSerializerSettings JsonSettings => new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.All
    };
}