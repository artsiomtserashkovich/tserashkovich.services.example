using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaaS.Services.Example.Api.Storage.Persistence.Model;

namespace TaaS.Services.Example.Api.Storage.Persistence.Dao
{
    public interface IPrintersDao
    {
        IAsyncEnumerable<Printer> ReadAll();
        
        ValueTask<Printer> Read(Guid id);
        
        Task<Guid> Create(Printer printer);

        Task Delete(Guid id);

        Task Edit(Printer printer);

        Task<bool> IsExist(Guid id);
    }
}