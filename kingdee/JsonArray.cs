using Newtonsoft.Json.Linq;
using System.Collections;

namespace Kingdee.CDP.WebApi.SDK
{
    public class JsonArray : IEnumerable<JsonObject>, IEnumerable
    {
        private JArray jarray;

        private JsonArray(string json)
        {
            jarray = JArray.Parse(json);
        }

        protected JsonArray()
        {
            jarray = new JArray();
        }

        public static JsonArray Parse(string json)
        {
            return new JsonArray(json);
        }

        public IEnumerable<T> ConvertTo<T>()
        {
            using IEnumerator<JsonObject> enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                JsonObject current = enumerator.Current;
                yield return current.Deserialize<T>();
            }
        }

        public IEnumerator<JsonObject> GetEnumerator()
        {
            foreach (JToken item in jarray)
            {
                yield return JsonObject.Parse(item.ToString());
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return jarray.ToString();
        }
    }
}
