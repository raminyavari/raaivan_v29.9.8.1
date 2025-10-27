using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace RaaiVan.Modules.GlobalUtilities.DBCompositeTypes
{
    [Serializable]
    public class PrivacyAudienceTableType : ITableType
    {
        public PrivacyAudienceTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "PrivacyAudienceTableType"; } }

        [PgName("object_id")]
        public Guid? ObjectID;

        [PgName("role_id")]
        public Guid? RoleID;

        [PgName("permission_type")]
        public string PermissionType;

        [PgName("allow")]
        public bool? Allow;

        [PgName("expiration_date")]
        public DateTime? ExpirationDate;

        public PrivacyAudienceTableType(Guid? objectId, Guid? roleId, string permissionType, bool? allow, DateTime? expirationDate)
        {
            ObjectID = objectId;
            RoleID = roleId;
            PermissionType = permissionType;
            Allow = allow;
            ExpirationDate = expirationDate;
        }

        public object[] to_array()
        {
            return new List<object>() {
                ObjectID,
                RoleID,
                PermissionType,
                Allow,
                ExpirationDate
            }.ToArray();
        }

        public PrivacyAudienceTableType[] get_array(List<PrivacyAudienceTableType> list)
        {
            return list.ToArray();
        }
    }
}
