﻿using CertiPay.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSupergoo.ABCpdf10;

namespace CertiPay.PDF
{
    public interface IPDFService
    {
        /// <summary>
        /// Return the data for a PDF representing the provided settings, including a list of URIs
        /// </summary>
        byte[] CreatePdf(PDFService.Settings settings);
    }

    public class PDFService : IPDFService
    {
        private static readonly ILog Log = LogManager.GetLogger<PDFService>();

        public PDFService()
        {
            // Run without installing a license key
        }

        public PDFService(String abcPdfLicenseKey)
        {
            if (XSettings.LicenseType != LicenseType.Professional || !XSettings.LicenseValid)
            {
                XSettings.InstallLicense(abcPdfLicenseKey);
            }
        }

        public byte[] CreatePdf(PDFService.Settings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (!settings.Uris.Any()) throw new ArgumentException("No URIs provided to create PDF from");

            using (Doc pdf = new Doc())
            {
                pdf.HtmlOptions.Engine = EngineType.Gecko;
                pdf.HtmlOptions.AddLinks = true;
                pdf.HtmlOptions.Timeout = (int)settings.Timeout.TotalMilliseconds;
                pdf.HtmlOptions.RetryCount = settings.RetryCount;
                pdf.HtmlOptions.UseScript = settings.UseScript;
                pdf.HtmlOptions.PageCacheClear();
                pdf.HtmlOptions.PageCacheEnabled = false;

                pdf.Color.Red = 255;
                pdf.Color.Green = 255;
                pdf.Color.Blue = 255;
                pdf.Rect.Inset(10, 10);
                pdf.FillRect();

                // If selected, make the PDF in landscape format
                if (settings.UseLandscapeOrientation)
                {
                    pdf.Transform.Rotate(90, pdf.MediaBox.Left, pdf.MediaBox.Bottom);
                    pdf.Transform.Translate(pdf.MediaBox.Width, 0);
                    pdf.Rect.Width = pdf.MediaBox.Height;
                    pdf.Rect.Height = pdf.MediaBox.Width;
                }

                int imageId = 0;

                // For each URI provided, add the result to the output doc
                foreach (String uri in settings.Uris)
                {
                    if (imageId != 0)
                    {
                        pdf.Page = pdf.AddPage();
                    }

                    // Render the web page by uri and return the image id for chaining
                    imageId = pdf.AddImageUrl(uri, paged: true, width: 0, disableCache: false);

                    while (true)
                    {
                        // Stop when we reach a page which wasn't truncated, per the examples
                        if (!pdf.Chainable(imageId)) break;

                        // Add a page to the pdf and sets the page id
                        pdf.Page = pdf.AddPage();

                        // Add the previous image to the chain and set the image id
                        imageId = pdf.AddImageToChain(imageId);
                    }
                }

                // flatten the pages
                for (var ii = 1; ii <= pdf.PageCount; ii++)
                {
                    pdf.PageNumber = ii;
                    pdf.Flatten();
                }

                // Return the byte array representing the pdf
                return pdf.GetData();
            }
        }

        public class Settings
        {
            /// <summary>
            /// A list of Uris from which to generate the PDF from
            /// </summary>
            public ICollection<String> Uris { get; set; }

            /// <summary>
            /// HTML rendering can take some time.
            /// If the time taken exceeds the Timeout then the page is assumed to be unavailable. Depending on the RetryCount settings the page may be re-requested or an error may be returned.
            /// This value is measured in milliseconds.
            /// </summary>
            /// <remarks>
            /// ABCPDF defaults this to 15 seconds, we changed it to 60 seconds
            /// </remarks>
            public TimeSpan Timeout { get; set; }

            /// <summary>
            /// This property controls how many times ABCpdf will attempt to obtain a page.
            /// HTML rendering may fail one time but succeed the next. This is often for reasons outside the control of ABCpdf.
            /// So ABCpdf may attempt to re-request a page if it is not immediately available. This is analogous to clicking on the refresh button of a web browser if the page is failing to load.
            /// See the ContentCount and the Timeout properties for how ABCpdf determines if a page is unavailable or invalid.
            /// </summary>
            /// <remarks>
            /// ABCPDF defaults this to 5, but we change it to 1
            /// </remarks>
            public int RetryCount { get; set; }

            /// <summary>
            /// The minimum number of items a page of HTML should contain.
            /// If the number is less than this value then the page will be assumed to be invalid.
            /// </summary>
            /// <remarks>
            /// ABCPDF defaults this to 36
            /// </remarks>
            public int ContentCount { get; set; }

            /// <summary>
            /// This property determines whether JavaScript and VBScript are enabled.
            /// By default, client-side script such as JavaScript is disabled when rendering HTML documents.
            /// This is done for good security reasons, and we strongly recommend that you do not change this setting.
            /// However, if you are sure that your source documents do not pose a security risk, you can enable Script using this setting.
            /// </summary>
            /// <remarks>
            /// ABCPDF defaults this to false, but we default it to true
            /// </remarks>
            public Boolean UseScript { get; set; }

            /// <summary>
            /// Set this property to true to generate landscape oriented output files
            /// </summary>
            /// <remarks>
            public Boolean UseLandscapeOrientation { get; set; }

            public Settings()
            {
                this.Uris = new List<String>();
                this.Timeout = TimeSpan.FromSeconds(60);
                this.RetryCount = 1;
                this.ContentCount = 36;
                this.UseScript = true;
                this.UseLandscapeOrientation = false;
            }
        }
    }
}