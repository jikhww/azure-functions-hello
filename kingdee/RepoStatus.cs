namespace Kingdee.CDP.WebApi.SDK
{
    public class RepoStatus : BaseEntify
    {
        public string ErrorCode { get; set; }

        public bool IsSuccess { get; set; }

        public IList<RepoError> Errors { get; set; }

        public IList<SuccessEntity> SuccessEntitys { get; set; }
    }
}
