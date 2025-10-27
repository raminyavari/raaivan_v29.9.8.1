using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Search
{
    public enum SearchDocType
    {
        All,
        Node,
        NodeType,
        Question,
        File,
        User
    }

    public class SearchDoc
    {
        public Guid? ApplicationID;
        public Guid ID;
        public Guid? TypeID;
        public string Type;
        public string AdditionalID;
        public string Title;
        public string Description;
        public string Tags;
        public bool? Deleted;
        public string Content;
        public string FileContent;
        public bool? AccessIsDenied;
        public SearchDocType SearchDocType;
        public DocFileInfo FileInfo;

        public bool NoContent
        {
            get
            {
                return string.IsNullOrEmpty(Tags) && string.IsNullOrEmpty(Description) &&
                    string.IsNullOrEmpty(Content) && string.IsNullOrEmpty(FileContent) &&
                    SearchDocType != SearchDocType.User && SearchDocType != SearchDocType.NodeType;
            }
        }

        public SearchDoc(Guid? applicationId)
        {
            ApplicationID = applicationId;
            FileInfo = new DocFileInfo(applicationId);
        }

        public SearchDoc(Guid? applicationId, Guid id, Guid? typeId, string content, string additionalId, bool deleted, string type,
            SearchDocType docType, string title = null, string tags = null, string description = null, string fileContect = null)
        {
            ID = id;
            Deleted = deleted;
            TypeID = typeId;
            Type = type;
            AdditionalID = additionalId;
            Title = title;
            Description = description;
            Tags = tags;
            Content = content;
            FileContent = fileContect;
            SearchDocType = docType;

            FileInfo = new DocFileInfo(applicationId);
        }

        public string toJson(Guid applicationId, bool exact)
        {
            string iconUrl = string.Empty;

            switch (SearchDocType)
            {
                case SearchDocType.Node:
                    iconUrl = DocumentUtilities.get_icon_url(applicationId, ID, DefaultIconTypes.Node, TypeID);
                    break;
                case SearchDocType.User:
                    iconUrl = DocumentUtilities.get_personal_image_address(applicationId, ID);
                    break;
                case SearchDocType.File:
                    iconUrl = DocumentUtilities.get_icon_url(applicationId, Type);
                    break;
            }

            return "{\"ID\":\"" + ID.ToString() + "\"" +
                ",\"ItemType\":\"" + SearchDocType.ToString() + "\"" +
                ",\"Type\":\"" + Base64.encode(Type) + "\"" +
                ",\"AdditionalID\":\"" + Base64.encode(AdditionalID) + "\"" +
                ",\"IconURL\":\"" + iconUrl + "\"" +
                ",\"Title\":\"" + Base64.encode(Title) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"Exact\":" + exact.ToString().ToLower() +
                ",\"AccessIsDenied\":" + (AccessIsDenied.HasValue && AccessIsDenied.Value).ToString().ToLower() +
                (FileInfo == null || !FileInfo.OwnerNodeID.HasValue ? string.Empty :
                    ",\"FileOwnerNode\":{\"NodeID\":\"" + FileInfo.OwnerNodeID.ToString() + "\"" +
                        ",\"Name\":\"" + Base64.encode(FileInfo.OwnerNodeName) + "\"" +
                        ",\"NodeType\":\"" + Base64.encode(FileInfo.OwnerNodeType) + "\"" +
                    "}"
                ) +
                "}";
        }
    }

    public class SearchOptions
    {
        public List<SearchDocType> DocTypes;
        public List<Guid> TypeIDs;
        public List<string> Types;
        public bool AdditionalID;
        public bool Title;
        public bool Description;
        public bool Content;
        public bool Tags;
        public bool FileContent;
        public bool ForceHasContent;
        public string Phrase;
        public bool Highlight;

        public Dictionary<string, object> CustomData;

        public int Count;
        public int LowerBoundary;
        public int TotalCount;

        public SearchOptions(List<SearchDocType> docTypes = null, List<Guid> typeIds = null, List<string> types = null, 
            bool additionalId = false, bool title = false, bool description = false, bool content = false, bool tags = false, 
            bool fileContent = false, bool forceHasContent = false, bool highlight = false, string phrase = null,
            int count = 20, int lowerBoundary = 0)
        {
            DocTypes = docTypes == null ? new List<SearchDocType>() : docTypes;
            TypeIDs = typeIds == null? new List<Guid>() : typeIds;
            Types = types == null ? new List<string>() : types;
            AdditionalID = additionalId;
            Title = title;
            Description = description;
            Content = content;
            Tags = tags;
            FileContent = fileContent;
            ForceHasContent = forceHasContent;
            Highlight = highlight;
            Phrase = phrase;

            Count = count;
            LowerBoundary = lowerBoundary;
            TotalCount = 0;

            CustomData = new Dictionary<string, object>();
        }
    }
}
