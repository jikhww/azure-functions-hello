using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;

namespace Kingdee.CDP.WebApi.SDK
{
    public class K3CloudApi : ApiClient
    {
        public K3CloudApi()
        {
        }

        public K3CloudApi(string serverUrl)
            : base(serverUrl)
        {
        }

        public K3CloudApi(string serverUrl, int timeout)
            : base(serverUrl, timeout)
        {
        }

        public K3CloudApi(ThirdPassPortInfo thirdPassPortInfo, int timeout)
            : base(thirdPassPortInfo, timeout)
        {
        }

        public K3CloudApi(ThirdPassPortInfo thirdPassPortInfo, int timeout, Dictionary<string, string> headerParam)
            : base(thirdPassPortInfo, timeout, headerParam)
        {
        }

        public string Save(string formId, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Save", new object[2] { formId, data });
        }

        public string BatchSave(string formId, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.BatchSave", new object[2] { formId, data });
        }

        public string Audit(string formId, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Audit", new object[2] { formId, data });
        }

        public string Delete(string formId, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Delete", new object[2] { formId, data });
        }

        public string UnAudit(string formId, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.UnAudit", new object[2] { formId, data });
        }

        public string Submit(string formId, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Submit", new object[2] { formId, data });
        }

        public string View(string formId, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.View", new object[2] { formId, data });
        }

        public List<List<object>> ExecuteBillQuery(string data)
        {
            return Execute<List<List<object>>>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.ExecuteBillQuery", new object[1] { data });
        }

        public string BillQuery(string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.BillQuery", new object[1] { data });
        }

        public string Draft(string formid, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Draft", new object[2] { formid, data });
        }

        public string Allocate(string formid, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Allocate", new object[2] { formid, data });
        }

        public string Push(string formid, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Push", new object[2] { formid, data });
        }

        public string FlexSave(string formid, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.FlexSave", new object[2] { formid, data });
        }

        public string SendMsg(string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.SendMsg", new object[1] { data });
        }

        public string GroupSave(string formid, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.GroupSave", new object[2] { formid, data });
        }

        public string Disassembly(string formid, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Disassembly", new object[2] { formid, data });
        }

        public string QueryBusinessInfo(string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.QueryBusinessInfo", new object[1] { data });
        }

        public string QueryGroupInfo(string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.QueryGroupInfo", new object[1] { data });
        }

        public string GroupDelete(string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.GroupDelete", new object[1] { data });
        }

        public string WorkflowAudit(string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.WorkflowAudit", new object[1] { data });
        }

        public string ExcuteOperation(string formid, string opNumber, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.ExcuteOperation", new object[3] { formid, opNumber, data });
        }

        public string SwitchOrg(string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.SwitchOrg", new object[1] { data });
        }

        public string CancelAllocate(string formid, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.CancelAllocate", new object[2] { formid, data });
        }

        public string CancelAssign(string formid, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.CancelAssign", new object[2] { formid, data });
        }

        public string GetSysReportData(string formid, string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.GetSysReportData", new object[2] { formid, data });
        }

        public string AttachmentUpLoad(string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.AttachmentUpLoad", new object[1] { data });
        }

        public string AttachmentDownLoad(string data)
        {
            return Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.AttachmentDownLoad", new object[1] { data });
        }

        public RepoResult CheckAuthInfo()
        {
            string filterString = "FNUMBER='PRE001'";
            QueryParam value = new QueryParam
            {
                FormId = "BD_Currency",
                FieldKeys = "FCODE",
                FilterString = filterString
            };
            try
            {
                if (Execute<string>("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.ExecuteBillQuery", new object[1] { JsonConvert.SerializeObject(value) }).IndexOf("\"CNY\"") > -1)
                {
                    return new RepoResult
                    {
                        ResponseStatus = new RepoStatus
                        {
                            IsSuccess = true
                        }
                    };
                }

                return new RepoResult
                {
                    ResponseStatus = new RepoStatus
                    {
                        IsSuccess = false,
                        ErrorCode = "500",
                        Errors = new List<RepoError>
                    {
                        new RepoError
                        {
                            Message = "The query fails！User authentication information is wrong！"
                        }
                    }
                    }
                };
            }
            catch (Exception ex)
            {
                return new RepoResult
                {
                    ResponseStatus = new RepoStatus
                    {
                        IsSuccess = false,
                        ErrorCode = "500",
                        Errors = new List<RepoError>
                    {
                        new RepoError
                        {
                            Message = ex.Message
                        }
                    }
                    }
                };
            }
        }
    }
}
