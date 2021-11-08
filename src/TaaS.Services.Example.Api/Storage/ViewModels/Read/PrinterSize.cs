namespace TaaS.Services.Example.Api.Storage.ViewModels.Read
{
    public class PrinterSize
    {
        public PrinterSize(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
            
        public double X { get; private set; }
        
        public double Y { get; private set; }
        
        public double Z { get; private set; }
    }
}