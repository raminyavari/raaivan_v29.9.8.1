using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RaaiVan.Modules.GlobalUtilities
{
    public class RedisSessionStateProviderSettings
    {
        public static string getConnectionString()
        {
            return string.IsNullOrEmpty(RaaiVanSettings.Redis.Hosts) ? string.Empty : RaaiVanSettings.Redis.Hosts +
                (string.IsNullOrEmpty(RaaiVanSettings.Redis.Password) ? string.Empty : ",password=" + RaaiVanSettings.Redis.Password);
        }
    }

    public class RedisAPI
    {
        private static ConnectionMultiplexer Redis = null;

        private static bool init()
        {
            /*
            string hosts = RaaiVanSettings.Redis.Hosts.Trim();
            ConfigurationOptions options = ConfigurationOptions.Parse(hosts);
            if (!string.IsNullOrEmpty(RaaiVanSettings.Redis.Password)) options.Password = RaaiVanSettings.Redis.Password;
            if (!string.IsNullOrEmpty(hosts) && Redis == null) Redis = ConnectionMultiplexer.Connect(hosts);
            */

            string conn = RedisSessionStateProviderSettings.getConnectionString();
            if (!string.IsNullOrEmpty(conn) && Redis == null) Redis = ConnectionMultiplexer.Connect(conn);
            return Redis != null;
        }

        private static IDatabase get_database()
        {
            return !init() ? null : Redis.GetDatabase();
        }

        public static bool Enabled
        {
            get { return init(); }
        }

        public static bool set_value<T>(string key, T value)
        {
            try
            {
                IDatabase db = get_database();

                if (db == null || value == null || !value.GetType().IsSerializable || string.IsNullOrEmpty(key)) return false;

                JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new RVJsonContractResolver() };
                string str = JsonConvert.SerializeObject(value, settings);

                return !string.IsNullOrEmpty(str) && db.StringSet(key, str);
            }
            catch { return false; }
        }

        public static void set_value(string key, string value)
        {
            set_value<string>(key, value);
        }

        public static T get_value<T>(string key)
        {
            try
            {
                IDatabase db = get_database();

                if (db != null && !string.IsNullOrEmpty(key))
                {
                    RedisValue value = db.StringGet(key);

                    if (!value.IsNullOrEmpty)
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new RVJsonContractResolver() };
                        return JsonConvert.DeserializeObject<T>(value, settings);
                    }
                }
            }
            catch { }

            return default(T);
        }

        public static string get_value(string key)
        {
            return get_value<string>(key);
        }

        public static bool remove_key(string key)
        {
            try
            {
                IDatabase db = get_database();
                return db != null && !string.IsNullOrEmpty(key) && db.KeyDelete(new RedisKey(key));
            }
            catch { return false; }
        }
    }
}