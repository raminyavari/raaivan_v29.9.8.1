using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web
{
    /*
    public class RVMembershipProvider : SqlMembershipProvider
    {
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
            
            FieldInfo connectionStringField = GetType().BaseType
                .GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
            connectionStringField.SetValue(this, ProviderUtil.ConnectionString);
        }
    }

    public class RVRoleProvider : SqlRoleProvider
    {
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
            
            FieldInfo connectionStringField = GetType().BaseType
                .GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
            connectionStringField.SetValue(this, ProviderUtil.ConnectionString);
        }
    }

    public class RVProfileProvider : SqlProfileProvider
    {
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            FieldInfo connectionStringField = GetType().BaseType
                .GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
            connectionStringField.SetValue(this, ProviderUtil.ConnectionString);
        }
    }
    */
}