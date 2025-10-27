<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Explorer.aspx.cs" Inherits="RaaiVan.Web.Page.View.Explorer"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="explorerContainer" class="small-12 medium-12 large-12" style="padding:0vw 6vw; margin-bottom:5rem;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.loading("explorerContainer");

            GlobalUtilities.load_files([
                { Root: "CN/CNExplorer/", Childs: ["CNExplorer.css", "CNExplorer.js"] }
            ], { OnLoad: function () { new CNExplorer("explorerContainer"); } });
        })();
    </script>
</asp:Content>