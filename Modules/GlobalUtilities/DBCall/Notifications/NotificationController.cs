using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Threading;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.QA;
using RaaiVan.Modules.Sharing;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Modules.NotificationCenter
{
    public class NotificationController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[NTFN_" + name + "]"; //'[dbo].' is database owner and 'NTFN_' is module qualifier
        }

        private static List<Guid> _get_audience_user_ids(ref Notification notification, UserStatus status)
        {
            if (notification.Audience.ContainsKey(status)) return notification.Audience[status];
            else return null;
        }

        private static bool _send_notification(Guid applicationId, List<KeyValuePair<Guid, UserStatus>> users, Notification notification)
        {
            if (users == null) users = new List<KeyValuePair<Guid, UserStatus>>();

            //users param
            List<Guid> userIds = new List<Guid>();

            DBCompositeType<GuidStringTableType> usersParam = new DBCompositeType<GuidStringTableType>();

            users.ForEach(usr => {
                if (!userIds.Exists(u => u == usr.Key))
                {
                    userIds.Add(usr.Key);
                    usersParam.add(new GuidStringTableType(usr.Key, usr.Value.ToString()));
                }
            });
            //end of users param

            string subjectName = string.IsNullOrEmpty(notification.SubjectName) ? null : notification.SubjectName;
            DateTime sendDate = notification.SendDate.HasValue ? notification.SendDate.Value : DateTime.Now;
            string description = string.IsNullOrEmpty(notification.Description) ? null : notification.Description;
            string info = string.IsNullOrEmpty(notification.Info) ? null : notification.Info;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SendNotification"),
                applicationId, usersParam, notification.SubjectID, notification.RefItemID, notification.SubjectType.ToString(),
                subjectName, notification.Action.ToString(), notification.Sender.UserID, sendDate, description, info);
        }

        private static void _send_notification(Guid applicationId, Notification info)
        {
            if (!RaaiVanConfig.Modules.Notifications(applicationId)) return;

            if (!info.Action.HasValue || info.Action.Value == ActionType.None ||
                !info.SubjectType.HasValue || info.SubjectType.Value == SubjectType.None) return;

            List<KeyValuePair<Guid, UserStatus>> users = new List<KeyValuePair<Guid, UserStatus>>();
            if (info.UserID.HasValue && info.UserID != info.Sender.UserID)
                users.Add(new KeyValuePair<Guid, UserStatus>(info.UserID.Value, UserStatus.Owner));

            List<Guid> userIds = new List<Guid>();

            List<Guid> mentionedUserIds = info.Action == ActionType.Post || info.Action == ActionType.Share || 
                info.Action == ActionType.Comment ?
                Expressions.get_tagged_items(info.Description, "User").Where(u => u.ID.HasValue && u.ID != info.UserID)
                .Select(u => u.ID.Value).ToList() : new List<Guid>();

            info.Description = PublicMethods.markup2plaintext(applicationId,
                Expressions.replace(info.Description, Expressions.Patterns.HTMLTag, " "));

            switch (info.Action.Value)
            {
                case ActionType.Like:
                    switch (info.SubjectType.Value)
                    {
                        case SubjectType.Node:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                                userIds = CNController.get_node_creators(applicationId, info.RefItemID.Value).Select(
                                    u => u.User.UserID.Value).ToList();
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Member)) == null)
                                userIds = CNController.get_member_user_ids(applicationId, 
                                    info.RefItemID.Value, NodeMemberStatuses.Accepted);
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Member));

                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Expert)) == null)
                                userIds = CNController.get_experts(applicationId, info.RefItemID.Value).Select(
                                    u => u.User.UserID.Value).ToList();
                            foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Expert));

                            Node node = CNController.get_node(applicationId, info.RefItemID.Value, true);
                            if (node != null)
                            {
                                info.SubjectName = node.Name;
                                info.Description = node.Description;
                                info.Info = "{\"NodeType\":\"" + Base64.encode(node.NodeType) + "\"}";
                            }
                            break;
                        case SubjectType.Question:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>() { };
                                Guid? id = QAController.get_question_asker_id(applicationId, info.RefItemID.Value);
                                if (id.HasValue) userIds.Add(id.Value);
                            }
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            Question question = QAController.get_question(applicationId, info.RefItemID.Value, null);
                            if (question != null)
                            {
                                info.SubjectName = question.Title;
                                info.Description = question.Description;
                            }
                            break;
                        case SubjectType.Post:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>();
                                Guid? id = SharingController.get_post_sender_id(applicationId, info.RefItemID.Value);
                                if(id.HasValue) userIds.Add(id.Value);
                            }
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            Post post = SharingController.get_post(applicationId, info.RefItemID.Value, null);
                            info.Description = string.IsNullOrEmpty(post.Description) ? 
                                post.OriginalDescription : post.Description;
                            break;
                        case SubjectType.Comment:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>();
                                Guid? id = SharingController.get_comment_sender_id(applicationId, info.SubjectID.Value);
                                if (id.HasValue) userIds.Add(id.Value);
                            }
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            Sharing.Comment comment = SharingController.get_comment(applicationId, info.SubjectID.Value, null);
                            info.RefItemID = comment.PostID;
                            info.Description = comment.Description;
                            break;
                    }
                    break;
                case ActionType.Dislike:
                    switch (info.SubjectType.Value)
                    {
                        case SubjectType.Post:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>();
                                Guid? id = SharingController.get_post_sender_id(applicationId, info.RefItemID.Value);
                                if (id.HasValue) userIds.Add(id.Value);
                            }
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            Post post = SharingController.get_post(applicationId, info.RefItemID.Value, null);
                            info.Description = string.IsNullOrEmpty(post.Description) ? post.OriginalDescription : post.Description;
                            break;
                        case SubjectType.Comment:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>();
                                Guid? id = SharingController.get_comment_sender_id(applicationId, info.SubjectID.Value);
                                if (id.HasValue) userIds.Add(id.Value);
                            }
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            Sharing.Comment comment = SharingController.get_comment(applicationId, info.SubjectID.Value, null);
                            info.RefItemID = comment.PostID;
                            info.Description = comment.Description;
                            break;
                    }
                    break;
                case ActionType.Question:
                    switch (info.SubjectType.Value)
                    {
                        case SubjectType.Question:
                            if(info.ReceiverUserIDs != null && info.ReceiverUserIDs.Count > 0)
                            {
                                userIds = info.ReceiverUserIDs;
                                foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Contributor));
                            }
                            break;
                    }
                    break;
                case ActionType.Answer:
                    switch (info.SubjectType.Value)
                    {
                        case SubjectType.Answer:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>();
                                Guid? id = QAController.get_question_asker_id(applicationId, info.RefItemID.Value);
                                if (id.HasValue) userIds.Add(id.Value);
                            }
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Fan)) == null)
                                userIds = GlobalController.get_fan_ids(applicationId, info.RefItemID.Value).ToList();
                            foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Fan));

                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Contributor)) == null)
                                userIds = QAController.get_answer_sender_ids(applicationId, info.RefItemID.Value).ToList();
                            foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Contributor));

                            info.SubjectName = QAController.get_question(applicationId, info.RefItemID.Value, null).Title;
                            break;
                    }
                    break;
                case ActionType.Post:
                case ActionType.Share:
                    switch (info.SubjectType.Value)
                    {
                        case SubjectType.Post:
                            foreach (Guid _usr in mentionedUserIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Mentioned));

                            Node node = null;

                            bool isNode = info.RefItemID.HasValue && 
                                CNController.is_node(applicationId, info.RefItemID.Value);

                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Director)) != null)
                                foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Owner));

                            if (isNode)
                            {
                                if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                                    userIds = CNController.get_node_creators(applicationId, info.RefItemID.Value).Select(
                                        u => u.User.UserID.Value).ToList();
                                foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Owner));

                                if ((userIds = _get_audience_user_ids(ref info, UserStatus.Member)) == null)
                                    userIds = CNController.get_members(applicationId, info.RefItemID.Value,
                                        pending: false, admin: null).Select(u => u.Member.UserID.Value).ToList();
                                foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Member));

                                if ((userIds = _get_audience_user_ids(ref info, UserStatus.Fan)) == null)
                                    userIds = CNController.get_node_fans_user_ids(applicationId, info.RefItemID.Value).ToList();
                                foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Fan));

                                node = CNController.get_node(applicationId, info.RefItemID.Value);
                                if (node != null) info.SubjectName = node.Name;
                            }

                            if (RaaiVanConfig.Modules.SMSEMailNotifier(applicationId))
                            {
                                User user = UsersController.get_user(applicationId, info.Sender.UserID.Value);
                                info.ReplacementDic["SenderProfileImageURL"] =
                                    DocumentUtilities.get_personal_image_address(applicationId, user.UserID.Value, true);
                                info.ReplacementDic["SenderFullName"] = user.FirstName + " " + user.LastName;
                                info.ReplacementDic["SenderPageURL"] = 
                                    PublicConsts.get_complete_url(applicationId, PublicConsts.HomePage) +
                                    "/" + user.UserID.Value.ToString();
                                info.ReplacementDic["PostURL"] = 
                                    PublicConsts.get_complete_url(applicationId, PublicConsts.PostPage) +
                                    "/" + info.SubjectID.Value.ToString();
                                info.ReplacementDic["Description"] = info.Description;

                                if (isNode)
                                {
                                    info.ReplacementDic["NodePageURL"] =
                                        PublicConsts.get_complete_url(applicationId, PublicConsts.NodePage) +
                                        "/" + info.RefItemID.Value.ToString();
                                    if (node != null) info.ReplacementDic["NodeName"] = node.Name;
                                }
                            }

                            break;
                    }
                    break;
                case ActionType.Comment:
                    switch (info.SubjectType.Value)
                    {
                        case SubjectType.Comment:
                            foreach (Guid _usr in mentionedUserIds)
                                users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Mentioned));

                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>();
                                Guid? id = SharingController.get_post_sender_id(applicationId, info.RefItemID.Value);
                                if (id.HasValue) userIds.Add(id.Value);
                            }
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Fan)) == null)
                                userIds = SharingController.get_post_fan_ids(applicationId, info.RefItemID.Value).ToList();
                            foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Fan));

                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Contributor)) == null)
                                userIds = SharingController.get_comment_sender_ids(applicationId, info.RefItemID.Value).ToList();
                            foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Contributor));

                            if (RaaiVanConfig.Modules.SMSEMailNotifier(applicationId))
                            {
                                User user = UsersController.get_user(applicationId, info.Sender.UserID.Value);
                                info.ReplacementDic["SenderProfileImageURL"] =
                                    DocumentUtilities.get_personal_image_address(applicationId, user.UserID.Value, true);
                                info.ReplacementDic["SenderFullName"] = user.FirstName + " " + user.LastName;
                                info.ReplacementDic["SenderPageURL"] = 
                                    PublicConsts.get_complete_url(applicationId, PublicConsts.ProfilePage) + 
                                    "/" + user.UserID.Value.ToString();
                                info.ReplacementDic["PostURL"] = 
                                    PublicConsts.get_complete_url(applicationId, PublicConsts.PostPage) +
                                    "/" + info.RefItemID.Value.ToString();
                                info.ReplacementDic["Description"] = info.Description;
                            }

                            break;
                        case SubjectType.Question:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>();
                                Guid? id = QAController.get_question_asker_id(applicationId, info.RefItemID.Value);
                                if (id.HasValue) userIds.Add(id.Value);
                            }
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));
                            
                            break;
                        case SubjectType.Answer:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>();
                                Guid? commentId = QAController.get_comment_owner_id(applicationId, info.SubjectID.Value);
                                if(commentId.HasValue) userIds.Add(commentId.Value);
                            }
                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            break;
                    }
                    break;
                case ActionType.Modify:
                    switch (info.SubjectType.Value)
                    {
                        case SubjectType.Wiki:
                            Node node = CNController.get_node(applicationId, info.RefItemID.Value, false);

                            if (node != null && node.NodeID.HasValue)
                            {
                                if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                                    userIds = CNController.get_node_creators(applicationId, info.RefItemID.Value).Select(
                                        u => u.User.UserID.Value).ToList();
                                foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Owner));

                                if ((userIds = _get_audience_user_ids(ref info, UserStatus.Fan)) == null) userIds = 
                                        CNController.get_node_fans_user_ids(applicationId, info.RefItemID.Value).ToList();
                                foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Fan));

                                if ((userIds = _get_audience_user_ids(ref info, UserStatus.Expert)) == null)
                                    userIds = CNController.get_experts(applicationId, info.RefItemID.Value).Select(
                                        u => u.User.UserID.Value).ToList();
                                foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Expert));

                                if ((userIds = _get_audience_user_ids(ref info, UserStatus.Member)) == null)
                                    userIds = CNController.get_member_user_ids(applicationId, info.RefItemID.Value);
                                foreach (Guid _usr in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Member));

                                if (!users.Exists(u => u.Value == UserStatus.Owner))
                                    users.Add(new KeyValuePair<Guid, UserStatus>(node.Creator.UserID.Value, UserStatus.Owner));

                                info.SubjectName = node.Name;
                                info.Info = "{\"NodeType\":\"" + Base64.encode(node.NodeType) + "\"}";

                                if (RaaiVanConfig.Modules.SMSEMailNotifier(applicationId))
                                {
                                    User user = UsersController.get_user(applicationId, info.Sender.UserID.Value);
                                    info.ReplacementDic["SenderProfileImageURL"] =
                                        DocumentUtilities.get_personal_image_address(applicationId, user.UserID.Value, true);
                                    info.ReplacementDic["SenderFullName"] = user.FirstName + " " + user.LastName;
                                    info.ReplacementDic["SenderPageURL"] = 
                                        PublicConsts.get_complete_url(applicationId, PublicConsts.ProfilePage) +
                                        "/" + user.UserID.Value.ToString();
                                    info.ReplacementDic["NodeName"] = node.Name;
                                    info.ReplacementDic["NodeType"] = node.NodeType;
                                    info.ReplacementDic["NodePageURL"] = 
                                        PublicConsts.get_complete_url(applicationId, PublicConsts.NodePage) +
                                        "/" + info.RefItemID.Value.ToString();
                                    info.ReplacementDic["Description"] = info.Description;
                                }
                            }
                            break;
                    }
                    break;
                case ActionType.FriendRequest:
                    switch (info.SubjectType.Value)
                    {
                        case SubjectType.User:
                            users.Clear();
                            users.Add(new KeyValuePair<Guid, UserStatus>(info.UserID.Value, UserStatus.Mentioned));

                            info.UserStatus = UserStatus.Mentioned;

                            if (RaaiVanConfig.Modules.SMSEMailNotifier(applicationId))
                            {
                                User user = UsersController.get_user(applicationId, info.RefItemID.Value);
                                info.ReplacementDic["SenderProfileImageURL"] =
                                    DocumentUtilities.get_personal_image_address(applicationId, user.UserID.Value, true);
                                info.ReplacementDic["SenderFullName"] = user.FirstName + " " + user.LastName;
                                info.ReplacementDic["SenderPageURL"] = 
                                    PublicConsts.get_complete_url(applicationId, PublicConsts.ProfilePage) +
                                    "/" + user.UserID.Value.ToString();
                            }

                            break;
                    }
                    break;
                case ActionType.AcceptFriendRequest:
                    switch (info.SubjectType.Value)
                    {
                        case SubjectType.User:
                            users.Clear();
                            users.Add(new KeyValuePair<Guid, UserStatus>(info.UserID.Value, UserStatus.Mentioned));

                            info.UserStatus = UserStatus.Mentioned;

                            if (RaaiVanConfig.Modules.SMSEMailNotifier(applicationId))
                            {
                                User user = UsersController.get_user(applicationId, info.RefItemID.Value);
                                info.ReplacementDic["SenderProfileImageURL"] =
                                    DocumentUtilities.get_personal_image_address(applicationId, user.UserID.Value, true);
                                info.ReplacementDic["SenderFullName"] = user.FirstName + " " + user.LastName;
                                info.ReplacementDic["SenderPageURL"] = 
                                    PublicConsts.get_complete_url(applicationId, PublicConsts.ProfilePage) +
                                    "/" + user.UserID.Value.ToString();
                            }
                            
                            break;
                    }
                    break;
                case ActionType.Accept:
                    switch (info.SubjectType)
                    {
                        case SubjectType.Node:
                            {
                                if ((userIds = _get_audience_user_ids(ref info, UserStatus.Member)) == null && 
                                    info.RefItemID.HasValue)
                                {
                                    List<Guid> nIds = CNController.get_related_node_ids(applicationId, info.RefItemID.Value,
                                        null, null, false, true);

                                    List<Guid> creatorIds = 
                                        CNController.get_node_creators(applicationId, info.RefItemID.Value)
                                        .Select(u => u.User.UserID.Value).ToList();

                                    userIds = CNController.get_members(applicationId, nIds,
                                        pending: false, admin: null).Select(u => u.Member.UserID.Value)
                                        .Distinct().Where(x => !creatorIds.Any(a => a == x)).ToList();
                                }

                                foreach (Guid _usr in userIds)
                                    users.Add(new KeyValuePair<Guid, UserStatus>(_usr, UserStatus.Member));
                            }
                            break;
                    }
                    break;
                case ActionType.Publish:
                    switch (info.SubjectType)
                    {
                        case SubjectType.Question:
                            if ((userIds = _get_audience_user_ids(ref info, UserStatus.Owner)) == null)
                            {
                                userIds = new List<Guid>();
                                Guid? id = QAController.get_question_asker_id(applicationId, info.RefItemID.Value);
                                if (id.HasValue) userIds.Add(id.Value);
                            }

                            foreach (Guid _uid in userIds) users.Add(new KeyValuePair<Guid, UserStatus>(_uid, UserStatus.Owner));

                            break;
                    }
                    break;
            }

            users = users.Except(users.Where(u => info.Sender.UserID.HasValue && u.Key == info.Sender.UserID)).ToList();

            _send_notification(applicationId, users, info);

            if (RaaiVanConfig.Modules.SMSEMailNotifier(applicationId))
                NotificationController._send_notification_message(applicationId, users, info);
        }

        public static void send_notification(Guid applicationId, Notification info)
        {
            PublicMethods.set_timeout(() => _send_notification(applicationId, info), 0);
        }

        private static void _transfer_dashboards(object obj)
        {
            Guid applicationId = (Guid)((Pair)obj).First;
            List<Dashboard> dashboards = (List<Dashboard>)((Pair)obj).Second;
        }

        public static void transfer_dashboards(Guid applicationId, List<Dashboard> dashboards)
        {
            if (RaaiVanConfig.Modules.SMSEMailNotifier(applicationId))
                ThreadPool.QueueUserWorkItem(new WaitCallback(_transfer_dashboards), new Pair(applicationId, dashboards));
        }

        public static bool set_notifications_as_seen(Guid applicationId, Guid userId, List<long> notificationIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetNotificationsAsSeen"),
                applicationId, userId, ProviderUtil.list_to_string<long>(notificationIds), ',', DateTime.Now);
        }

        public static bool set_notification_as_seen(Guid applicationId, Guid userId, long notificationId)
        {
            return set_notifications_as_seen(applicationId, userId, new List<long>() { notificationId });
        }

        public static bool set_user_notifications_as_seen(Guid applicationId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetUserNotificationsAsSeen"),
                applicationId, userId, DateTime.Now);
        }

        public static bool remove_notification(Guid applicationId, long notificationId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteNotification"),
                applicationId, notificationId, userId);
        }

        public static void remove_notifications(Guid applicationId, Notification info, List<string> actions)
        {
            if (!RaaiVanConfig.Modules.Notifications(applicationId)) return;

            if (!string.IsNullOrEmpty(info.Action.ToString())) actions.Add(info.Action.ToString());
            actions = actions.Distinct().ToList();

            if (info.SubjectID.HasValue) info.SubjectIDs.Add(info.SubjectID.Value);
            if (info.RefItemID.HasValue) info.RefItemIDs.Add(info.RefItemID.Value);

            PublicMethods.set_timeout(() => {
                DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteNotifications"),
                    applicationId, ProviderUtil.list_to_string<Guid>(info.SubjectIDs),
                    ProviderUtil.list_to_string<Guid>(info.RefItemIDs), info.Sender.UserID, 
                    ProviderUtil.list_to_string<string>(actions), ',');
            }, 0);
        }

        public static void remove_notifications(Guid applicationId, Notification info)
        {
            remove_notifications(applicationId, info, new List<string>());
        }

        public static int get_user_notifications_count(Guid applicationId, Guid userId, bool? seen = false)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetUserNotificationsCount"), applicationId, userId, seen);
        }

        public static List<Notification> get_user_notifications(Guid applicationId, Guid userId, bool? seen = null, 
            long? lastNotSeenId = null, long? lastSeenId = null,DateTime? lastViewDate = null, 
            DateTime? lowerDateLimit = null, DateTime? upperDateLimit = null, int? count = null)
        {
            return NTFNParsers.notifications(DBConnector.read(applicationId, GetFullyQualifiedName("GetUserNotifications"),
                applicationId, userId, seen, lastNotSeenId, lastSeenId, lastViewDate, lowerDateLimit, upperDateLimit, count));
        }

        public static bool set_dashboards_as_seen(Guid applicationId, Guid userId, List<long> dashboardIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetDashboardsAsSeen"),
                applicationId, userId, ProviderUtil.list_to_string<long>(dashboardIds), ',', DateTime.Now);
        }

        public static bool remove_dashboards(Guid applicationId, Guid userId, List<long> dashboardIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteDashboards"),
                applicationId, userId, ProviderUtil.list_to_string<long>(dashboardIds), ',');
        }

        public static List<DashboardCount> get_dashboards_count(Guid applicationId, Guid userId, 
            Guid? nodeTypeId, Guid? nodeId, string nodeAdditionalId, DashboardType type)
        {
            string strType = type == DashboardType.NotSet ? null : type.ToString();

            return DBConnector.get_dashboards_count(applicationId, GetFullyQualifiedName("GetDashboardsCount"),
                applicationId, userId, nodeTypeId, nodeId, nodeAdditionalId, strType);
        }

        private static List<Guid> _get_dashboards(Guid applicationId, ref List<Dashboard> retDashboards, Guid? userId,
            Guid? nodeTypeId, Guid? nodeId, string nodeAdditionalId, DashboardType dashboardType, DashboardSubType subType,
            string subTypeTitle, bool? done, DateTime? dateFrom, DateTime? dateTo, string searchText, bool? getDistinctItems,
            bool? inWorkFlowState, int? lowerBoundary, int? count, ref long totalCount)
        {
            List<Guid> retList = new List<Guid>();

            if (lowerBoundary == 0) lowerBoundary = null;
            if (!count.HasValue || count <= 0) count = 50;

            if (!string.IsNullOrEmpty(nodeAdditionalId)) nodeAdditionalId = nodeAdditionalId.Trim();
            if (string.IsNullOrEmpty(nodeAdditionalId)) nodeAdditionalId = null;

            string strDashboardType = dashboardType == DashboardType.NotSet ? null : dashboardType.ToString();
            string strSubType = subType == DashboardSubType.NotSet ?
                (string.IsNullOrEmpty(subTypeTitle) ? null : subTypeTitle) : subType.ToString();

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetDashboards"),
                applicationId, userId, nodeTypeId, nodeId, nodeAdditionalId, strDashboardType, strSubType, done,
                dateFrom, dateTo, ProviderUtil.get_search_text(searchText), getDistinctItems, inWorkFlowState, lowerBoundary, count);

            if (!getDistinctItems.HasValue || !getDistinctItems.Value)
                retDashboards = DBConnector.parse_dashboards(results.get_table(), ref totalCount);
            else
            {
                retList = DBConnector.parse_guid_list(results.get_table());
                totalCount = results.get_table().GetLong(row: 0, column: "TotalCount", defaultValue: 0).Value;
            }

            return retList;
        }

        public static List<Dashboard> get_dashboards(Guid applicationId, Guid? userId, Guid? nodeTypeId, Guid? nodeId, 
            string nodeAdditionalId, DashboardType dashboardType, DashboardSubType subType, string subTypeTitle, bool? done, 
            DateTime? dateFrom, DateTime? dateTo, string searchText, int? lowerBoundary, int? count, ref long totalCount)
        {
            List<Dashboard> retList = new List<Dashboard>();
            _get_dashboards(applicationId, ref retList, userId, nodeTypeId, nodeId, nodeAdditionalId, dashboardType, 
                subType, subTypeTitle, done, dateFrom, dateTo, searchText, false, null, lowerBoundary, count, ref totalCount);
            return retList;
        }

        public static List<Dashboard> get_dashboards(Guid applicationId, Guid? userId, Guid? nodeTypeId, Guid? nodeId,
            string nodeAdditionalId, DashboardType dashboardType, DashboardSubType subType, string subTypeTitle, bool? done,
            DateTime? dateFrom, DateTime? dateTo, string searchText, int? lowerBoundary, int? count)
        {
            long totalCount = 0;
            return get_dashboards(applicationId, userId, nodeTypeId, nodeId, nodeAdditionalId, dashboardType, subType,
                subTypeTitle, done, dateFrom, dateTo, searchText, lowerBoundary, count, ref totalCount);
        }

        public static List<Guid> get_dashboards(Guid applicationId, Guid? userId, Guid? nodeTypeId, Guid? nodeId,
            DashboardType dashboardType, DashboardSubType subType, string subTypeTitle, string searchText, bool? inWorkFlowState,
            int? lowerBoundary, int? count, ref long totalCount)
        {
            List<Dashboard> retList = new List<Dashboard>();
            return _get_dashboards(applicationId, ref retList, userId, nodeTypeId, nodeId, null, dashboardType, subType,
                subTypeTitle, null, null, null, searchText, true, inWorkFlowState, lowerBoundary, count, ref totalCount);
        }

        public static bool dashboard_exists(Guid applicationId, Guid? userId = null, Guid? nodeId = null, 
            DashboardType? type = null, DashboardSubType? subType = null, bool? seen = null, bool? done = null,
            DateTime? lowerDataLimit = null, DateTime? upperDateLimit = null)
        {
            string strType = null;
            if (type.HasValue && type.Value != DashboardType.NotSet) strType = type.Value.ToString();

            string strSubType = null;
            if (subType.HasValue && subType.Value != DashboardSubType.NotSet) strSubType = subType.Value.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("DashboardExists"),
                applicationId, userId, nodeId, strType, strSubType, seen, done, lowerDataLimit, upperDateLimit);
        }

        public static bool set_message_template(Guid applicationId, MessageTemplate info)
        {
            if (info.AudienceType == AudienceType.NotSet) return false;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetMessageTemplate"),
                applicationId, info.TemplateID, info.OwnerID, info.BodyText, info.AudienceType.ToString(), 
                info.AudienceRefOwnerID, info.AudienceNodeID, info.AudienceNodeAdmin, info.CreatorUserID, DateTime.Now);
        }

        public static bool remove_message_template(Guid applicationId, Guid templateId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteMessageTemplate"),
                 applicationId, templateId, currentUserId, DateTime.Now);
        }

        public static List<MessageTemplate> get_owner_message_templates(Guid applicationId, List<Guid> ownerIds)
        {
            return NTFNParsers.message_templates(DBConnector.read(applicationId, GetFullyQualifiedName("GetOwnerMessageTemplates"),
                applicationId, ProviderUtil.list_to_string<Guid>(ownerIds), ','));
        }

        public static List<MessageTemplate> get_owner_message_templates(Guid applicationId, Guid ownerId)
        {
            List<Guid> lst = new List<Guid>();
            lst.Add(ownerId);
            return get_owner_message_templates(applicationId, lst);
        }

        //Notification Messages

        private static void _send_notification_message(Guid applicationId, List<KeyValuePair<Guid, UserStatus>> users, Notification not)
        {
            try
            {
                if (not == null || !not.SubjectType.HasValue || !not.Action.HasValue) return;

                EmailTemplates.Initialize(applicationId);

                List<NotificationMessage> messageLst = NotificationController._get_notification_messages_info(
                    applicationId, users, not.SubjectType.Value, not.Action.Value);

                List<Guid> receiverIds = messageLst.Where(m => m != null && m.ReceiverUserID.HasValue)
                    .Select(m => m.ReceiverUserID.Value).Distinct().ToList();

                List<EmailAddress> emailList = UsersController.get_users_main_email(applicationId, receiverIds);
                List<PhoneNumber> phoneList = UsersController.get_users_main_phone(applicationId, receiverIds);

                List<NotificationMessage> sentMessages = new List<NotificationMessage>();

                SortedSet<Guid> emailSentTo = new SortedSet<Guid>();
                SortedSet<Guid> smsSentTo = new SortedSet<Guid>();

                messageLst.Where(m => m.ReceiverUserID.HasValue).ToList()
                    .ForEach(m =>
                    {
                        m.Subject = Expressions.replace(m.Subject, ref not.ReplacementDic, Expressions.Patterns.AutoTag);
                        m.Text = Expressions.replace(m.Text, ref not.ReplacementDic, Expressions.Patterns.AutoTag);

                        if (m.Media == Media.Email &&
                            !emailSentTo.Any(u => u == m.ReceiverUserID) && emailList.Any(e => e.UserID == m.ReceiverUserID))
                        {
                            m.Action = not.Action.Value;
                            m.RefItemID = not.RefItemID.Value;
                            m.SubjectType = not.SubjectType.Value;
                            m.UserStatus =
                                users.Where(u => u.Key == m.ReceiverUserID).Select(u => u.Value).First();
                            m.EmailAddress = emailList.Where(e => e.UserID == m.ReceiverUserID).First();

                            ThreadPool.QueueUserWorkItem(new WaitCallback(m.send_email), applicationId);

                            emailSentTo.Add(m.ReceiverUserID.Value);
                        }

                        if (m.Media == Media.SMS &&
                            !smsSentTo.Any(u => u == m.ReceiverUserID) && phoneList.Any(p => p.UserID == m.ReceiverUserID))
                        {
                            m.Action = not.Action.Value;
                            m.RefItemID = not.RefItemID.Value;
                            m.SubjectType = not.SubjectType.Value;
                            m.UserStatus =
                                users.Where(u => u.Key == m.ReceiverUserID).Select(u => u.Value).First();
                            m.PhoneNumber = phoneList.Where(p => p.UserID == m.ReceiverUserID).First();
                            ThreadPool.QueueUserWorkItem(new WaitCallback(m.send_sms));

                            smsSentTo.Add(m.ReceiverUserID.Value);
                        }
                    });
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "send_notification_messages", ex, ModuleIdentifier.NTFN);
            }
        }

        private static List<NotificationMessage> _get_notification_messages_info(Guid applicationId, 
            List<KeyValuePair<Guid, UserStatus>> userStatusPairList, SubjectType subjectType, ActionType action)
        {
            if (userStatusPairList == null) userStatusPairList = new List<KeyValuePair<Guid, UserStatus>>();

            DBCompositeType<GuidStringTableType> usersParam = new DBCompositeType<GuidStringTableType>()
                .add(userStatusPairList.Select(i => new GuidStringTableType(i.Key, i.Value.ToString())).ToList());

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetNotificationMessagesInfo"),
                applicationId, RaaiVanSettings.ReferenceTenantID, usersParam, subjectType.ToString(), action.ToString());

            return NTFNParsers.notification_messages_info(results, applicationId);
        }

        public static bool set_admin_messaging_activation(Guid applicationId, Guid templateId, Guid currentUserId, 
            SubjectType subjectType, ActionType action, Media media, UserStatus userStatus, string lang, bool enable)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetAdminMessagingActivation"),
                applicationId, templateId, currentUserId, DateTime.Now, subjectType, action, media, userStatus, lang, enable);
        }

        public static bool set_notification_message_template_text(Guid applicationId, 
            Guid templateId, Guid currentUserId, string subject, string text)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetNotificationMessageTemplateText"),
                applicationId, templateId, currentUserId, DateTime.Now, subject, text);
        }

        public static bool set_user_messaging_activation(Guid applicationId, Guid optionId, 
            Guid userId, Guid currentUserId, SubjectType subjectType, UserStatus userStatus, 
            ActionType action, Media media, string lang, bool enable)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetUserMessagingActivation"),
                applicationId, optionId, userId, currentUserId, DateTime.Now, subjectType, userStatus, action, media, lang, enable);
        }

        public static List<NotificationMessageTemplate> get_notification_message_templates_info(Guid applicationId)
        {
            return NTFNParsers.notification_message_template(
                DBConnector.read(applicationId, GetFullyQualifiedName("GetNotificationMessageTemplatesInfo"), applicationId));
        }

        public static List<MessagingActivationOption> get_user_messaging_activation(Guid applicationId, Guid userId)
        {
            return NTFNParsers.messaging_activation_option(
                DBConnector.read(applicationId, GetFullyQualifiedName("GetUserMessagingActivation"),
                applicationId, RaaiVanSettings.ReferenceTenantID, userId));
        }

        //end of Notification Messages
    }
}
