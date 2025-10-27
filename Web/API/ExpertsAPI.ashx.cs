using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.CoreNetwork;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for ExpertsAPI
    /// </summary>
    public class ExpertsAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;
            string command = string.IsNullOrEmpty(context.Request.Params["Command"]) ? string.Empty : context.Request.Params["Command"];

            Guid userId = string.IsNullOrEmpty(context.Request.Params["UserID"]) ? Guid.Empty : Guid.Parse(context.Request.Params["UserID"]);

            Guid nodeId = string.IsNullOrEmpty(context.Request.Params["NodeID"]) ? Guid.Empty : Guid.Parse(context.Request.Params["NodeID"]);

            switch (command)
            {
                case "SearchExpertiseDomains":
                    string searchText = string.IsNullOrEmpty(context.Request.Params["SearchText"]) ? string.Empty :
                        Base64.decode(context.Request.Params["SearchText"]);
                    int count = string.IsNullOrEmpty(context.Request.Params["Count"]) ? 20 : int.Parse(context.Request.Params["Count"]);
                    search_expertise_domains(searchText, count, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetExpertiseDomains":
                    get_expertise_domains(userId == Guid.Empty && paramsContainer.CurrentUserID.HasValue ? paramsContainer.CurrentUserID.Value : userId, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "IAmExpert":
                    string expertiseDomain = string.IsNullOrEmpty(context.Request.Params["ExpertiseDomain"]) ? string.Empty :
                        Base64.decode(context.Request.Params["ExpertiseDomain"]);
                    i_am_expert(expertiseDomain, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "IAmNotExpert":
                    i_am_not_expert(nodeId, ref responseText);
                    _return_response(ref responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected void search_expertise_domains(string searchText, int count, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            List<Node> nodes = CNController.get_nodes(applicationId: paramsContainer.Tenant.Id,
                nodeType: NodeTypes.Expertise, searchText: searchText, count: count);

            responseText = "{\"Nodes\":[" + "" + string.Join(",", nodes.Select(
                nd => "{\"NodeID\":\"" + nd.NodeID.Value.ToString() +
                    "\",\"Name\":\"" + Base64.encode(nd.Name) + "\"}")) + "]}";
        }

        protected void get_expertise_domains(Guid userId, ref string responseText)
        {
            if (!paramsContainer.GBView) return;

            NodeType nodeType = CNController.get_node_type(paramsContainer.Tenant.Id, NodeTypes.Expertise);
            if (nodeType == null) return;

            List<Expert> domains = CNController.get_expertise_domains(paramsContainer.Tenant.Id,
                userId, null, null, true, nodeType.NodeTypeID.Value);

            responseText = "{\"Nodes\":[";

            bool isFirst = true;
            foreach (Expert ex in domains)
            {
                responseText += (isFirst ? string.Empty : ",") + "{\"NodeID\":\"" + ex.Node.NodeID.Value.ToString() +
                    "\",\"NodeName\":\"" + Base64.encode(ex.Node.Name) +
                    "\",\"ReferralsCount\":\"" + (ex.ReferralsCount.HasValue ? ex.ReferralsCount : 0).ToString() +
                    "\",\"ConfirmsPercentage\":\"" + (ex.ConfirmsPercentage.HasValue ? ex.ConfirmsPercentage : 0).ToString() + "\"}";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void i_am_expert(string expertiseDomain, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            Guid? result = CNController.i_am_expert(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, expertiseDomain);

            responseText = result.HasValue ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"NodeID\":\"" + result.ToString() +
                    "\",\"ReferralsCount\":\"" + CNController.get_referrals_count(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, result.Value).ToString() + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void i_am_not_expert(Guid nodeId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = CNController.i_am_not_expert(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, nodeId);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
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