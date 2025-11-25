namespace Kingdee.CDP.WebApi.SDK
{
    public class QueryParam : BaseEntify
    {
        public string FormId { get; set; }

        public string FieldKeys { get; set; }

        public string FilterString { get; set; }

        public string OrderString { get; set; }

        public int StartRow { get; set; }

        public int Limit { get; set; }

        public int TopRowCount { get; set; }
    }
}
