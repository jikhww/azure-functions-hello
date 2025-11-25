using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
namespace Kingdee.CDP.WebApi.SDK
{
    public class JsonObject : IEnumerable<KeyValuePair<string, string>>, IEnumerable
    {
        private JObject jobject;

        private JsonObject(string json)
        {
            jobject = JObject.Parse(json);
        }

        protected JsonObject()
        {
            jobject = new JObject();
        }

        public static JsonObject Parse(string json)
        {
            return new JsonObject(json);
        }

        public static T Deserialize<T>(string json)
        {
            return (T)Deserialize(json, typeof(T));
        }

        private static object Deserialize(string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }

                if (type.Equals(typeof(string)))
                {
                    return json;
                }

                return null;
            }

            if (type == typeof(string))
            {
                return json;
            }

            if (type.IsEnum)
            {
                return Enum.Parse(type, json, ignoreCase: true);
            }

            if (type == typeof(int))
            {
                return int.Parse(json);
            }

            if (type == typeof(DateTime))
            {
                return DateTime.Parse(json);
            }

            if (type == typeof(decimal))
            {
                return decimal.Parse(json);
            }

            if (type == typeof(bool))
            {
                return bool.Parse(json);
            }

            return JsonConvert.DeserializeObject(json, type);
        }

        public void SetValue(string prop, object v)
        {
            if (v != null && v.GetType().IsSimpleType())
            {
                AddOrUpdate(prop, new JValue(v));
            }
            else
            {
                AddOrUpdate(prop, JObject.FromObject(v));
            }
        }

        private void AddOrUpdate(string prop, JToken v)
        {
            if (jobject.Property(prop) == null)
            {
                jobject.Add(prop, v);
            }
            else
            {
                jobject[prop] = v;
            }
        }

        public T GetValue<T>(string prop)
        {
            if (jobject.TryGetValue(prop, out JToken value))
            {
                return value.Value<T>();
            }

            throw new Exception("对象不包括属性" + prop);
        }

        public IEnumerable<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (KeyValuePair<string, JToken> item in jobject)
            {
                yield return new KeyValuePair<string, string>(item.Key, item.Value.ToString());
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return jobject.ToString();
        }

        internal T Deserialize<T>()
        {
            return (T)JsonConvert.DeserializeObject(ToString(), typeof(T));
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
