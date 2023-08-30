using CVSharer.Services;
using Microsoft.AspNetCore.Mvc;



namespace CVSharer.Controllers
{
    public class DashboardController : Controller
    {
        private readonly PdfGenerator _pdfGenerator;

        public DashboardController(PdfGenerator pdfGenerator)
        {
            _pdfGenerator = pdfGenerator;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string s)
        {
            string htmlContent = "<html><body><h1>Hello, World!</h1>" +
                "<img src='https://cdn.pixabay.com/photo/2014/02/27/16/10/flowers-276014_1280.jpg' " +
                "alt='Example Image'></body></html>";


            byte[] pdfBytes = _pdfGenerator.GeneratorPdf(htmlContent);

            return File(pdfBytes, "application/pdf", "generated.pdf");
        }


        
        
    }
}
