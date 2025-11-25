using Newtonsoft.Json;

namespace Kingdee.CDP.WebApi.SDK
{
    public class BaseEntify
    {
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static T Parse<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
