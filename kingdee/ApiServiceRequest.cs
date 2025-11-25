using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Kingdee.CDP.WebApi.SDK
{
    public class ApiServiceRequest : ApiRequest
    {
        public string ServiceName { get; private set; }

        public List<object> Parameters { get; private set; }

        public override Uri ServiceUri
        {
            get
            {
                if (base.IsAsync)
                {
                    return new Uri(new Uri(base.ServerUrl), "a\\" + ServiceName + ".common.kdsvc");
                }

                return new Uri(new Uri(base.ServerUrl), ServiceName + ".common.kdsvc");
            }
        }

        public ApiServiceRequest(string serverUrl, bool async, Encoding encoder, CookieContainer cookies, string servicename, object[] parameters)
            : base(serverUrl, async, encoder, cookies)
        {
            ServiceName = servicename;
            if (parameters == null)
            {
                Parameters = new List<object>();
            }
            else
            {
                Parameters = new List<object>(parameters);
            }
        }

        public override string ToJsonString()
        {
            SetValue("rid", base.RequestId);
            SetValue("parameters", JsonConvert.SerializeObject(Parameters));
            return base.ToJsonString();
        }
    }
}
