using System.Collections.Generic;
using System.Threading.Tasks;
using TaaS.Services.Example.Api.Storage.Persistence.Model;

namespace TaaS.Services.Example.Api.Storage.Persistence.Query
{
    public interface IGetPreviewPrintersQuery
    {
        Task<IReadOnlyCollection<Printer>> Execute();
    }
}