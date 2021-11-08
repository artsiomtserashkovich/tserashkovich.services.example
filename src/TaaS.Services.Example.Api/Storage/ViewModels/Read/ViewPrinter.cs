using System;
using TaaS.Services.Example.Api.Storage.Persistence.Model;

namespace TaaS.Services.Example.Api.Storage.ViewModels.Read
{
    public class ViewPrinter
    {
        public ViewPrinter(
            Guid id, 
            string name, 
            string description, 
            string ownerEmail, 
            PrinterSize size, 
            PrinterType type)
        {
            Id = id; 
            Name = name;
            Description = description;
            OwnerEmail = ownerEmail;
            Size = size;
            Type = type;
        }

        public Guid Id { get; private set; }
        
        public string Name { get; private set; }
        
        public string Description { get; private set; }
        
        public string OwnerEmail { get; private set; }
        
        public PrinterSize Size { get; private set; }
        
        public PrinterType Type { get; private set; }
    }
}