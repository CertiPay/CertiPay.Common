using CertiPay.Common.Logging;
using System.Web.Mvc;

namespace CertiPay.Common.Web
{
    public sealed class ExceptionLoggingFilter : IExceptionFilter
    {
        private static readonly ILog Log = LogManager.GetLogger("UncaughtExceptions");

        public void OnException(ExceptionContext ctx)
        {
            Log.ErrorException("Unhandled Exception on Request: " + ctx.HttpContext.Request.Url + " : " + ctx.Exception.Message, ctx.Exception);
        }
    }
}