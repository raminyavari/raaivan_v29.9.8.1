<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Node.aspx.cs" Inherits="RaaiVan.Web.Page.View.Node" 
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="nodeView" class="small-12 medium-12 large-12" style="padding:0vw 10vw; margin-bottom:5rem;"></div>

    <script type="text/javascript">
        (function () {
            var initialJson = JSON.parse(document.getElementById("initialJson").value);
            var container = document.getElementById("nodeView");

            var modules = (window.RVGlobal || {}).Modules || {};
            
            var nodeId = initialJson.NodeID;
            var showWorkFlow = initialJson.ShowWorkFlow === true;
            var showKnowledgeOptions = initialJson.ShowKnowledgeOptions === true;
            var hideContributors = initialJson.HideContributors === true;

            GlobalUtilities.loading(container);

            var loadArr = [
                { Root: "jQuery/highlight/styles/", Childs: ["vs.css"] },
                "CN/NodeViewer.js",
                "CN/NodeAccessDeniedResponse.js"
            ];

            GlobalUtilities.load_files(loadArr, {
                OnLoad: function () {
                    if (initialJson.NodeAccessDenied) return new NodeAccessDeniedResponse(container, initialJson);

                    new NodeViewer(container, {
                        NodeID: nodeId,
                        Modules: modules,
                        ShowWorkFlow: showWorkFlow,
                        ShowKnowledgeOptions: showKnowledgeOptions,
                        HideContributors: hideContributors
                    });
                }
            });

            GlobalUtilities.append_goto_top_button();
        })();
    </script>
</asp:Content>
