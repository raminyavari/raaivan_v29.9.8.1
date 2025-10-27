using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.NotificationCenter
{
    public static class NTFNParsers
    {
        public static List<Notification> notifications(DBResultSet results)
        {
            List<Notification> retList = new List<Notification>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Notification()
                {
                    NotificationID = table.GetLong(i, "NotificationID"),
                    UserID = table.GetGuid(i, "UserID"),
                    SubjectID = table.GetGuid(i, "SubjectID"),
                    RefItemID = table.GetGuid(i, "RefItemID"),
                    SubjectName = table.GetString(i, "SubjectName"),
                    SubjectType = table.GetEnum<SubjectType>(i, "SubjectType", SubjectType.None),
                    Sender = new User()
                    {
                        UserID = table.GetGuid(i, "SenderUserID"),
                        UserName = table.GetString(i, "SenderUserName"),
                        FirstName = table.GetString(i, "SenderFirstName"),
                        LastName = table.GetString(i, "SenderLastName")
                    },
                    Action = table.GetEnum<ActionType>(i, "Action", ActionType.None),
                    Description = table.GetString(i, "Description"),
                    Info = table.GetString(i, "Info"),
                    UserStatus = table.GetEnum<UserStatus>(i, "UserStatus", UserStatus.None),
                    SendDate = table.GetDate(i, "SendDate"),
                    Seen = table.GetBool(i, "Seen"),
                    ViewDate = table.GetDate(i, "ViewDate")
                });
            }

            return retList;
        }

        public static List<MessageTemplate> message_templates(DBResultSet results)
        {
            List<MessageTemplate> retList = new List<MessageTemplate>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new MessageTemplate()
                {
                    TemplateID = table.GetGuid(i, "TemplateID"),
                    OwnerID = table.GetGuid(i, "OwnerID"),
                    BodyText = table.GetString(i, "BodyText"),
                    AudienceType = table.GetEnum<AudienceType>(i, "AudienceType", AudienceType.NotSet),
                    AudienceRefOwnerID = table.GetGuid(i, "AudienceRefOwnerID"),
                    AudienceNodeID = table.GetGuid(i, "AudienceNodeID"),
                    AudienceNodeName = table.GetString(i, "AudienceNodeName"),
                    AudienceNodeTypeID = table.GetGuid(i, "AudienceNodeTypeID"),
                    AudienceNodeType = table.GetString(i, "AudienceNodeType"),
                    AudienceNodeAdmin = table.GetBool(i, "AudienceNodeAdmin")
                });
            }

            return retList;
        }

        public static List<NotificationMessage> notification_messages_info(DBResultSet results, Guid? applicationId)
        {
            List<NotificationMessage> retList = new List<NotificationMessage>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new NotificationMessage()
                {
                    ReceiverUserID = table.GetGuid(i, "UserID"),
                    Lang = table.GetString(i, "Lang"),
                    Subject = table.GetString(i, "Subject"),
                    Text = EmailTemplates.inject_into_master(applicationId, table.GetString(i, "Text")),
                    Media = table.GetEnum<Media>(i, "Media", Media.None)
                });
            }

            return retList;
        }

        public static List<NotificationMessageTemplate> notification_message_template(DBResultSet results)
        {
            List<NotificationMessageTemplate> retList = new List<NotificationMessageTemplate>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new NotificationMessageTemplate()
                {
                    TemplateId = table.GetGuid(i, "TemplateID"),
                    Enable = table.GetBool(i, "Enable"),
                    Lang = table.GetString(i, "Lang"),
                    Subject = table.GetString(i, "Subject"),
                    Text = table.GetString(i, "Text"),
                    SubjectType = table.GetEnum<SubjectType>(i, "SubjectType", defaultValue: SubjectType.None),
                    Action = table.GetEnum<ActionType>(i, "Action", defaultValue: ActionType.None),
                    Media = table.GetEnum<Media>(i, "Media", defaultValue: Media.None),
                    UserStatus = table.GetEnum<UserStatus>(i, "UserStatus", defaultValue: UserStatus.None)
                });
            }

            return retList;
        }

        public static List<MessagingActivationOption> messaging_activation_option(DBResultSet results)
        {
            List<MessagingActivationOption> retList = new List<MessagingActivationOption>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new MessagingActivationOption()
                {
                    OptionID = table.GetGuid(i, "OptionID"),
                    Lang = table.GetString(i, "Lang"),
                    Enable = table.GetBool(i, "Enable"),
                    AdminEnable = table.GetBool(i, "AdminEnable"),
                    SubjectType = table.GetEnum<SubjectType>(i, "SubjectType", defaultValue: SubjectType.None),
                    UserStatus = table.GetEnum<UserStatus>(i, "UserStatus", defaultValue: UserStatus.None),
                    Action = table.GetEnum<ActionType>(i, "Action", defaultValue: ActionType.None),
                    Media = table.GetEnum<Media>(i, "Media", defaultValue: Media.None)
                });
            }

            return retList;
        }
    }
}
