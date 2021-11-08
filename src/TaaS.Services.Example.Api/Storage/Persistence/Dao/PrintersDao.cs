using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaaS.Services.Example.Api.Storage.Persistence.Database;
using TaaS.Services.Example.Api.Storage.Persistence.Model;

namespace TaaS.Services.Example.Api.Storage.Persistence.Dao
{
    public class PrintersDao : IPrintersDao
    {
        private readonly PrintersDatabaseContext _databaseContext;

        public PrintersDao(PrintersDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public IAsyncEnumerable<Printer> ReadAll()
        {
            return _databaseContext.Printers.AsAsyncEnumerable();
        }

        public ValueTask<Printer> Read(Guid id)
        {
            return _databaseContext.Printers.FindAsync(id);
        }

        public async Task<Guid> Create(Printer printer)
        {
            printer.Id ??= Guid.NewGuid();

            await _databaseContext.Printers.AddAsync(printer);
            await _databaseContext.SaveChangesAsync();

            return printer.Id.Value;
        }

        public async Task Delete(Guid id)
        {
            var printer = new Printer {Id  = id};
            _databaseContext.Attach(printer);
            _databaseContext.Printers.Remove(printer);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task Edit(Printer printer)
        {
            _databaseContext.Printers.Attach(printer); 
            _databaseContext.Entry(printer).State = EntityState.Modified;
            await _databaseContext.SaveChangesAsync();
        }

        public Task<bool> IsExist(Guid id)
        {
            return _databaseContext.Printers.AnyAsync(x => x.Id == id);
        }
    }
}