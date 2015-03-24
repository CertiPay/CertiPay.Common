﻿using NUnit.Framework;
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
    }
}