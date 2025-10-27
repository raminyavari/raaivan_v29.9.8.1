using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.Privacy;

namespace RaaiVan.Modules.DataExchange
{
    public class ExchangeNode
    {
        public Guid? NodeID;
        public string AdditionalID;
        public string Name;
        public string ParentAdditionalID;
        public string Abstract;
        public string Tags;

        private static List<ExchangeNode> _sort_nodes_based_on_parents(List<ExchangeNode> nodes, string parentId = null)
        {
            List<ExchangeNode> ret = new List<ExchangeNode>();

            if (nodes == null || nodes.Count == 0) return ret;

            if (string.IsNullOrEmpty(parentId)) parentId = null;

            ret.AddRange(nodes.Where(u =>
                (parentId == null && string.IsNullOrEmpty(u.ParentAdditionalID)) || u.ParentAdditionalID == parentId));

            ret.Where(u => !string.IsNullOrEmpty(u.AdditionalID)).ToList()
                .ForEach(x => ret.AddRange(_sort_nodes_based_on_parents(nodes, x.AdditionalID)));

            return ret;
        }

        public static List<ExchangeNode> sort_nodes_based_on_parents(List<ExchangeNode> nodes)
        {
            return _sort_nodes_based_on_parents(nodes);
        }
    }

    public class ExchangeUser
    {
        public string UserName;
        public string NewUserName;
        public string FirstName;
        public string LastName;
        public EmploymentType EmploymentType;
        public string DepartmentID;
        public bool? IsManager;
        public string Email;
        public string PhoneNumber;
        public bool? ResetPassword;
        public Password Password;
        
        public ExchangeUser(){
            EmploymentType = new EmploymentType();
            Password = new Password();
        }
    }

    public class ExchangeMember
    {
        public string NodeTypeAdditionalID;
        public string NodeAdditionalID;
        public Guid? NodeID;
        public string UserName;
        public bool IsAdmin;
    }

    public class ExchangeRelation
    {
        public string SourceTypeAdditionalID;
        public string SourceAdditionalID;
        public Guid? SourceID;
        public string DestinationTypeAdditionalID;
        public string DestinationAdditionalID;
        public Guid? DestinationID;
        public bool? Bidirectional;
    }

    public class ExchangeAuthor
    {
        public string NodeTypeAdditionalID;
        public string NodeAdditionalID;
        public string UserName;
        public int? Percentage;

        public static List<ExchangeAuthor> sort_items(List<ExchangeAuthor> authors)
        {
            return authors.Where(x => !string.IsNullOrEmpty(x.NodeTypeAdditionalID) && !string.IsNullOrEmpty(x.NodeAdditionalID))
                .OrderBy(u => u.NodeTypeAdditionalID.ToLower() + "__" + u.NodeAdditionalID.ToLower()).ToList();
        }
    }

    public class ExchangePermission
    {
        public string NodeTypeAdditionalID;
        public string NodeAdditionalID;
        public string GroupTypeAdditionalID;
        public string GroupAdditionalID;
        public string UserName;
        public PermissionType PermissionType;
        public bool? Allow;
        public bool? DropAll;

        public ExchangePermission() {
            PermissionType = PermissionType.None;
        }
    }
}
