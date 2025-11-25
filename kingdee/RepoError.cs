namespace Kingdee.CDP.WebApi.SDK
{
    public class RepoError : BaseEntify
    {
        public virtual string FieldName { get; set; }

        public virtual string Message { get; set; }

        public virtual int DIndex { get; set; }
    }
}
