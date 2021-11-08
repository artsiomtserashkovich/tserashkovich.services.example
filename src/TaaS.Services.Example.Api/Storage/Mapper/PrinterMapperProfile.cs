using TaaS.Services.Example.Api.Storage.Persistence.Model;
using TaaS.Services.Example.Api.Storage.ViewModels.Read;
using TaaS.Services.Example.Api.Storage.ViewModels.Write;

namespace TaaS.Services.Example.Api.Storage.Mapper
{
    public class PrinterMapperProfile : AutoMapper.Profile
    {
        public PrinterMapperProfile()
        {
            CreateMap<Printer, WritePrinter>();

            CreateMap<WritePrinter, Printer>();

            CreateMap<Printer, PreviewPrinter>()
                .ConstructUsing(x => 
                    new PreviewPrinter(
                        x.Id.Value,
                        x.Name,
                        x.Type
                    )
                );

            CreateMap<Printer, ViewPrinter>()
                .ConstructUsing(x => 
                    new ViewPrinter(
                        x.Id.Value, 
                        x.Name, 
                        x.Description,
                        x.OwnerEmail,
                        new PrinterSize(x.XSize, x.YSize, x.ZSize),
                        x.Type
                    )
                );
            
            
        }
    }
}