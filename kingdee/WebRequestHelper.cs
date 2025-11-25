using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Kingdee.CDP.WebApi.SDK
{
    public class WebRequestHelper
    {
        private static void SetSecurityProtocol(Uri uri)
        {
            if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            }

            ServicePointManager.ServerCertificateValidationCallback = (object _003Cp0_003E, X509Certificate? _003Cp1_003E, X509Chain? _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true;
        }

        public static HttpWebRequest Create(string uri)
        {
            return Create(new Uri(uri));
        }

        public static HttpWebRequest Create(Uri uri)
        {
            SetSecurityProtocol(uri);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.UserAgent = $"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.123; WOW64; Trident/5.0; .NET4.0E; Kingdee/{typeof(WebRequestHelper).Assembly.FullName} MANM)";
            if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                X509Store x509Store = new X509Store(StoreName.My);
                if (x509Store.Certificates.Count == 1)
                {
                    httpWebRequest.ClientCertificates.Add(x509Store.Certificates[0]);
                }
                else if (x509Store.Certificates.Count > 0)
                {
                    X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectName, Environment.MachineName, validOnly: true);
                    if (x509Certificate2Collection.Count > 0)
                    {
                        httpWebRequest.ClientCertificates.Add(x509Certificate2Collection[0]);
                    }

                    httpWebRequest.ClientCertificates.Add(x509Store.Certificates[0]);
                }
            }

            return httpWebRequest;
        }
    }
}
