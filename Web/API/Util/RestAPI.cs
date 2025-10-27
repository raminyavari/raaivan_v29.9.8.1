using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web.API
{
    public class RestAPI
    {
        private static Dictionary<string, Guid> _Tickets = new Dictionary<string, Guid>();

        private static string get_redis_key(Guid userId)
        {
            return userId.ToString() + "_rest_api_ticket";
        }

        private static string get_redis_token_key(string ticket)
        {
            return ticket + "_token_list";
        }

        public static void new_ticket(string ticket, Guid userId)
        {
            if (string.IsNullOrEmpty(ticket)) return;
            
            if (RedisAPI.Enabled)
            {
                RedisAPI.set_value<Guid?>(ticket, userId);
                RedisAPI.set_value(get_redis_key(userId), ticket);
            }
            else _Tickets[ticket] = userId;
        }

        public static Guid? get_user_id(string ticket)
        {
            if (string.IsNullOrEmpty(ticket)) return null;

            return RedisAPI.Enabled ? RedisAPI.get_value<Guid?>(ticket) :
                (!_Tickets.ContainsKey(ticket) ? (Guid?)null : _Tickets[ticket]);
        }

        public static string get_ticket(Guid userId)
        {
            return RedisAPI.Enabled ? RedisAPI.get_value(get_redis_key(userId)) :
                _Tickets.Keys.Where(u => _Tickets[u] == userId).FirstOrDefault();
        }

        private static SortedList<string, AccessTokenList> APITokens = new SortedList<string, AccessTokenList>();

        public static AccessTokenList get_token_list(string ticket)
        {
            if (string.IsNullOrEmpty(ticket)) return null;

            if (RedisAPI.Enabled)
            {
                AccessTokenList lst = RedisAPI.get_value<AccessTokenList>(get_redis_token_key(ticket));
                return lst == null ? new AccessTokenList() : lst;
            }
            else
            {
                if (!APITokens.ContainsKey(ticket))
                    APITokens.Add(ticket, new AccessTokenList());
                return APITokens[ticket];
            }
        }

        public static string new_token(string ticket) {
            if (string.IsNullOrEmpty(ticket)) return string.Empty;

            AccessTokenList lst = get_token_list(ticket);
            string token = AccessTokenList.new_token(lst);

            if (RedisAPI.Enabled) RedisAPI.set_value<AccessTokenList>(get_redis_token_key(ticket), lst);

            return token;
        }
    }
}