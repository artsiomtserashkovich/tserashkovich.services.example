using System.Collections.Generic;

namespace TaaS.Services.Example.Api.Storage.ViewModels.Read
{
    public class PrinterList
    {
        public PrinterList(IReadOnlyCollection<PreviewPrinter> printers)
        {
            Printers = printers ?? new List<PreviewPrinter>();
        }

        public IReadOnlyCollection<PreviewPrinter> Printers { get; private set; }
    }
}