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

        /// <summary>
        /// Returns information about the license being used by the PDF generator
        /// </summary>
        String LicenseDescription { get; }
    }

    public class PDFService : IPDFService
    {
        public PDFService()
        {
            // Run without installing a license key
        }

        public PDFService(String abcPdfLicenseKey)
        {
            if (XSettings.LicenseType != LicenseType.Professional)
            {
                XSettings.InstallLicense(abcPdfLicenseKey);
            }
        }

        public String LicenseDescription { get { return XSettings.LicenseDescription; } }

        public byte[] CreatePdf(PDFService.Settings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (!settings.Uris.Any()) throw new ArgumentException("No URIs provided to create PDF from");

            using (Doc pdf = new Doc())
            {
                pdf.Rect.Inset(20, 20);

                pdf.HtmlOptions.Engine = settings.UseMSHtmlEngine ? EngineType.MSHtml : EngineType.Gecko;

                pdf.HtmlOptions.Timeout = (int)settings.Timeout.TotalMilliseconds;
                pdf.HtmlOptions.RetryCount = settings.RetryCount;

                pdf.HtmlOptions.PageCacheClear();
                pdf.HtmlOptions.PageCacheEnabled = false;

                pdf.HtmlOptions.AddForms = settings.UseForms;
                pdf.HtmlOptions.AddLinks = settings.UseLinks;
                pdf.HtmlOptions.UseScript = settings.UseScript;

                //Help with rendering css on markup
                pdf.HtmlOptions.PageLoadMethod = PageLoadMethodType.WebBrowserNavigate;
                pdf.HtmlOptions.DoMarkup = true;

                // If selected, make the PDF in landscape format
                if (settings.UseLandscapeOrientation)
                {
                    pdf.Transform.Rotate(90, pdf.MediaBox.Left, pdf.MediaBox.Bottom);
                    pdf.Transform.Translate(pdf.MediaBox.Width, 0);
                    pdf.Rect.Width = pdf.MediaBox.Height;
                    pdf.Rect.Height = pdf.MediaBox.Width;

                    //Need to adjust the rotation of the default page to show in landscape
                    int theID = pdf.GetInfoInt(pdf.Root, "Pages");
                    pdf.SetInfo(theID, "/Rotate", "90");

                }

                int imageId = 0;

                //Hard coding the page height for a good looking PDF from URL
                int intHTMLWidth = Convert.ToInt32(1175 * Convert.ToDouble(pdf.Rect.Width / pdf.Rect.Height));
                pdf.HtmlOptions.BrowserWidth = intHTMLWidth;

                // For each URI provided, add the result to the output doc
                foreach (String uri in settings.Uris)
                {
                    if (imageId != 0)
                    {
                        pdf.Page = pdf.AddPage();
                    }

                    // Render the web page by uri and return the image id for chaining
                    imageId = pdf.AddImageUrl(url: uri, paged: true, width: intHTMLWidth, disableCache: true);

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

            /// <summary>
            /// Set this property to true to make forms active on rendered pages
            /// </summary>
            public Boolean UseForms { get; set; }

            /// <summary>
            /// Set this property to true to make links active on rendered pages
            /// </summary>
            public Boolean UseLinks { get; set; }

            /// <summary>
            /// Set this property to true to use the MSHTML rendering engine instead of GECKO
            /// </summary>
            public Boolean UseMSHtmlEngine { get; set; }

            public Settings()
            {
                this.Uris = new List<String>();
                this.Timeout = TimeSpan.FromSeconds(60);
                this.RetryCount = 1;
                this.ContentCount = 36;
                this.UseScript = false;
                this.UseLandscapeOrientation = false;
                this.UseForms = false;
                this.UseLinks = false;
                this.UseMSHtmlEngine = false;
            }
        }
    }
}