using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;

namespace Kingdee.CDP.WebApi.SDK
{
    public class ApiRequest : JsonObject
    {
        private static bool HasSetSecurityProtocol = false;

        private static string fmtProp = "format";

        private static string uaProp = "useragent";

        private HttpWebRequest httpRequest;

        public bool IsAsync { get; private set; }

        private int Format
        {
            get
            {
                return GetValue<int>(fmtProp);
            }
            set
            {
                SetValue(fmtProp, value);
            }
        }

        internal ApiHeaderContainer AuthInfor { get; set; }

        internal Dictionary<string, string> HeaderParam { get; set; }

        public CookieContainer CookiesContainer { get; private set; }

        public Encoding Encoder { get; private set; }

        public string ServerUrl { get; private set; }

        public virtual Uri ServiceUri
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string RequestId { get; protected set; }

        public bool AutoGetLastProgress { get; set; }

        private string UserAgent
        {
            get
            {
                return GetValue<string>(uaProp);
            }
            set
            {
                SetValue(uaProp, value);
            }
        }

        public string Version { get; set; }

        internal HttpWebRequest HttpRequest
        {
            get
            {
                lock (this)
                {
                    if (httpRequest == null)
                    {
                        httpRequest = CreateRequest();
                    }

                    Dictionary<string, string> headerParam = HeaderParam;
                    if (headerParam != null && headerParam.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> item in HeaderParam)
                        {
                            httpRequest.Headers.Set(item.Key, item.Value);
                        }
                    }
                    else if (AuthInfor != null)
                    {
                        foreach (KeyValuePair<string, string> header in AuthInfor.GetHeaders(ServiceUri))
                        {
                            httpRequest.Headers.Set(header.Key, header.Value);
                        }
                    }

                    return httpRequest;
                }
            }
        }

        private static void SetSecurityProtocol(Uri uri)
        {
            if (!HasSetSecurityProtocol)
            {
                try
                {
                    if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    HasSetSecurityProtocol = true;
                }
            }

            ServicePointManager.ServerCertificateValidationCallback = (ServicePointManager.ServerCertificateValidationCallback = (object _003Cp0_003E, X509Certificate? _003Cp1_003E, X509Chain? _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true);
        }

        public virtual string ToJsonString()
        {
            SetValue("timestamp", DateTime.Now);
            SetValue("v", Version);
            return ToString();
        }

        public ApiRequest(string serverUrl, bool async, Encoding encoder, CookieContainer cookies)
        {
            Format = 1;
            UserAgent = "ApiClient";
            ServerUrl = serverUrl;
            Encoder = encoder;
            CookiesContainer = cookies;
            RequestId = Guid.NewGuid().ToString().GetHashCode()
                .ToString();
            AutoGetLastProgress = true;
            IsAsync = async;
            Version = "1.0";
        }

        private HttpWebRequest CreateRequest()
        {
            HttpWebRequest httpWebRequest = WebRequestHelper.Create(ServiceUri);
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("Accept-Charset", Encoder.HeaderName);
            httpWebRequest.CookieContainer = CookiesContainer;
            httpWebRequest.Pipelined = true;
            return httpWebRequest;
        }

        public void Abort()
        {
            httpRequest.Abort();
        }

        private static string GetCerFile()
        {
            string text = "";
            if (Environment.UserInteractive)
            {
                text = TryToGetCerFile(AppDomain.CurrentDomain.BaseDirectory);
                if (!File.Exists(text))
                {
                    text = TryToGetCerFile(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName);
                }
            }
            else
            {
                text = TryToGetCerFile(AppDomain.CurrentDomain.BaseDirectory);
                if (!File.Exists(text))
                {
                    text = TryToGetCerFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"));
                }
            }

            return text;
        }

        private static string TryToGetCerFile(string dirName)
        {
            string text = Path.Combine(dirName, Environment.MachineName + ".cer");
            if (File.Exists(text))
            {
                return text;
            }

            return Path.Combine(dirName, "K3CloudCert.cer");
        }
    }
}
