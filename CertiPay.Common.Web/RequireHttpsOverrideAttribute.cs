using System;
using System.Linq;
using System.Web.Mvc;
using RequireHttpsAttributeBase = System.Web.Mvc.RequireHttpsAttribute;

namespace CertiPay.Common.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsOverrideAttribute : RequireHttpsAttributeBase
    {
        // This class is borrowed from the AppHarbor FAQ's.
        // Right now, we mainly use it for the Request.IsLocal check to avoid requiring SSL locally
        // but we'll likely need it in the future for the load balancer header check

        private const String PROTOCOL_HEADER_FIELD = "X-Forwarded-Proto";

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsSecureConnection)
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                return;
            }

            if (string.Equals(filterContext.HttpContext.Request.Headers[PROTOCOL_HEADER_FIELD], Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            //Mimic behavoir from Nancy's SSLProxy to conform with RFC7239
            //http://tools.ietf.org/html/rfc7239#section-1
            if (filterContext.HttpContext.Request.Headers.Get("Forwarded") != null)
            {
                var forwardedHeader = filterContext.HttpContext.Request.Headers.Get("Forwarded").Split(',');
                var protoValue = forwardedHeader.FirstOrDefault(x => x.StartsWith("proto", StringComparison.OrdinalIgnoreCase));
                if (protoValue != null && protoValue.Equals("proto=https", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            HandleNonHttpsRequest(filterContext);
        }
    }
}