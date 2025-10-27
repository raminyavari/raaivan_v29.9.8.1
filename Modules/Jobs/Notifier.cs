using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Jobs
{
    public class Notifier
    {
        public static void send_dashboards(Guid applicationId, Guid? currentUserId, List<Dashboard> dashboards)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["ApplicationID"] = applicationId;
            if(currentUserId.HasValue) data["CurrentUserID"] = currentUserId.Value;
            data["Dashboards"] = dashboards;

            ThreadPool.QueueUserWorkItem(new WaitCallback(_send_dashboards), data);
        }

        public static void _send_dashboards(object obj) {
            Dictionary<string, object> dic = (Dictionary<string, object>)obj;

            Guid? currentUserId = null;
            if (dic.ContainsKey("CurrentUserID")) currentUserId = (Guid)dic["CurrentUserID"];

            List<Dashboard> failedItems = CustomNotifications.send_dashboards((Guid)dic["ApplicationID"], 
                currentUserId, (List<Dashboard>)dic["Dashboards"]);
        }
    }
}
