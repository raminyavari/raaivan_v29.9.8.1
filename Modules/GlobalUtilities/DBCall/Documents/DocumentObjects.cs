using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Documents
{
    public class Tree
    {
        public Guid? TreeID;
        public bool? IsPrivate;
        public Guid? OwnerID;
        public string Name;
        public string Description;
        public DateTime? CreationDate;
        public Guid? CreatorUserID;
        public string CreatorUserName;
        public string CreatorFirstName;
        public string CreatorLastName;
        public DateTime? LastModificationDate;
        public Guid? LastModifierUserID;
        public bool? IsTemplate;
        public string Privacy;

        public string toJson()
        {
            return "{\"ID\":\"" + (!TreeID.HasValue ? string.Empty : TreeID.Value.ToString()) + "\"" +
                ",\"Title\":\"" + Base64.encode(Name) + "\"" +
                "}";
        }
    }

    public class TreeNode
    {
        public Guid? TreeNodeID;
        public Guid? TreeID;
        public Guid? ParentNodeID;
        public bool? HasChild;
        public string Name;
        public string Description;
        public DateTime? CreationDate;
        public Guid? CreatorUserID;
        public string CreatorUserName;
        public string CreatorFirstName;
        public string CreatorLastName;
        public DateTime? LastModificationDate;
        public Guid? LastModifierUserID;
        public string Privacy;

        public string toJson() {
            return "{\"TreeID\":\"" + (!TreeID.HasValue ? string.Empty : TreeID.Value.ToString()) + "\"" +
                ",\"TreeNodeID\":\"" + (!TreeNodeID.HasValue ? string.Empty : TreeNodeID.Value.ToString()) + "\"" +
                ",\"Title\":\"" + Base64.encode(Name) + "\"" +
                ",\"ParentID\":\"" + (ParentNodeID.HasValue ? ParentNodeID.Value.ToString() : string.Empty) + "\"" +
                (!HasChild.HasValue ? string.Empty : ",\"HasChild\":" + HasChild.ToString().ToLower()) +
                "}";
        }
    }
}
