using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;
using RaaiVan.Modules.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaaiVan.Modules.DataExchange
{
    public static class DEController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[DE_" + name + "]"; //'[dbo].' is database owner and 'DE_' is module qualifier
        }

        public static bool update_nodes(Guid applicationId, List<ExchangeNode> nodes, 
            Guid? nodeTypeId, string nodeTypeAdditionalId, Guid currentUserId, ref List<Guid> newNodeIds)
        {
            DBCompositeType<ExchangeNodeTableType> nodesParam = new DBCompositeType<ExchangeNodeTableType>()
                .add(nodes.Select(nd => {
                    if (nd.NodeID == Guid.Empty) nd.NodeID = null;
                    if (string.IsNullOrEmpty(nd.AdditionalID)) nd.AdditionalID = null;
                    if (string.IsNullOrEmpty(nd.Name)) nd.Name = string.Empty;
                    if (string.IsNullOrEmpty(nd.ParentAdditionalID)) nd.ParentAdditionalID = null;
                    if (string.IsNullOrEmpty(nd.Abstract)) nd.Abstract = string.Empty;
                    if (string.IsNullOrEmpty(nd.Tags)) nd.Tags = string.Empty;

                    if (nd.Tags.Length > 1900) nd.Tags = nd.Tags.Substring(0, 1900);

                    return new ExchangeNodeTableType(
                        nodeId: nd.NodeID,
                        nodeAdditionalId: nd.AdditionalID,
                        name: nd.Name.Substring(0, Math.Min(250, nd.Name.Length)),
                        parentAdditionalId: nd.ParentAdditionalID,
                        abstractDesc: nd.Abstract,
                        tags: nd.Tags);
                }).ToList());

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("UpdateNodes"),
                applicationId, nodeTypeId, nodeTypeAdditionalId, nodesParam, currentUserId, DateTime.Now);

            return DEParsers.update_nodes_results(results, ref newNodeIds);
        }

        public static bool update_node_ids(Guid applicationId, 
            Guid currentUserId, Guid nodeTypeId, List<KeyValuePair<string, string>> items)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdateNodeIDs"), applicationId, nodeTypeId,
                string.Join(",", items.Select(i => i.Key + "|" + i.Value)), '|', ',', currentUserId, DateTime.Now);
        }

        public static bool remove_nodes(Guid applicationId, Guid currentUserId, List<KeyValuePair<string, string>> items)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveNodes"),
                applicationId, string.Join(",", items.Select(i => i.Key + "|" + i.Value)), '|', ',', currentUserId, DateTime.Now);
        }

        public static bool update_users(Guid applicationId, List<ExchangeUser> users)
        {
            DBCompositeType<ExchangeUserTableType> usersParam = new DBCompositeType<ExchangeUserTableType>()
                .add(users.Select(u => new ExchangeUserTableType(
                    userId: null,
                    username: u.UserName,
                    newUsername: u.NewUserName,
                    firstName: u.FirstName,
                    lastName: u.LastName,
                    employmentType: u.EmploymentType == EmploymentType.NotSet ? null : u.EmploymentType.ToString(),
                    departmentId: u.DepartmentID,
                    isManager: u.IsManager,
                    email: u.Email,
                    phoneNumber: u.PhoneNumber,
                    resetPassword: u.ResetPassword,
                    password: u.Password.Salted,
                    passwordSalt: u.Password.Salt,
                    encryptedPassword: u.Password.Encrypted)).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdateUsers"), applicationId, usersParam, DateTime.Now);
        }

        public static bool update_members(Guid applicationId, List<ExchangeMember> members)
        {
            DBCompositeType<ExchangeMemberTableType> membersParam = new DBCompositeType<ExchangeMemberTableType>()
                .add(members.Select(m => new ExchangeMemberTableType(
                    nodeTypeAdditionalId: m.NodeTypeAdditionalID,
                    nodeAdditionalId: m.NodeAdditionalID,
                    nodeId: m.NodeID,
                    username: m.UserName,
                    isAdmin: m.IsAdmin)).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdateMembers"), applicationId, membersParam, DateTime.Now);
        }

        public static bool update_experts(Guid applicationId, List<ExchangeMember> experts)
        {
            DBCompositeType<ExchangeMemberTableType> expertsParam = new DBCompositeType<ExchangeMemberTableType>()
                .add(experts.Select(x => new ExchangeMemberTableType(
                    nodeTypeAdditionalId: x.NodeTypeAdditionalID,
                    nodeAdditionalId: x.NodeAdditionalID,
                    nodeId: x.NodeID,
                    username: x.UserName,
                    isAdmin: x.IsAdmin)).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdateExperts"), applicationId, expertsParam, DateTime.Now);
        }

        public static bool update_relations(Guid applicationId, Guid currentUserId, List<ExchangeRelation> relations)
        {
            DBCompositeType<ExchangeRelationTableType> relationsParam = new DBCompositeType<ExchangeRelationTableType>()
                .add(relations.Select(r => new ExchangeRelationTableType(
                    sourceTypeAdditionalId: r.SourceTypeAdditionalID,
                    sourceAdditionalId: r.SourceAdditionalID,
                    sourceId: r.SourceID,
                    destinationTypeAdditionalId: r.DestinationTypeAdditionalID, 
                    destinationAdditionalId: r.DestinationAdditionalID,
                    destinationId: r.DestinationID,
                    bidirectional: r.Bidirectional)).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdateRelations"),
                applicationId, currentUserId, relationsParam, DateTime.Now);
        }

        public static bool update_authors(Guid applicationId, Guid currentUserId, List<ExchangeAuthor> authors)
        {
            DBCompositeType<ExchangeAuthorTableType> authorsParam = new DBCompositeType<ExchangeAuthorTableType>()
                .add(authors.Select(a => new ExchangeAuthorTableType(
                    nodeTypeAdditionalId: a.NodeTypeAdditionalID,
                    nodeAdditionalId: a.NodeAdditionalID,
                    username: a.UserName,
                    percentage: a.Percentage)).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdateAuthors"), 
                applicationId, currentUserId, authorsParam, DateTime.Now);
        }

        public static bool update_user_confidentialities(Guid applicationId, Guid currentUserId, List<KeyValuePair<string, int>> items)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdateUserConfidentialities"), applicationId,
                currentUserId, string.Join(",", items.Select(i => i.Value.ToString() + "|" + i.Key)), '|', ',', DateTime.Now);
        }

        public static bool update_permissions(Guid applicationId, Guid currentUserId, List<ExchangePermission> permissions)
        {
            DBCompositeType<ExchangePermissionTableType> permissionsParam = new DBCompositeType<ExchangePermissionTableType>()
                .add(permissions.Select(p => new ExchangePermissionTableType(
                    nodeTypeAdditionalId: p.NodeTypeAdditionalID,
                    nodeAdditionalId: p.NodeAdditionalID,
                    groupTypeAdditionalId: p.GroupTypeAdditionalID,
                    groupAdditionalId: p.GroupAdditionalID,
                    username: p.UserName,
                    permissionType: p.PermissionType.ToString(),
                    allow: p.Allow,
                    dropAll: p.DropAll)).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdatePermissions"),
                applicationId, currentUserId, permissionsParam, DateTime.Now);
        }
    }
}
