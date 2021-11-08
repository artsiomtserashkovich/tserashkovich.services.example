using System;

namespace TaaS.Services.Example.Api.Storage.Persistence.Model
{
    public class Printer
    {
        public Guid? Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string OwnerEmail { get; set; }
        
        public PrinterType Type { get; set; }
        
        public double XSize { get; set; }
        
        public double YSize { get; set; }
        
        public double ZSize { get; set; }
    }
}