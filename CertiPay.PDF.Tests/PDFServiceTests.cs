using NUnit.Framework;
using System.IO;

namespace CertiPay.PDF.Tests
{
    public class PDFServiceTests
    {
        [Test]
        public void ShouldGenerateMultiPagePDF()
        {
            IPDFService svc = new PDFService();

            byte[] output = svc.CreatePdf(new PDFService.Settings
            {
                Uris = new[]
                    {
                        @"http://google.com",
                        @"http://github.com"
                    }
            });

            File.WriteAllBytes("Output.pdf", output);
        }

        [Test]
        public void Should_Generate_Landscape_PDF()
        {
            IPDFService svc = new PDFService { };

            byte[] output = svc.CreatePdf(new PDFService.Settings
            {
                Uris = new[] { "http://google.com" },
                UseLandscapeOrientation = true
            });

            File.WriteAllBytes("Output-Landscape.pdf", output);
        }

        [Test]
        public void Should_Generate_Live_Form_PDF()
        {
            IPDFService svc = new PDFService { };

            byte[] output = svc.CreatePdf(new PDFService.Settings
            {
                Uris = new[] { "http://google.com" },
                UseForms = true
            });

            File.WriteAllBytes("Output-Form.pdf", output);
        }

        [Test]
        public void Should_Generate_Live_Links_PDF()
        {
            IPDFService svc = new PDFService { };

            byte[] output = svc.CreatePdf(new PDFService.Settings
            {
                Uris = new[] { "http://google.com" },
                UseLinks = true
            });

            File.WriteAllBytes("Output-Links.pdf", output);
        }

        [Test, Ignore("Integration Test")]
        public void Should_Install_and_Use_License_Key()
        {
            IPDFService svc = new PDFService(abcPdfLicenseKey: "put-license-to-check-here") { };

            byte[] output = svc.CreatePdf(new PDFService.Settings
            {
                Uris = new[] { "http://google.com" },
                UseLinks = true
            });

            File.WriteAllBytes("Output-WithLicense.pdf", output);
        }
    }
}