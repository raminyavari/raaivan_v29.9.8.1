using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.DataExchange;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.Privacy;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for ManageDataImports
    /// </summary>
    public class ImportAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private ParamsContainer paramsContainer = null;
        
        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;
            string command = string.IsNullOrEmpty(context.Request.Params["Command"]) ? string.Empty : context.Request.Params["Command"];

            switch (command)
            {
                /*
                case "ImportData":
                    string attachedFile = string.IsNullOrEmpty(context.Request.Params["AttachedFile"]) ? string.Empty :
                        context.Request.Params["AttachedFile"];

                    string strGuidName = attachedFile.Split(',')[0];
                    string folderAddress = DocumentUtilities.map_path(paramsContainer.Tenant.Id, 
                        FolderNames.TemporaryFiles);

                    handle_imported_file(folderAddress + "\\" + strGuidName, ref responseText);
                    _return_response(ref responseText);
                    return;
                */
                case "ImportData":
                    List<DocFileInfo> files = 
                        DocumentUtilities.get_files_info(paramsContainer.ApplicationID, context.Request.Params["AttachedFile"]);

                    if (files == null || files.Count != 1)
                        responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                    else
                    {
                        files[0].FolderName = FolderNames.TemporaryFiles;

                        handle_imported_file(files[0], ref responseText);
                    }
                    
                    _return_response(ref responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected void handle_imported_file(DocFileInfo file, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.DataImport, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            if (!file.exists())
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                using (MemoryStream stream = new MemoryStream(file.toByteArray()))
                    xmlDoc.Load(stream);
            }
            catch (Exception ex)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }
            
            string docName = xmlDoc.DocumentElement.Name.ToLower();

            bool result = false;

            switch (docName)
            {
                case "nodes":
                    result = update_nodes(ref xmlDoc);
                    break;
                case "nodeids":
                    result = update_node_ids(ref xmlDoc);
                    break;
                case "removenodes":
                    result = remove_nodes(ref xmlDoc);
                    break;
                case "users":
                    result = update_users(ref xmlDoc);
                    break;
                case "members":
                    result = update_members(ref xmlDoc);
                    break;
                case "experts":
                    result = update_experts(ref xmlDoc);
                    break;
                case "relations":
                    result = update_relations(ref xmlDoc);
                    break;
                case "authors":
                    result = update_authors(ref xmlDoc);
                    break;
                case "userconfidentialities":
                    result = update_user_confidentialities(ref xmlDoc);
                    break;
                case "permissions":
                    result = update_permissions(ref xmlDoc);
                    break;
            }

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected bool update_nodes(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            string nodeType = PublicMethods.verify_string(doc.DocumentElement.Attributes["nodetype"].Value);

            List<ExchangeNode> exNodes = new List<ExchangeNode>();

            XmlNodeList nodes = doc.GetElementsByTagName("Node");
            foreach (XmlNode _nd in nodes)
            {
                string tags = _nd.Attributes["tags"] == null || _nd.Attributes["tags"].ToString() == string.Empty ?
                        null : _nd.Attributes["tags"].Value.Trim();

                if (!string.IsNullOrEmpty(tags))
                    tags = tags.Replace('-', '~').Replace(';', '~').Replace(',', '~');

                ExchangeNode newNode = new ExchangeNode()
                {
                    Name = _nd.Attributes["name"] == null ? null : _nd.Attributes["name"].Value.Trim(),
                    AdditionalID = _nd.Attributes["id"] == null || _nd.Attributes["id"].ToString() == string.Empty ? null :
                        PublicMethods.verify_string(_nd.Attributes["id"].Value.Trim()),
                    ParentAdditionalID = _nd.Attributes["parentid"] == null || _nd.Attributes["parentid"].ToString() == string.Empty ?
                        null : PublicMethods.verify_string(_nd.Attributes["parentid"].Value.Trim()),
                    Abstract = _nd.Attributes["abstract"] == null || _nd.Attributes["abstract"].ToString() == string.Empty ?
                        null : _nd.Attributes["abstract"].Value.Trim(),
                    Tags = tags
                };

                if (string.IsNullOrEmpty(newNode.AdditionalID) || !exNodes.Any(
                    u => !string.IsNullOrEmpty(u.AdditionalID) && u.AdditionalID.ToLower() == newNode.AdditionalID.ToLower()))
                    exNodes.Add(newNode);
            }

            exNodes = ExchangeNode.sort_nodes_based_on_parents(exNodes);

            bool result = true;

            PublicMethods.split_list<ExchangeNode>(exNodes, 200, nds =>
            {
                List<Guid> newNodeIds = new List<Guid>();
                
                result = DEController.update_nodes(paramsContainer.Tenant.Id,
                    nds, null, nodeType, paramsContainer.CurrentUserID.Value, ref newNodeIds);
            });

            return result;
        }

        protected bool update_node_ids(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            string nodeType = PublicMethods.verify_string(doc.DocumentElement.Attributes["nodetype"].Value);

            Guid? nodeTypeId = CNController.get_node_type_id(paramsContainer.Tenant.Id, nodeType);

            if (!nodeTypeId.HasValue) return false;

            List<KeyValuePair<string, string>> exNodes = new List<KeyValuePair<string, string>>();

            XmlNodeList nodes = doc.GetElementsByTagName("Node");
            foreach (XmlNode _nd in nodes)
            {
                string id = _nd.Attributes["id"] == null || _nd.Attributes["id"].ToString() == string.Empty ? null :
                    PublicMethods.verify_string(_nd.Attributes["id"].Value.Trim());
                string newId = _nd.Attributes["newid"] == null || _nd.Attributes["newid"].ToString() == string.Empty ? null :
                    PublicMethods.verify_string(_nd.Attributes["newid"].Value.Trim());

                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(newId) && !exNodes.Any(n => n.Value == newId))
                    exNodes.Add(new KeyValuePair<string, string>(id, newId));
            }
            
            bool result = true;

            PublicMethods.split_list<KeyValuePair<string, string>>(exNodes, 200, items =>
            {
                result = DEController.update_node_ids(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, nodeTypeId.Value, items);
            });

            return result;
        }

        protected bool remove_nodes(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            List<KeyValuePair<string, string>> exNodes = new List<KeyValuePair<string, string>>();

            XmlNodeList nodes = doc.GetElementsByTagName("Node");
            foreach (XmlNode _nd in nodes)
            {
                string typeId = _nd.Attributes["typeid"] == null || _nd.Attributes["typeid"].ToString() == string.Empty ? null :
                    PublicMethods.verify_string(_nd.Attributes["typeid"].Value.Trim());
                string id = _nd.Attributes["id"] == null || _nd.Attributes["id"].ToString() == string.Empty ? null :
                    PublicMethods.verify_string(_nd.Attributes["id"].Value.Trim());

                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(typeId) && !exNodes.Any(n => n.Value == typeId))
                    exNodes.Add(new KeyValuePair<string, string>(typeId, id));
            }

            bool result = true;

            PublicMethods.split_list<KeyValuePair<string, string>>(exNodes, 200, items =>
            {
                result = DEController.remove_nodes(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, items);
            });

            return result;
        }

        protected bool update_users(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            List<ExchangeUser> exUsers = new List<ExchangeUser>();

            List<string> usernames = new List<string>();

            XmlNodeList users = doc.GetElementsByTagName("User");
            foreach (XmlNode _usr in users)
            {
                ExchangeUser eu = new ExchangeUser()
                {
                    UserName = _usr.Attributes["username"] == null ? null :
                        PublicMethods.verify_string(_usr.Attributes["username"].Value.Trim()),
                    NewUserName = _usr.Attributes["newusername"] == null ? null :
                        PublicMethods.verify_string(_usr.Attributes["newusername"].Value.Trim()),
                    FirstName = _usr.Attributes["firstname"] == null ? null : _usr.Attributes["firstname"].Value.Trim(),
                    LastName = _usr.Attributes["lastname"] == null ? null : _usr.Attributes["lastname"].Value.Trim(),
                    DepartmentID = _usr.Attributes["departmentid"] == null ? null : _usr.Attributes["departmentid"].Value.Trim()
                };

                if (!string.IsNullOrEmpty(eu.UserName) && eu.UserName.ToLower() == "admin") eu.NewUserName = null;

                if (string.IsNullOrEmpty(eu.UserName) || 
                    exUsers.Any(u => u.UserName.ToLower() == eu.UserName.ToLower())) continue;
                
                EmploymentType empType = EmploymentType.NotSet;
                if (_usr.Attributes["employmenttype"] == null ||
                    !Enum.TryParse<EmploymentType>(_usr.Attributes["employmenttype"].Value.Trim(), out empType))
                {
                    empType = EmploymentType.NotSet;
                }
                eu.EmploymentType = empType;
                
                if (_usr.Attributes["ismanager"] != null) eu.IsManager = PublicMethods.parse_bool(_usr.Attributes["ismanager"].Value);
                if (_usr.Attributes["resetpassword"] != null) eu.ResetPassword = PublicMethods.parse_bool(_usr.Attributes["resetpassword"].Value);
                if (eu.NewUserName == eu.UserName) eu.NewUserName = null;

                string password = eu.UserName;
                while (password.Length < 5) password += eu.UserName;
                eu.Password = new Password(password);
                
                exUsers.Add(eu);
            }

            bool result = true;

            PublicMethods.split_list<ExchangeUser>(exUsers, 200, items => {
                result = DEController.update_users(paramsContainer.Tenant.Id, items);
            });

            return result;
        }

        protected bool update_members(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            List<ExchangeMember> exMembers = new List<ExchangeMember>();

            List<string> usernames = new List<string>();
            
            XmlNodeList members = doc.GetElementsByTagName("Member");
            foreach (XmlNode _mbr in members)
            {
                ExchangeMember mm = new ExchangeMember()
                {
                    NodeTypeAdditionalID = _mbr.Attributes["nodetypeid"] == null ? null :
                        PublicMethods.verify_string(_mbr.Attributes["nodetypeid"].Value.Trim()),
                    NodeAdditionalID = _mbr.Attributes["nodeid"] == null ? null :
                        PublicMethods.verify_string(_mbr.Attributes["nodeid"].Value.Trim()),
                    UserName = _mbr.Attributes["username"] == null ? null :
                        PublicMethods.verify_string(_mbr.Attributes["username"].Value.Trim()),
                    IsAdmin = _mbr.Attributes["isadmin"] != null && _mbr.Attributes["isadmin"].Value.Trim().ToLower() == "true"
                };

                Guid nodeId = Guid.Empty;
                if (string.IsNullOrEmpty(mm.NodeTypeAdditionalID) && Guid.TryParse(mm.NodeAdditionalID, out nodeId))
                    mm.NodeID = nodeId;

                exMembers.Add(mm);
            }

            bool result = true;

            PublicMethods.split_list<ExchangeMember>(exMembers, 200, items => {
                result = DEController.update_members(paramsContainer.Tenant.Id, items);
            });

            return result;
        }

        protected bool update_experts(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            List<ExchangeMember> exExperts = new List<ExchangeMember>();

            List<string> usernames = new List<string>();

            XmlNodeList experts = doc.GetElementsByTagName("Expert");
            foreach (XmlNode _xprt in experts)
            {
                ExchangeMember ex = new ExchangeMember()
                {
                    NodeTypeAdditionalID = _xprt.Attributes["nodetypeid"] == null ? null :
                        PublicMethods.verify_string(_xprt.Attributes["nodetypeid"].Value.Trim()),
                    NodeAdditionalID = _xprt.Attributes["nodeid"] == null ? null :
                        PublicMethods.verify_string(_xprt.Attributes["nodeid"].Value.Trim()),
                    UserName = _xprt.Attributes["username"] == null ? null :
                        PublicMethods.verify_string(_xprt.Attributes["username"].Value.Trim())
                };

                Guid nodeId = Guid.Empty;
                if (string.IsNullOrEmpty(ex.NodeTypeAdditionalID) && Guid.TryParse(ex.NodeAdditionalID, out nodeId))
                    ex.NodeID = nodeId;

                exExperts.Add(ex);
            }

            bool result = true;

            PublicMethods.split_list<ExchangeMember>(exExperts, 200, items => {
                result = DEController.update_experts(paramsContainer.Tenant.Id, items);
            });

            return result;
        }

        protected bool update_relations(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            List<ExchangeRelation> exRelations = new List<ExchangeRelation>();

            XmlNodeList relations = doc.GetElementsByTagName("Relation");
            foreach (XmlNode r in relations)
            {
                ExchangeRelation rel = new ExchangeRelation()
                {
                    SourceTypeAdditionalID = r.Attributes["nodetypeid"] == null ? null :
                        PublicMethods.verify_string(r.Attributes["nodetypeid"].Value.Trim()),
                    SourceAdditionalID = r.Attributes["nodeid"] == null ? null :
                        PublicMethods.verify_string(r.Attributes["nodeid"].Value.Trim()),
                    DestinationTypeAdditionalID = r.Attributes["relatednodetypeid"] == null ? null :
                        PublicMethods.verify_string(r.Attributes["relatednodetypeid"].Value.Trim()),
                    DestinationAdditionalID = r.Attributes["relatedid"] == null ? null :
                        PublicMethods.verify_string(r.Attributes["relatedid"].Value.Trim()),
                    Bidirectional = false //r.Attributes["bidirectional"] == null ? false : r.Attributes["bidirectional"].Value.ToLower() == "true"
                };
                
                exRelations.Add(rel);
            }

            bool result = true;

            PublicMethods.split_list<ExchangeRelation>(exRelations, 200, items => {
                result = DEController.update_relations(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, items);
            });

            return result;
        }

        protected bool update_authors(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            List<ExchangeAuthor> exAuthors = new List<ExchangeAuthor>();

            XmlNodeList authors = doc.GetElementsByTagName("Author");
            foreach (XmlNode a in authors)
            {
                ExchangeAuthor item = new ExchangeAuthor()
                {
                    NodeTypeAdditionalID = a.Attributes["nodetypeid"] == null ? null :
                        PublicMethods.verify_string(a.Attributes["nodetypeid"].Value.Trim()),
                    NodeAdditionalID = a.Attributes["nodeid"] == null ? null :
                        PublicMethods.verify_string(a.Attributes["nodeid"].Value.Trim()),
                    UserName = a.Attributes["username"] == null ? null :
                        PublicMethods.verify_string(a.Attributes["username"].Value.Trim()),
                    Percentage = a.Attributes["percentage"] == null ? null : PublicMethods.parse_int(a.Attributes["percentage"].Value.Trim())
                };

                if (!string.IsNullOrEmpty(item.NodeTypeAdditionalID) && !string.IsNullOrEmpty(item.NodeAdditionalID) &&
                    !string.IsNullOrEmpty(item.UserName) && item.Percentage.HasValue && item.Percentage > 0 &&
                    item.Percentage <= 100 && !exAuthors.Any(u =>
                        u.NodeTypeAdditionalID.ToLower() == item.NodeTypeAdditionalID.ToLower() &&
                        u.NodeAdditionalID.ToLower() == item.NodeAdditionalID.ToLower() &&
                        u.UserName.ToLower() == item.UserName.ToLower())) exAuthors.Add(item);
            }

            exAuthors = ExchangeAuthor.sort_items(exAuthors);

            int SIZE = 200;

            bool result = true;

            while (exAuthors.Count > 0)
            {
                List<ExchangeAuthor> newItems = exAuthors.Take(Math.Min(exAuthors.Count, SIZE)).ToList();
                ExchangeAuthor lastItem = newItems[newItems.Count - 1];

                exAuthors.RemoveRange(0, newItems.Count);

                List<ExchangeAuthor> additionalItems = exAuthors.TakeWhile(
                    u => u.NodeTypeAdditionalID.ToLower() == lastItem.NodeTypeAdditionalID.ToLower() &&
                    u.NodeAdditionalID.ToLower() == lastItem.NodeAdditionalID.ToLower()).ToList();

                if (additionalItems.Count > 0) {
                    newItems.AddRange(additionalItems);
                    exAuthors.RemoveRange(0, additionalItems.Count);
                }

                result = DEController.update_authors(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, newItems);
            }
            
            return result;
        }

        protected bool update_user_confidentialities(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            List<KeyValuePair<string, int>> exItems = new List<KeyValuePair<string, int>>();

            XmlNodeList authors = doc.GetElementsByTagName("Item");

            foreach (XmlNode a in authors)
            {
                string un = a.Attributes["username"] == null ? null :
                        PublicMethods.verify_string(a.Attributes["username"].Value.Trim());
                int? lId = a.Attributes["levelid"] == null ? null : PublicMethods.parse_int(a.Attributes["levelid"].Value.Trim());

                if (!string.IsNullOrEmpty(un) && lId.HasValue && lId.Value > 0) exItems.Add(new KeyValuePair<string, int>(un, lId.Value));
            }

            bool result = true;

            PublicMethods.split_list<KeyValuePair<string, int>>(exItems, 200, items => {
                result = DEController.update_user_confidentialities(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, items);
            });

            return result;
        }

        protected bool update_permissions(ref XmlDocument doc)
        {
            if (!paramsContainer.GBEdit) return false;

            List<ExchangePermission> exDropAll = new List<ExchangePermission>();
            List<ExchangePermission> exPermissions = new List<ExchangePermission>();
            
            XmlNodeList items = doc.GetElementsByTagName("Item");
            foreach (XmlNode a in items)
            {
                PermissionType pt = PermissionType.None;
                if (a.Attributes["permission"] != null) Enum.TryParse<PermissionType>(a.Attributes["permission"].Value.Trim(), out pt);

                bool? deny = a.Attributes["deny"] == null ? null : PublicMethods.parse_bool(a.Attributes["deny"].Value.Trim());
                bool? dropAll = a.Attributes["drop_all"] == null ? null : PublicMethods.parse_bool(a.Attributes["drop_all"].Value.Trim());

                ExchangePermission item = new ExchangePermission()
                {
                    NodeTypeAdditionalID = a.Attributes["nodetypeid"] == null ? null :
                        PublicMethods.verify_string(a.Attributes["nodetypeid"].Value.Trim()),
                    NodeAdditionalID = a.Attributes["nodeid"] == null ? null :
                        PublicMethods.verify_string(a.Attributes["nodeid"].Value.Trim()),
                    GroupTypeAdditionalID = a.Attributes["grouptypeid"] == null ? null :
                        PublicMethods.verify_string(a.Attributes["grouptypeid"].Value.Trim()),
                    GroupAdditionalID = a.Attributes["groupid"] == null ? null :
                        PublicMethods.verify_string(a.Attributes["groupid"].Value.Trim()),
                    UserName = a.Attributes["username"] == null ? null :
                        PublicMethods.verify_string(a.Attributes["username"].Value.Trim()),
                    PermissionType = pt,
                    Allow = !deny.HasValue || !deny.Value
                };

                if (!string.IsNullOrEmpty(item.NodeTypeAdditionalID) && !string.IsNullOrEmpty(item.NodeAdditionalID))
                {
                    if (dropAll.HasValue && dropAll.Value)
                    {
                        exDropAll.Add(new ExchangePermission()
                        {
                            NodeTypeAdditionalID = item.NodeTypeAdditionalID,
                            NodeAdditionalID = item.NodeAdditionalID,
                            DropAll = dropAll
                        });
                    }

                    bool valid = !string.IsNullOrEmpty(item.GroupTypeAdditionalID) && !string.IsNullOrEmpty(item.GroupAdditionalID);
                    valid = valid || !string.IsNullOrEmpty(item.UserName);
                    valid = valid && item.PermissionType != PermissionType.None;

                    if (valid) exPermissions.Add(item);
                }
            }

            bool result = true;

            PublicMethods.split_list<ExchangePermission>(exDropAll, 200, lst => {
                result = DEController.update_permissions(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, lst);
            });

            PublicMethods.split_list<ExchangePermission>(exPermissions, 200, lst => {
                result = DEController.update_permissions(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, lst);
            });

            return result;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}