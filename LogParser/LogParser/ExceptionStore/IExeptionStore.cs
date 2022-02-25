using System.Collections.Generic;
using System.Threading.Tasks;
using LogParser.LogRecordParser;

namespace LogParser.ExceptionStore;

public interface IExceptionStore
{

    public Task StoreException(LogRecordParseException exception);
    
    public IAsyncEnumerable<LogRecordParseException> FetchExceptions();

}