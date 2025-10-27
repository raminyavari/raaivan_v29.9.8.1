using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using Newtonsoft.Json;
using RaaiVan.Modules.FormGenerator;

namespace RaaiVan.Modules.GlobalUtilities.DBCompositeTypes
{
    [Serializable]
    public class FormInstanceTableType : ITableType
    {
        public FormInstanceTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "FormInstanceTableType"; } }

        [PgName("instance_id")]
        public Guid? InstanceID;

        [PgName("form_id")]
        public Guid? FormID;

        [PgName("owner_id")]
        public Guid? OwnerID;

        [PgName("director_id")]
        public Guid? DirectorID;

        [PgName("admin")]
        public bool? Admin;

        [PgName("is_temporary")]
        public bool? IsTemporary;


        public FormInstanceTableType(Guid? instanceId, Guid? formId, Guid? ownerId, Guid? directorId, bool? admin, bool? isTemporary)
        {
            InstanceID = instanceId;
            FormID = formId;
            OwnerID = ownerId;
            DirectorID = directorId;
            Admin = admin;
            IsTemporary = isTemporary;
        }

        public object[] to_array()
        {
            return new List<object>() {
                InstanceID,
                FormID,
                OwnerID,
                DirectorID,
                Admin,
                IsTemporary
            }.ToArray();
        }

        public FormInstanceTableType[] get_array(List<FormInstanceTableType> list)
        {
            return list.ToArray();
        }

        public static DBCompositeType<FormInstanceTableType> getCompositeType(List<FormType> lst)
        {
            if (lst == null) lst = new List<FormType>();

            return new DBCompositeType<FormInstanceTableType>()
                .add(lst.Select(i => new FormInstanceTableType(
                    instanceId: i.InstanceID,
                    formId: i.FormID,
                    ownerId: i.OwnerID,
                    directorId: i.DirectorID,
                    admin: i.Admin,
                    isTemporary: i.IsTemporary)).ToList());
        }
    }
}
