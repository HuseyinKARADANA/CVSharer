using DinkToPdf;
using DinkToPdf.Contracts;

namespace CVSharer.Services
{
   
    public class PdfGenerator
    {
        private readonly IConverter _converter;

        public PdfGenerator(IConverter converter)
        {
            this._converter = converter;
        }

        public byte[] GeneratorPdf(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                DocumentTitle = "Personal CV"
            };

            var objectSettings = new ObjectSettings
            {
                
                PagesCount = true,
                HtmlContent = htmlContent,
                //Page = "https://www.google.com/",
                WebSettings = { DefaultEncoding = "utf-8" },
                
                FooterSettings = { FontSize = 12, Line = true, Center = "Created By ©CVSharer"  }
            };

            var document = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _converter.Convert(document);
        }
    }
}
