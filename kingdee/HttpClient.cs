using System.Net;
using System.Text;

namespace Kingdee.CDP.WebApi.SDK
{
    internal class HttpClient
    {
        internal class CAuther
        {
            internal string AcctID { get; set; }

            internal string AppID { get; set; }

            internal string AppSec { get; set; }

            internal string UserName { get; set; }

            internal int Lcid { get; set; }

            internal string OrgNum { get; set; }

            internal string SID { get; set; }
        }

        private class RequestState
        {
            public Action SentCallBack { get; private set; }

            public ApiRequest Request { get; private set; }

            public Action<AsyncResult<string>> CallBack { get; private set; }

            internal void EndSendRequest(IAsyncResult asyncResult)
            {
                using Stream stream = Request.HttpRequest.EndGetRequestStream(asyncResult);
                byte[] bytes = Request.Encoder.GetBytes(Request.ToJsonString());
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }

            public RequestState(ApiRequest request, Action<AsyncResult<string>> callback, Action aftersent)
            {
                Request = request;
                CallBack = callback;
                SentCallBack = aftersent;
            }
        }

        private CAuther Auther;

        internal Dictionary<string, string> HeaderParam;

        internal void InitAuthInfo(string acctID, string appID, string appSec, string userName, int lcid = 2052, string orgNum = null)
        {
            Auther = new CAuther
            {
                AcctID = acctID,
                AppID = appID,
                AppSec = appSec,
                UserName = userName,
                Lcid = lcid,
                OrgNum = orgNum
            };
        }

        public string GetAutherUserName()
        {
            return Auther.UserName;
        }

        public string GetSessionId()
        {
            return Auther.SID;
        }

        internal void InitAuthInfo(ThirdPassPortInfo thirdPassPortInfo)
        {
            Auther = new CAuther
            {
                AcctID = thirdPassPortInfo.CloudDbId,
                AppID = thirdPassPortInfo.ApiAppId,
                AppSec = thirdPassPortInfo.AppSec,
                UserName = thirdPassPortInfo.CloudUser,
                Lcid = int.Parse(thirdPassPortInfo.Language),
                OrgNum = thirdPassPortInfo.OrgId
            };
        }

        internal void InitHeaderParam(Dictionary<string, string> headerParam)
        {
            HeaderParam = headerParam;
        }

        private void SetAuth(ApiRequest request)
        {
            if (Auther != null)
            {
                string xApiClientID = "";
                string xApiSec = "";
                int num = Auther.AppID.IndexOf("_");
                if (num > -1)
                {
                    xApiClientID = Auther.AppID.Substring(0, num);
                    xApiSec = EnDecode.DecryptAppSecret(Auther.AppID.Substring(num + 1));
                }

                request.AuthInfor = new ApiHeaderContainer
                {
                    XApiClientID = xApiClientID,
                    XApiSec = xApiSec,
                    XKDAppKey = Auther.AppID,
                    XKDAppSec = Auther.AppSec,
                    XKDAcctID = Auther.AcctID,
                    XKDLCID = Auther.Lcid.ToString(),
                    XKDUserName = Auther.UserName,
                    XKDOrgNum = Auther.OrgNum,
                    SID = Auther.SID
                };
            }

            Dictionary<string, string> headerParam = HeaderParam;
            if (headerParam != null && headerParam.Count > 0)
            {
                request.HeaderParam = HeaderParam;
            }
        }

        private void FillCookieOrHeader(HttpWebResponse repo)
        {
            if ((repo.StatusCode == HttpStatusCode.OK || repo.StatusCode == HttpStatusCode.PartialContent) && repo.Headers.AllKeys.Contains("kdservice-sessionid") && Auther != null)
            {
                Auther.SID = repo.Headers["kdservice-sessionid"];
            }
        }

        public string Send(ApiRequest request)
        {
            SetAuth(request);
            using (Stream stream = request.HttpRequest.GetRequestStream())
            {
                byte[] bytes = request.Encoder.GetBytes(request.ToJsonString());
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }

            try
            {
                using HttpWebResponse httpWebResponse = (HttpWebResponse)request.HttpRequest.GetResponse();
                FillCookieOrHeader(httpWebResponse);
                using Stream stream2 = httpWebResponse.GetResponseStream();
                using StreamReader streamReader = new StreamReader(stream2);
                return ValidateResult(streamReader.ReadToEnd());
            }
            catch (WebException ex)
            {
                using StreamReader streamReader2 = new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream(), Encoding.UTF8);
                throw new Exception(streamReader2.ReadToEnd(), ex);
            }
        }

        public void SendAysnc(ApiRequest request, Action<AsyncResult<string>> callback, Action aftersent = null)
        {
            lock (this)
            {
                RequestState state = new RequestState(request, callback, aftersent);
                HttpWebRequest httpRequest = request.HttpRequest;
                SetAuth(request);
                httpRequest.BeginGetRequestStream(BeginGetRequestCallBack, state);
            }
        }

        private void BeginGetRequestCallBack(IAsyncResult asyncResult)
        {
            RequestState reqs = (RequestState)asyncResult.AsyncState;
            SafeDo(reqs, delegate
            {
                reqs.EndSendRequest(asyncResult);
                if (reqs.SentCallBack != null)
                {
                    reqs.SentCallBack();
                }

                reqs.Request.HttpRequest.BeginGetResponse(GetResponseCallback, asyncResult.AsyncState);
            });
        }

        private void GetResponseCallback(IAsyncResult asyncResult)
        {
            RequestState reqs = (RequestState)asyncResult.AsyncState;
            SafeDo(reqs, delegate
            {
                Action<AsyncResult<string>> callBack = reqs.CallBack;
                using HttpWebResponse httpWebResponse = (HttpWebResponse)((RequestState)asyncResult.AsyncState).Request.HttpRequest.EndGetResponse(asyncResult);
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    FillCookieOrHeader(httpWebResponse);
                    using Stream stream = httpWebResponse.GetResponseStream();
                    using StreamReader streamReader = new StreamReader(stream, reqs.Request.Encoder);
                    string result = ValidateResult(streamReader.ReadToEnd());
                    callBack(AsyncResult<string>.CreateSuccess(result));
                    return;
                }

                Exception ex = new Exception(httpWebResponse.StatusDescription);
                callBack(AsyncResult<string>.CreateUnsuccess(ex));
            });
        }

        private static string ValidateResult(string responseText)
        {
            if (responseText.StartsWith("response_error:"))
            {
                string text = responseText.TrimStart("response_error:".ToCharArray());
                if (text == null || text == "")
                {
                    throw new Exception("返回的异常信息为空");
                }

                text = text.Replace("\"", "");
                throw new Exception(text);
            }

            return responseText;
        }

        private void SafeDo(RequestState reqs, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                reqs.CallBack(AsyncResult<string>.CreateUnsuccess(ex));
            }
        }

        private void RequestTimeout(object state, bool timeOut)
        {
            if (timeOut)
            {
                ((WebRequest)state).Abort();
            }
        }
    }
}
