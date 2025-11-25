namespace Kingdee.CDP.WebApi.SDK
{
    public class AsyncResult<T>
    {
        public bool Successful { get; internal set; }

        public T ReturnValue { get; set; }

        public Exception exception { get; internal set; }

        internal void ThrowEx()
        {
            if (exception != null)
            {
                throw exception;
            }
        }

        internal AsyncResult()
        {
        }

        internal static AsyncResult<T> CreateUnsuccess(Exception ex)
        {
            return new AsyncResult<T>
            {
                Successful = false,
                ReturnValue = default(T),
                exception = ex
            };
        }

        internal static AsyncResult<T> CreateSuccess(T result)
        {
            return new AsyncResult<T>
            {
                Successful = true,
                ReturnValue = result,
                exception = null
            };
        }

        internal AsyncResult<List<To>> ToList<To>()
        {
            AsyncResult<List<To>> asyncResult = new AsyncResult<List<To>>
            {
                Successful = Successful,
                exception = exception
            };
            if (typeof(T) == typeof(string))
            {
                asyncResult.ReturnValue = JsonArray.Parse(ReturnValue.ToString()).ConvertTo<To>().ToList();
            }
            else
            {
                asyncResult.ReturnValue = (List<To>)(object)ReturnValue;
            }

            return asyncResult;
        }

        internal AsyncResult<To> Cast<To>()
        {
            AsyncResult<To> asyncResult = new AsyncResult<To>
            {
                Successful = Successful,
                exception = exception
            };
            if (typeof(T) == typeof(string))
            {
                string text = ((ReturnValue == null) ? "" : ReturnValue.ToString());
                if (string.IsNullOrEmpty(text))
                {
                    asyncResult.ReturnValue = default(To);
                }
                else
                {
                    asyncResult.ReturnValue = JsonObject.Deserialize<To>(text);
                }
            }
            else
            {
                asyncResult.ReturnValue = (To)(object)ReturnValue;
            }

            return asyncResult;
        }
    }
}
