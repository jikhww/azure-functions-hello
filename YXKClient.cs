using Kingdee.CDP.WebApi.SDK;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Kingdee.Function;

public class YXKClient
{
    private K3CloudApi CreateClient(YXKApiConfig config)
    {
        var client = new K3CloudApi(config.Server);
        client.InitClient(config.AccountId, config.AppId, config.AppSecret, config.LoginUser, orgNum: config.CompanyCode);
        return client;
    }
    public async Task<string> Query(HttpRequest request)
    {
        using(var reader = new StreamReader(request.Body))
        {
            string body = await reader.ReadToEndAsync();
            RequestBody? requestBody = System.Text.Json.JsonSerializer.Deserialize<RequestBody>(body);
            if(requestBody.IsNull() || requestBody!.Config.IsNull() || requestBody!.Body.IsNull())
            {
                throw new Exception("Invalidate Parameters!");
            }
            return InternalQuery(requestBody.Config, requestBody.Body);
        }
    }
    private string InternalQuery(YXKApiConfig config, QueryBody body)
    {
        var client = CreateClient(config);
        var bodyStr = System.Text.Json.JsonSerializer.Serialize(body);
        return client.BillQuery(bodyStr);
    }
}

public class RequestBody
{
    public YXKApiConfig Config { get; set; } = null!;
    public QueryBody Body { get; set; } = null!;
}

public class QueryBody
{
    public string FilterString { get; set; } = string.Empty;
    public string FieldKeys { get; set; } = string.Empty;
    public string FormId { get; set; } = null!;
    public int StartRow { get; set; } = 0;
    public int Limit { get; set; } = 0;
}