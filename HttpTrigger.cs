using Kingdee.CDP.WebApi.SDK;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Kingdee.Function;

public class HttpTrigger
{
    private readonly ILogger<HttpTrigger> _logger;

    public HttpTrigger(ILogger<HttpTrigger> logger)
    {
        _logger = logger;
    }

    [Function("Hello")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }

    [Function("KingdeeQuery")]
    public async Task<IActionResult> Query([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            YXKClient client = new YXKClient();
            string result = await client.Query(req);
            return new OkObjectResult(result);
        }
        catch (Exception ex) {
            return new OkObjectResult(ex.ToString());
        }
    }

}

public static class KingdeePatch
{

    public static void Patch()
    {
        MethodInfo? originalMethod = typeof(WebRequestHelper)
            .GetMethod("SetSecurityProtocol", BindingFlags.NonPublic | BindingFlags.Static);
        MethodInfo? replacementMethod = typeof(KingdeePatch)
           .GetMethod("CustomSetSecurityProtocol", BindingFlags.NonPublic | BindingFlags.Static);
        if(originalMethod == null || replacementMethod == null)
        {
            Console.WriteLine("Method not found for patching.");
            return;
        }
        ReplaceMethod(originalMethod, replacementMethod);
    }

    private static void CustomSetSecurityProtocol(Uri uri)
    {
    }
    private static unsafe void ReplaceMethod(MethodInfo original, MethodInfo replacement)
    {
        if (original == null || replacement == null)
        {
            throw new ArgumentNullException("Methods cannot be null.");
        }

        if (original.MethodHandle.Value == IntPtr.Zero || replacement.MethodHandle.Value == IntPtr.Zero)
        {
            throw new InvalidOperationException("Method handles must be valid.");
        }

        // 准备方法以确保它们的指针可用
        RuntimeHelpers.PrepareMethod(original.MethodHandle);
        RuntimeHelpers.PrepareMethod(replacement.MethodHandle);

        // 获取方法指针
        IntPtr* originalMethodPtr = (IntPtr*)original.MethodHandle.Value.ToPointer() + 1;
        IntPtr* replacementMethodPtr = (IntPtr*)replacement.MethodHandle.Value.ToPointer() + 1;

        // 替换方法指针
        *originalMethodPtr = *replacementMethodPtr;
    }
}