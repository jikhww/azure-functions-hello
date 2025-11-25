using Microsoft.Extensions.Configuration;

namespace Kingdee.CDP.WebApi.SDK
{
    public class ApiSettingsHelper
    {
        private static readonly object objLock = new object();

        private static ApiSettingsHelper instance = null;

        private IConfiguration Config { get; }

        public static int SessionExpire
        {
            get
            {
                string config = GetConfig("SessionExpire");
                if (!string.IsNullOrWhiteSpace(config))
                {
                    return Convert.ToInt32(config);
                }

                return 10800;
            }
        }

        public static int ClientSessionExpire
        {
            get
            {
                string config = GetConfig("ClientSessionExpire");
                if (!string.IsNullOrWhiteSpace(config))
                {
                    return Convert.ToInt32(config);
                }

                return 86400;
            }
        }

        private ApiSettingsHelper()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true);
            Config = configurationBuilder.Build();
        }

        private static ApiSettingsHelper GetInstance()
        {
            if (instance == null)
            {
                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new ApiSettingsHelper();
                    }
                }
            }

            return instance;
        }

        public static IConfigurationSection GetSection(string name)
        {
            return GetInstance().Config.GetSection(name);
        }

        public static string GetConfig(string name)
        {
            return GetSection(name).Value;
        }
    }
}
