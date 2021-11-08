using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaaS.Services.Example.Api.Storage.Persistence.Database;
using TaaS.Services.Example.Api.Storage.Persistence.Model;

namespace TaaS.Services.Example.Api.Storage.Persistence.Query
{
    public class GetPreviewPrintersQuery : IGetPreviewPrintersQuery
    {
        private readonly PrintersDatabaseContext _databaseContext;

        public GetPreviewPrintersQuery(PrintersDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IReadOnlyCollection<Printer>> Execute()
        {
            return await _databaseContext.Printers
                .OrderBy(x => x.Name)
                .Select(x => new Printer {Id = x.Id, Name = x.Name, Type = x.Type})
                .AsNoTracking()
                .ToListAsync();
        }
    }
}