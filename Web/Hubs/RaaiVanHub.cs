using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;
using System.Web;

namespace RaaiVan.Web
{
    /// <summary>
    /// The Hub class is used to define methods the clients can call on the server
    /// </summary>
    [HubName("raaivanHub")]
    public class RaaiVanHub : Hub
    {
        public enum RealTimeAction
        {
            //Chat
            NewMessage,
            IsOnline,
            WentOffline,
            ChatWindow,
            NewChatMember,
            IsTyping,
            IsNotTyping,
            //end of Chat

            //Social
            NewPost,
            ModifyPost,
            RemovePost,
            NewComment,
            ModifyComment,
            RemoveComment
            //end of Social
        }

        private class UserGroup
        {
            public string GroupID;
            public bool Private;
        }

        private class Registration
        {
            public string ConnectionID;
            public Guid ApplicationID;
            public Guid UserID;
            public List<string> Events;

            public Registration(string connectionId, Guid applicationId, Guid userId, List<string> events) {
                ConnectionID = connectionId;
                ApplicationID = applicationId;
                UserID = userId;
                Events = events;
            }
        }
        
        private static Dictionary<Guid, List<string>> UserConnectionsDic = new Dictionary<Guid, List<string>>(); //UserID, ConnectionIDs
        private static Dictionary<string, Registration> ConnectedUsers = new Dictionary<string, Registration>(); //ConnectionID, Registration
        private static Dictionary<string, Dictionary<string, SortedSet<string>>> Feeds = 
            new Dictionary<string, Dictionary<string, SortedSet<string>>>(); //Dic<FeedID, Dic<Event,ConntectionIDs>>

        private static Dictionary<string, List<Guid>> ChatGroups = new Dictionary<string, List<Guid>>(); //GroupID, Members UserID List
        private static Dictionary<Guid, List<UserGroup>> UserGroups = new Dictionary<Guid, List<UserGroup>>(); //UserID, GroupsIDs List

        private static Dictionary<Guid, User> UsersDic = new Dictionary<Guid, User>();
        private static Dictionary<Guid, List<Guid>> FriendsDic = new Dictionary<Guid, List<Guid>>();

        public Guid get_current_user_id()
        {
            try { return Guid.Parse(Context.User.Identity.Name); }
            catch { return Guid.Empty; }
        }

        public Guid get_tenant_id() {
            try
            {
                return PublicMethods.get_current_tenant(Context.Request.GetHttpContext().GetOwinContext().Request,
                    RaaiVanSettings.Tenants).Id;
            }
            catch (Exception ex) { return Guid.Empty; }
        }

        public void RegisterListener(string uniqueId, string jsonItems)
        {
            Guid tenantId = get_tenant_id();

            if (!ConnectedUsers.ContainsKey(Context.ConnectionId) ||
                string.IsNullOrEmpty(jsonItems) || !Modules.RaaiVanConfig.Modules.RealTime(tenantId)) return;

            Dictionary<string, object> items = PublicMethods.fromJSON(jsonItems);

            List<string> allNames = new List<string>();

            foreach (string n in items.Keys)
            {
                if (string.IsNullOrEmpty(n)) continue;

                string feedId = !((Dictionary<string, object>)items[n]).ContainsKey("FeedID") ? string.Empty :
                    ((Dictionary<string, object>)items[n])["FeedID"].ToString().ToLower();

                allNames.Add(n);

                if (string.IsNullOrEmpty(feedId))
                {
                    if (!ConnectedUsers[Context.ConnectionId].Events.Any(u => u == n))
                        ConnectedUsers[Context.ConnectionId].Events.Add(n);
                }
                else {
                    if (!Feeds.ContainsKey(feedId)) Feeds[feedId] = new Dictionary<string, SortedSet<string>>();
                    if (!Feeds[feedId].ContainsKey(n)) Feeds[feedId][n] = new SortedSet<string>();
                    if (!Feeds[feedId][n].Any(u => u == Context.ConnectionId)) Feeds[feedId][n].Add(Context.ConnectionId);
                }
            }
            
            Clients.Client(Context.ConnectionId).Registered(uniqueId, string.Join(",", allNames));
        }

