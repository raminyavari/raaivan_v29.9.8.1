using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace RaaiVan.Modules.Jobs
{
    public class CustomJobs
    {
        public static void do_job(Guid applicationId, string jobName)
        {
            jobName = jobName.ToLower();

            switch (jobName)
            {
            }
        }
    }
}
