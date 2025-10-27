<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Graph.aspx.cs" Inherits="RaaiVan.Web.Page.View.Graph"
    MasterPageFile="~/Page/Master/MainMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainMasterBodySection" runat="server">
    <div id="graphContainer"></div>

    <script type="text/javascript">
        (function () {
            document.title = Base64.decode((window.RVGlobal || {}).SystemTitle);

            var filesArr = [
                {
                    Root: "JIT/",
                    Childs: [
                        "base.css",
                        GlobalUtilities.browser_version().Name == "msie" ? "excanvas.js" : null,
                        "jit.js"
                    ].filter(f => !!f)
                },
                "GraphViewer/GraphViewer.js"
            ];

            GlobalUtilities.load_files(filesArr, {
                OnLoad: function () { new GraphViewer("graphContainer"); }
            });
        })();
    </script>
</asp:Content>