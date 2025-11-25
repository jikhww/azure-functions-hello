using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Kingdee.CDP.WebApi.SDK
{

    public class ApiClient
    {
        public delegate void FailCallBackHandler(Exception ex);

        private HttpClient httpClient;

        private string serverUrl;

        private FailCallBackHandler defaultFailCallBack;

        private CookieContainer CookieContainer;

        private Encoding Encoder;

        private const string X_KDApi_ServerUrl = "X-KDApi-ServerUrl";

        private const string X_KDApi_TimeOut = "X-KDApi-TimeOut";

        private int m_defaultTimeOut;

        protected int DefaultTimeOut
        {
            get
            {
                if (m_defaultTimeOut == 0)
                {
                    string value = ApiSettingsHelper.GetSection("X-KDApi-TimeOut").Value;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        m_defaultTimeOut = int.Parse(value);
                    }
                    else
                    {
                        m_defaultTimeOut = 300000;
                    }
                }

                return m_defaultTimeOut;
            }
            private set
            {
                m_defaultTimeOut = value;
            }
        }

        protected string GetServerUrl()
        {
            string value = ApiSettingsHelper.GetSection("X-KDApi-ServerUrl").Value;
            if (value != null)
            {
                if (!value.EndsWith("/"))
                {
                    return value + "/";
                }

                return value;
            }

            throw new ArgumentException("ServerUrl is required for ApiClient!");
        }

        public ApiClient()
            : this(null)
        {
        }

        public ApiClient(string serverUrl)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                serverUrl = GetServerUrl();
            }

            Encoder = new UTF8Encoding();
            this.serverUrl = (serverUrl.EndsWith("/") ? serverUrl : (serverUrl + "/"));
            CookieContainer = new CookieContainer();
            httpClient = new HttpClient();
            LoadConfig(httpClient);
        }

        public ApiClient(ThirdPassPortInfo authInfo, int timeout)
        {
            m_defaultTimeOut = timeout * 1000;
            Encoder = new UTF8Encoding();
            CookieContainer = new CookieContainer();
            serverUrl = (authInfo.CloudUrl.EndsWith("/") ? authInfo.CloudUrl : (authInfo.CloudUrl + "/"));
            httpClient = new HttpClient();
            httpClient.InitAuthInfo(authInfo);
        }

        public ApiClient(ThirdPassPortInfo authInfo, int timeout, Dictionary<string, string> headerParam)
            : this(authInfo, timeout)
        {
            httpClient.InitHeaderParam(headerParam);
        }

        public ApiClient(string serverUrl, int timeout)
            : this(serverUrl)
        {
            m_defaultTimeOut = timeout * 1000;
        }

        public ApiClient(string serverUrl, FailCallBackHandler defaultfailCallBack)
            : this(serverUrl)
        {
            defaultFailCallBack = defaultfailCallBack;
        }

        public ApiClient(string serverUrl, FailCallBackHandler defaultfailCallBack, int timeout)
            : this(serverUrl, defaultfailCallBack)
        {
            m_defaultTimeOut = timeout * 1000;
        }

        public string GetAutherUserName()
        {
            return httpClient.GetAutherUserName();
        }

        public string GetSessionId()
        {
            return httpClient.GetSessionId();
        }

        private void LoadConfig(HttpClient client)
        {
            string value = ApiSettingsHelper.GetSection("X-KDApi-AppID").Value;
            StringBuilder stringBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(value))
            {
                string value2 = ApiSettingsHelper.GetSection("X-KDApi-AppSec").Value;
                string value3 = ApiSettingsHelper.GetSection("X-KDApi-UserName").Value;
                string value4 = ApiSettingsHelper.GetSection("X-KDApi-AcctID").Value;
                string text = ApiSettingsHelper.GetSection("X-KDApi-LCID").Value;
                string value5 = ApiSettingsHelper.GetSection("X-KDApi-OrgNum").Value;
                if (string.IsNullOrWhiteSpace(value2))
                {
                    stringBuilder.AppendLine("X-KDApi-AppSec is required for ApiClient!");
                }

                if (string.IsNullOrWhiteSpace(value3))
                {
                    stringBuilder.AppendLine("X-KDApi-UserName is required for ApiClient!");
                }

                if (string.IsNullOrWhiteSpace(value4))
                {
                    stringBuilder.AppendLine("X-KDApi-AcctID is required for ApiClient!");
                }

                if (string.IsNullOrWhiteSpace(text))
                {
                    text = "2052";
                }

                if (stringBuilder.Length != 0)
                {
                    throw new ArgumentException(stringBuilder.ToString());
                }

                client.InitAuthInfo(value4, value, value2, value3, int.Parse(text), value5);
            }
        }

        public void InitClient(string acctID, string appID, string appSec, string userName, int lcid = 2052, string orgNum = null, string serverUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(serverUrl))
            {
                this.serverUrl = (serverUrl.EndsWith("/") ? serverUrl : (serverUrl + "/"));
            }

            httpClient.InitAuthInfo(acctID, appID, appSec, userName, lcid, orgNum);
        }

        public bool Logout()
        {
            return Execute<bool>("Kingdee.BOS.WebApi.ServicesStub.AuthService.Logout", null);
        }

        public bool Login(string dbId, string userName, string password, int lcid)
        {
            object[] parameters = new object[4]
            {
            dbId,
            EnDecode.Encode(userName),
            EnDecode.Encode(password),
            lcid
            };
            if (JObject.Parse(Execute<string>("Kingdee.BOS.WebApi.ServicesStub.AuthService.ValidateUserEnDeCode", parameters))["LoginResultType"].Value<int>() == 1)
            {
                return true;
            }

            return false;
        }

        public string ValidateLogin(string dbId, string userName, string password, int lcid)
        {
            object[] parameters = new object[4]
            {
            dbId,
            EnDecode.Encode(userName),
            EnDecode.Encode(password),
            lcid
            };
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.AuthService.ValidateUserEnDeCode", parameters);
        }

        public string LoginByAppSecret(string dbId, string userName, string appId, string appSecret, int lcid)
        {
            object[] parameters = new object[5] { dbId, userName, appId, appSecret, lcid };
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.AuthService.LoginByAppSecret", parameters);
        }

        public string LoginBySign(string dbId, string userName, string appId, long timestamp, string sign, int lcid)
        {
            object[] parameters = new object[6] { dbId, userName, appId, timestamp, sign, lcid };
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.AuthService.LoginBySign", parameters);
        }

        public ApiRequest CreateRequest(string servicename, object[] parameters = null)
        {
            return new ApiServiceRequest(serverUrl, async: false, Encoder, CookieContainer, servicename, parameters);
        }

        public T Execute<T>(string servicename, object[] parameters = null)
        {
            return Execute<T>(servicename, parameters, null, DefaultTimeOut);
        }

        public T Execute<T>(string servicename, object[] parameters = null, FailCallBackHandler failcallback = null, int timeout = 300)
        {
            ApiRequest apiRequest = CreateRequest(servicename, parameters);
            apiRequest.HttpRequest.Timeout = timeout * 1000;
            return Call<T>(apiRequest, failcallback);
        }

        public string Call(ApiRequest request, FailCallBackHandler failCallback = null)
        {
            return SafeDo(() => httpClient.Send(request), GetReallyFailCallBack(failCallback));
        }

        public T Call<T>(ApiRequest request, FailCallBackHandler failCallback = null)
        {
            return SafeDo(delegate
            {
                string text = httpClient.Send(request);
                try
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        return default(T);
                    }

                    return JsonObject.Deserialize<T>(text);
                }
                catch (Exception innerException)
                {
                    throw new Exception(text, innerException);
                }
            }, GetReallyFailCallBack(failCallback));
        }

        private FailCallBackHandler GetReallyFailCallBack(FailCallBackHandler failCallback)
        {
            if (failCallback != null)
            {
                return failCallback;
            }

            if (defaultFailCallBack != null)
            {
                return defaultFailCallBack;
            }

            return null;
        }

        private T SafeDo<T>(Func<T> action, FailCallBackHandler failCallback)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                failCallback?.Invoke(ex);
                if (0 == 0)
                {
                    throw;
                }
            }

            return default(T);
        }
    }
}
