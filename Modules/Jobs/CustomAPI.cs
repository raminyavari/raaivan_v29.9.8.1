using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Jobs
{
    public class CustomAPI
    {
        public static string handle_request(Guid applicationId, Guid currentUserId, string command, HttpRequest req)
        {
            command = command.ToLower();
            Dictionary<string, object> response = new Dictionary<string, object>();

            switch (command)
            {
            }

            return PublicMethods.toJSON(response);
        }
    }
}
