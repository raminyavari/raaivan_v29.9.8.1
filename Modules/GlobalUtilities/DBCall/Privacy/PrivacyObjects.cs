using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Privacy
{
    public enum PrivacyObjectType
    {
        None,
        Node,
        NodeType,
        FAQCategory,
        QAWorkFlow,
        Poll,
        Report,
        FormElement
    }

    public enum PrivacyType
    {
        NotSet,
        Public,
        Restricted
    }

    public enum PermissionType
    {
        None,
        Create,
        CreateBulk,
        View,
        ViewAbstract,
        ViewRelatedItems,
        Modify,
        Delete,
        Download
    }
    
    public static class PrivacyUtilities
    {
        public static List<Audience> parse_audience_request_param(string audience)
        {
            List<Audience> retList = new List<Audience>();

            string[] items = audience.Split('|');
            foreach (string _item in items)
            {
                string[] elems = _item.Split(':');

                Audience au = new Audience();

                if (elems.Length > 0) au.RoleID = Guid.Parse(elems[0]);
                if (elems.Length > 1) au.Allow = elems[1].ToLower() == "true";

                if(elems.Length > 0) retList.Add(au);
            }

            return retList;
        }
    }

    public class DefaultPermission {
        public PermissionType PermissionType;
        public PrivacyType DefaultValue;

        public DefaultPermission() {
            PermissionType = PermissionType.None;
            DefaultValue = PrivacyType.NotSet;
        }

        public string toJson() {
            return "{\"PermissionType\":" + (PermissionType == PermissionType.None ? "null" : "\"" + PermissionType + "\"") +
                ",\"DefaultValue\":" + (DefaultValue == PrivacyType.NotSet ? "null" : "\"" + DefaultValue + "\"") +
                "}";
        }

        public static DefaultPermission fromJson(Dictionary<string, object> json)
        {
            DefaultPermission dp = new DefaultPermission();

            PermissionType pt = PermissionType.None;
            if (json.ContainsKey("PermissionType") && json["PermissionType"] != null &&
                Enum.TryParse<PermissionType>(json["PermissionType"].ToString(), out pt)) dp.PermissionType = pt;

            PrivacyType dv = PrivacyType.NotSet;
            if (json.ContainsKey("DefaultValue") && json["DefaultValue"] != null &&
                Enum.TryParse<PrivacyType>(json["DefaultValue"].ToString(), out dv)) dp.DefaultValue = dv;

            return dp.PermissionType == PermissionType.None ? null : dp;
        }
    }

    public class Audience
    {
        public Guid? ObjectID;
        public Guid? RoleID;
        public string RoleName;
        public string RoleType;
        public string NodeType;
        public string AdditionalID;
        public PermissionType PermissionType;
        public bool? Allow;
        public DateTime? ExpirationDate;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;

        public Audience() {
            PermissionType = PermissionType.None;
        }

        public string toJson()
        {
            return "{\"ObjectID\":" + (!ObjectID.HasValue ? "null" : "\"" + ObjectID.Value.ToString() + "\"") +
                ",\"RoleID\":" + (!RoleID.HasValue ? "null" : "\"" + RoleID.Value.ToString() + "\"") +
                ",\"RoleName\":\"" + Base64.encode(RoleName) + "\"" +
                ",\"RoleType\":\"" + Base64.encode(RoleType) + "\"" +
                ",\"NodeType\":\"" + Base64.encode(NodeType) + "\"" +
                ",\"AdditionalID\":\"" + Base64.encode(AdditionalID) + "\"" +
                ",\"Allow\":" + (!Allow.HasValue ? "null" : Allow.Value.ToString().ToLower()) +
                ",\"ExpirationDate\":" + (!ExpirationDate.HasValue ? "null" : "\"" + ExpirationDate.Value.ToString("yyyy-MM-dd") + "\"") +
                ",\"ExpirationDate_Locale\":" + (!ExpirationDate.HasValue ? "null" : 
                    "\"" + GenericDate.get_local_date(ExpirationDate.Value) + "\"") +
                ",\"PermissionType\":" + (PermissionType == PermissionType.None ? "null" : "\"" + PermissionType.ToString() + "\"") +
                "}";
        }

        public static Audience fromJson(Dictionary<string, object> json)
        {
            Audience obj = new Audience();

            if (json.ContainsKey("RoleID") && json["RoleID"] != null)
                obj.RoleID = PublicMethods.parse_guid(json["RoleID"].ToString());
            if (json.ContainsKey("Allow") && json["Allow"] != null)
                obj.Allow = PublicMethods.parse_bool(json["Allow"].ToString());
            if (json.ContainsKey("ExpirationDate") && json["ExpirationDate"] != null)
                obj.ExpirationDate = PublicMethods.parse_date(json["ExpirationDate"].ToString());

            PermissionType pt = PermissionType.None;
            if (json.ContainsKey("PermissionType") && json["PermissionType"] != null &&
                Enum.TryParse<PermissionType>(json["PermissionType"].ToString(), out pt)) obj.PermissionType = pt;

            return obj.PermissionType != PermissionType.None && obj.RoleID.HasValue && obj.Allow.HasValue ? obj : null;
        }
    }

    public class Privacy {
        public Guid? ObjectID;
        public bool? CalculateHierarchy;
        public ConfidentialityLevel Confidentiality;
        public List<DefaultPermission> DefaultPermissions;
        public List<Audience> Audience;

        public Privacy() {
            Confidentiality = new ConfidentialityLevel();
            DefaultPermissions = new List<DefaultPermission>();
            Audience = new List<Audience>();
        }

        public string toJson()
        {
            return "{\"ObjectID\":" + (!ObjectID.HasValue ? "null" : "\"" + ObjectID.Value.ToString() + "\"") +
                ",\"CalculateHierarchy\":" + (!CalculateHierarchy.HasValue ? "null" : CalculateHierarchy.Value.ToString().ToLower()) +
                ",\"Confidentiality\":" + Confidentiality.toJson() +
                ",\"Audience\":[" + string.Join(",", Audience.Select(u => u.toJson())) + "]" +
                ",\"DefaultPermissions\":[" + string.Join(",", DefaultPermissions.Select(u => u.toJson())) + "]" +
                "}";
        }

        public static Privacy fromJson(Dictionary<string, object> json)
        {
            Privacy obj = new Privacy();

            obj.CalculateHierarchy = PublicMethods.parse_bool(PublicMethods.get_dic_value(json, "CalculateHierarchy"));

            Dictionary<string, object> conf = PublicMethods.get_dic_value<Dictionary<string, object>>(json, "Confidentiality");
            obj.Confidentiality.ID = PublicMethods.parse_guid(PublicMethods.get_dic_value(conf, "ID"));

            foreach (object a in PublicMethods.get_dic_value<ArrayList>(json, "Audience", defaultValue: new ArrayList()))
            {
                if (a.GetType() != typeof(Dictionary<string, object>)) continue;
                Audience o = Modules.Privacy.Audience.fromJson((Dictionary<string, object>)a);
                if (o != null) obj.Audience.Add(o);
            }

            foreach (object d in PublicMethods.get_dic_value<ArrayList>(json, "DefaultPermissions", defaultValue: new ArrayList()))
            {
                if (d.GetType() != typeof(Dictionary<string, object>)) continue;
                DefaultPermission o = DefaultPermission.fromJson((Dictionary<string, object>)d);
                if (o != null) obj.DefaultPermissions.Add(o);
            }

            return obj.CalculateHierarchy.HasValue || obj.Confidentiality.ID.HasValue ||
                obj.Audience.Count > 0 || obj.DefaultPermissions.Count > 0 ? obj : null;
        }
    }

    public class ConfidentialityLevel
    {
        public Guid? ID;
        public int? LevelID;
        public string Title;
        public User Creator;
        public User Modifier;

        public ConfidentialityLevel()
        {
            Creator = new User();
            Modifier = new User();
        }

        public string toJson() {
            return "{\"ID\":" + (!ID.HasValue ? "null" : "\"" + ID.Value.ToString() + "\"") +
                ",\"ConfidentialityID\":" + (!ID.HasValue ? "null" : "\"" + ID.Value.ToString() + "\"") +
                ",\"LevelID\":" + (!LevelID.HasValue ? "null" : "\"" + LevelID.Value.ToString() + "\"") +
                ",\"Title\":\"" + Base64.encode(Title) + "\"" +
                "}";
        }
    }

    public class Confidentiality
    {
        public Guid? ObjectID;
        public Guid? LevelID;
    }
}