        public void RemoveListener(string uniqueId, string jsonItems)
        {
            if (!ConnectedUsers.ContainsKey(Context.ConnectionId) || string.IsNullOrEmpty(jsonItems)) return;

            Dictionary<string, object> items = PublicMethods.fromJSON(jsonItems);

            List<string> allNames = new List<string>();

            foreach (string n in items.Keys)
            {
                if (string.IsNullOrEmpty(n)) continue;

                string feedId = !((Dictionary<string, object>)items[n]).ContainsKey("FeedID") ? string.Empty :
                    ((Dictionary<string, object>)items[n])["FeedID"].ToString().ToLower();

                allNames.Add(n);

                if (string.IsNullOrEmpty(feedId))
                {
                    if (ConnectedUsers[Context.ConnectionId].Events.Any(u => u == n))
                        ConnectedUsers[Context.ConnectionId].Events.Remove(n);
                }
                else {
                    if(Feeds.ContainsKey(feedId) && Feeds[feedId].ContainsKey(n) && Feeds[feedId][n].Any(u => u == Context.ConnectionId))
                        Feeds[feedId][n].Remove(Context.ConnectionId);
                    if (Feeds.ContainsKey(feedId) && Feeds[feedId].ContainsKey(n) && Feeds[feedId][n].Count == 0)
                        Feeds.Remove(feedId);
                }
            }

            Clients.Client(Context.ConnectionId).Unregistered(uniqueId, string.Join(",", allNames));
        }

        public static void SendData(Guid applicationId, List<Guid> userIds, RealTimeAction actionName, string jsonString)
        {
            if (!RaaiVanSettings.RealTime(applicationId) || userIds == null) return;
            string name = actionName.ToString().ToLower();

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<RaaiVanHub>();

            foreach (Guid uId in userIds.Distinct())
            {
                if (!UserConnectionsDic.ContainsKey(uId)) continue;

                foreach (string connId in UserConnectionsDic[uId])
                {
                    if (ConnectedUsers.ContainsKey(connId) && ConnectedUsers[connId].Events.Any(u => u == name))
                        context.Clients.Client(connId).GetData(name, jsonString);
                }
            }
        }
        
        public static void SendData(Guid applicationId, Guid userId, RealTimeAction actionName, string jsonString)
        {
            SendData(applicationId, new List<Guid> { userId }, actionName, jsonString);
        }

        public static void SendData(Guid applicationId, string feedId, RealTimeAction actionName, string jsonString)
        {
            if (!RaaiVanSettings.RealTime(applicationId) || string.IsNullOrEmpty(feedId)) return;

            feedId = feedId.ToLower();
            string name = actionName.ToString().ToLower();

            if (!Feeds.ContainsKey(feedId) || !Feeds[feedId].ContainsKey(name)) return;

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<RaaiVanHub>();

            foreach (string connId in Feeds[feedId][name])
                if (ConnectedUsers.ContainsKey(connId)) context.Clients.Client(connId).GetData(name, jsonString);
        }

        public static void SendData(Guid applicationId, List<string> feedIds, RealTimeAction actionName, string jsonString)
        {
            foreach (string f in feedIds) SendData(applicationId, f, actionName, jsonString);
        }

        public void GetData(string name, string data)
        {
            if (string.IsNullOrEmpty(name)) return;

            Guid tenantId = Guid.Empty;

            try
            {
                tenantId = PublicMethods.get_current_tenant(Context.Request.GetHttpContext().GetOwinContext().Request,
                    RaaiVanSettings.Tenants).Id;
            }
            catch { return; }

            Guid currentUserId = get_current_user_id();

            name = name.ToLower();
            Dictionary<string, string> dataDic = PublicMethods.json2dictionary(data);

            switch (name)
            {
                case "istyping":
                    if (dataDic.ContainsKey("GroupID")) show_is_typing(tenantId, currentUserId, dataDic);
                    break;
                case "isnottyping":
                    if (dataDic.ContainsKey("GroupID")) hide_is_typing(tenantId, currentUserId, dataDic);
                    break;
                case "newchat":
                    new_chat(tenantId, currentUserId, dataDic);
                    break;
                case "newchatmember":
                    if (dataDic.ContainsKey("GroupID")) new_chat_member(tenantId, currentUserId, dataDic);
                    break;
            }
        }

        public static bool is_online(Guid userId)
        {
            return UserConnectionsDic.ContainsKey(userId);
        }

