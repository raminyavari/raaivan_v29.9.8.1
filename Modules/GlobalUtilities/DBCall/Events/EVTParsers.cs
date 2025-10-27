using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Events
{
    public static class EVTParsers
    {
        public static List<Event> events(DBResultSet results, bool full = false)
        {
            List<Event> retList = new List<Event>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Event item = new Event() {
                    EventID = table.GetGuid(i, "EventID"),
                    Title = table.GetString(i, "Title")
                };

                if (full)
                {
                    item.EventType = table.GetString(i, "EventType");
                    item.Description = table.GetString(i, "Description");
                    item.BeginDate = table.GetDate(i, "BeginDate");
                    item.FinishDate = table.GetDate(i, "FinishDate");
                    item.CreatorUserID = table.GetGuid(i, "CreatorUserID");
                }

                retList.Add(item);
            }

            return retList;
        }

        public static List<RelatedUser> related_users(DBResultSet results, bool userInfo = false, bool eventInfo = false)
        {
            List<RelatedUser> retList = new List<RelatedUser>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                RelatedUser relatedUser = new RelatedUser() {
                    UserInfo = new User() {
                        UserID = table.GetGuid(i, "UserID")
                    },
                    EventInfo = new Event() {
                        EventID = table.GetGuid(i, "EventID")
                    },
                    Status = table.GetString(i, "Status"),
                    Done = table.GetBool(i, "Done"),
                    RealFinishDate = table.GetDate(i, "RealFinishDate")
                };

                if (userInfo)
                {
                    relatedUser.UserInfo.UserName = table.GetString(i, "UserName");
                    relatedUser.UserInfo.FirstName = table.GetString(i, "FirstName");
                    relatedUser.UserInfo.LastName = table.GetString(i, "LastName");
                }

                if (eventInfo)
                {
                    relatedUser.EventInfo.EventType = table.GetString(i, "EventType");
                    relatedUser.EventInfo.Title = table.GetString(i, "Title");
                    relatedUser.EventInfo.Description = table.GetString(i, "Description");
                    relatedUser.EventInfo.BeginDate = table.GetDate(i, "BeginDate");
                    relatedUser.EventInfo.FinishDate = table.GetDate(i, "FinishDate");
                    relatedUser.EventInfo.CreatorUserID = table.GetGuid(i, "CreatorUserID");
                }

                retList.Add(relatedUser);
            }

            return retList;
        }
    }
}
