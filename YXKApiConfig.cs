namespace Kingdee.Function;

public class YXKApiConfig
{
    public string Server { get; set; } = null!;
    public string AppId { get; set; } = null!;
    public string AppSecret { get; set; } = null!;
    public string AccountId { get; set; } = null!;
    public string LoginUser { get; set; } = null!;
    public string CompanyCode { get; set; } = null!; // 公司编码，用于区分不同的公司
}