        public static void new_chat(Guid applicationId, Guid currentUserId, Dictionary<string, string> data)
        {
            Guid userId = Guid.Empty;
            if (!Guid.TryParse(data["UserID"], out userId)) return;

            if (!UserGroups.ContainsKey(currentUserId)) UserGroups[currentUserId] = new List<UserGroup>();

            UserGroup group = UserGroups[currentUserId].Where(
                u => u.Private == true && ChatGroups.ContainsKey(u.GroupID) &&
                    ChatGroups[u.GroupID].Any(v => v == userId) && (
                        (userId == currentUserId && ChatGroups[u.GroupID].Count == 1) ||
                        (userId != currentUserId && ChatGroups[u.GroupID].Count == 2)
                    )).FirstOrDefault();

            string groupId = string.Empty;

            if (group != null)
                groupId = group.GroupID;
            else
            {
                groupId = Guid.NewGuid().ToString();
                if (!UserGroups.ContainsKey(userId)) UserGroups[userId] = new List<UserGroup>();

                UserGroups[currentUserId].Add(new UserGroup() { Private = true, GroupID = groupId });
                ChatGroups[groupId] = new List<Guid>() { currentUserId };
                if (userId != currentUserId)
                {
                    UserGroups[userId].Add(new UserGroup() { Private = true, GroupID = groupId });
                    ChatGroups[groupId].Add(userId);
                }
            }

            prepare_user(applicationId, userId);

            RaaiVanHub.SendData(applicationId, currentUserId, RealTimeAction.ChatWindow,
                "{\"GroupID\":\"" + groupId + "\",\"User\":" + _get_user_json(applicationId, userId) + "}");
        }

        public static void new_chat_member(Guid applicationId, Guid currentUserId, Dictionary<string, string> data)
        {
            Guid userId = Guid.Empty;
            if (!data.ContainsKey("GroupID") || !Guid.TryParse(data["UserID"], out userId)) return;

            string groupId = data["GroupID"];

            if (!ChatGroups.ContainsKey(groupId)) return;

            List<Guid> audience = new List<Guid>() { currentUserId };

            var result = "";

            if (ChatGroups[groupId].Contains(userId))
                result = "{\"ErrorText\":\"" + "Sorry, " + userId + " is already in group chat." + "\"}";
            else
            {
                ChatGroups[groupId].Add(userId);
                audience = ChatGroups[groupId].Where(u => u != userId && u != currentUserId).Select(v => v).ToList();

                foreach (Guid uId in audience)
                {
                    UserGroup ug = !UserGroups.ContainsKey(uId) ? null :
                        UserGroups[uId].Where(u => u.GroupID == groupId).FirstOrDefault();
                    if (ug != null) ug.Private = false;
                }

                UserGroups[userId].Add(new UserGroup() { Private = false, GroupID = groupId });

                prepare_user(applicationId, userId);
                result = _get_user_json(applicationId, userId);
            }

            RaaiVanHub.SendData(applicationId, audience, RealTimeAction.NewChatMember,
                "{\"GroupID\":\"" + groupId + "\",\"Data\":" + result + "}");
        }

        private static void show_is_typing(Guid applicationId, Guid currentUserId, Dictionary<string, string> data)
        {
            if (!data.ContainsKey("GroupID") || currentUserId == Guid.Empty) return;

            string groupId = data["GroupID"];

            if (!ChatGroups.ContainsKey(groupId)) return;
            
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<RaaiVanHub>();

            prepare_user(applicationId, currentUserId);

            RaaiVanHub.SendData(applicationId, ChatGroups[groupId].Where(u => u != currentUserId).ToList(), RealTimeAction.IsTyping,
                "{\"GroupID\":\"" + groupId + "\",\"User\":" + _get_user_json(applicationId, currentUserId) + "}");
        }

        private static void hide_is_typing(Guid applicationId, Guid currentUserId, Dictionary<string, string> data)
        {
            if (!data.ContainsKey("GroupID") || currentUserId == Guid.Empty) return;

            string groupId = data["GroupID"];

            if (!ChatGroups.ContainsKey(groupId)) return;

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<RaaiVanHub>();

            prepare_user(applicationId, currentUserId);

            RaaiVanHub.SendData(applicationId, ChatGroups[groupId].Where(u => u != currentUserId).ToList(), RealTimeAction.IsNotTyping,
                "{\"GroupID\":\"" + groupId + "\",\"User\":" + _get_user_json(applicationId, currentUserId) + "}");
        }

        private static List<Guid> get_friends(Guid applicationId, Guid userId)
        {
            if (!FriendsDic.ContainsKey(userId))
                FriendsDic[userId] = UsersController.get_friend_ids(applicationId, userId, true, true, true);

            return FriendsDic[userId];
        }

