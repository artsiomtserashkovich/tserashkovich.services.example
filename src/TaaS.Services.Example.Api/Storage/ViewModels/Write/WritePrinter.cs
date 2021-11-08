using System.ComponentModel.DataAnnotations;
using TaaS.Services.Example.Api.Storage.Persistence.Model;

namespace TaaS.Services.Example.Api.Storage.ViewModels.Write
{
    public class WritePrinter
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        [Required]
        [EmailAddress]
        public string OwnerEmail { get; set; }
        
        [Required]
        [Range(10, 10000)]
        public double XSize { get; set; }
        
        [Required]
        [Range(10, 10000)]
        public double YSize { get; set; }
        
        [Required]
        [Range(10, 10000)]
        public double ZSize { get; set; }
        
        public PrinterType Type { get; set; }
    }
}