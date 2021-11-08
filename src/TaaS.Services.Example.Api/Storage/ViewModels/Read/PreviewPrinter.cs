using System;
using TaaS.Services.Example.Api.Storage.Persistence.Model;

namespace TaaS.Services.Example.Api.Storage.ViewModels.Read
{
    public class PreviewPrinter
    {
        public PreviewPrinter(Guid id, string name, PrinterType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public Guid Id { get; private set; }
        
        public string Name { get; private set; }
        
        public PrinterType Type { get; private set; }
        
    }
}