        private static void prepare_users(Guid applicationId, List<Guid> userIds)
        {
            List<Guid> notExisting = new List<Guid>();

            foreach (Guid uId in userIds)
                if (!UsersDic.ContainsKey(uId)) notExisting.Add(uId);

            if (notExisting.Count > 0)
                foreach (User u in UsersController.get_users(applicationId, notExisting)) UsersDic[u.UserID.Value] = u;
        }

        private static void prepare_user(Guid applicationId, Guid userId)
        {
            prepare_users(applicationId, new List<Guid>() { userId });
        }

        private static string _get_user_json(Guid applicationId, User user)
        {
            return "{\"UserID\":\"" + user.UserID.Value.ToString() + "\"" +
                ",\"UserName\":\"" + Base64.encode(user.UserName) + "\"" +
                ",\"FirstName\":\"" + Base64.encode(user.FirstName) + "\"" +
                ",\"LastName\":\"" + Base64.encode(user.LastName) + "\"" +
                ",\"ProfileImageURL\":\"" + 
                    DocumentUtilities.get_personal_image_address(applicationId, user.UserID.Value) + "\"" +
                ",\"IsOnline\":" + UserConnectionsDic.ContainsKey(user.UserID.Value).ToString().ToLower() +
                "}";
        }

        private static string _get_user_json(Guid applicationId, Guid userId)
        {
            return _get_user_json(applicationId, UsersDic[userId]);
        }

        public override Task OnConnected()
        {
            Guid tenantId = Guid.Empty;

            try
            {
                tenantId = PublicMethods.get_current_tenant(Context.Request.GetHttpContext().GetOwinContext().Request,
                    RaaiVanSettings.Tenants).Id;
            }
            catch (Exception ex) { return base.OnConnected(); }

            if (!RaaiVanSettings.RealTime(tenantId)) return base.OnConnected();

            Guid currentUserId = get_current_user_id();
            if (currentUserId == Guid.Empty) return base.OnConnected();

            if (!UserConnectionsDic.ContainsKey(currentUserId)) UserConnectionsDic[currentUserId] = new List<string>();

            bool added = false;

            if (!UserConnectionsDic[currentUserId].Any(u => u == Context.ConnectionId))
            {
                UserConnectionsDic[currentUserId].Add(Context.ConnectionId);
                added = true;
            }

            ConnectedUsers.Add(Context.ConnectionId, 
                new Registration(Context.ConnectionId, tenantId, currentUserId, new List<string>()));

            //if the user is now online, tell his/her friends
            if (UserConnectionsDic[currentUserId].Count == 1 && added)
            {
                prepare_user(tenantId, currentUserId);
                string userJson = _get_user_json(tenantId, currentUserId);

                RaaiVanHub.SendData(tenantId, get_friends(tenantId, currentUserId), RealTimeAction.IsOnline, userJson);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Guid userId = ConnectedUsers.ContainsKey(Context.ConnectionId) ?
                ConnectedUsers[Context.ConnectionId].UserID : Guid.Empty;

            if (userId == Guid.Empty) return base.OnDisconnected(stopCalled);
            
            Guid tenantId = Guid.Empty;

            try
            {
                tenantId = ConnectedUsers.ContainsKey(Context.ConnectionId) ?
                    ConnectedUsers[Context.ConnectionId].ApplicationID : Guid.Empty;
            }
            catch (Exception ex) { }

            ConnectedUsers.Remove(Context.ConnectionId);

            if (tenantId == Guid.Empty) return base.OnDisconnected(stopCalled);

            if (UserConnectionsDic.ContainsKey(userId) && UserConnectionsDic[userId].Any(u => u == Context.ConnectionId))
            {
                UserConnectionsDic[userId].Remove(Context.ConnectionId);

                //if the user is now offline, tell his/her friends
                if (UserConnectionsDic[userId].Count == 0)
                {
                    UserConnectionsDic.Remove(userId);

                    if (UsersDic.ContainsKey(userId)) UsersDic.Remove(userId);
                    if (FriendsDic.ContainsKey(userId)) FriendsDic.Remove(userId);

                    RaaiVanHub.SendData(tenantId, get_friends(tenantId, userId), RealTimeAction.WentOffline,
                        "{\"UserID\":\"" + userId.ToString() + "\"}");
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }
}