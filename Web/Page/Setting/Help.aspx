<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.Help" 
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="helpArea" class="small-12 medium-12 large-12 row" style="margin:0rem; padding:1rem 6vw;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["RaaiVanHelp/HelpAdmin.js", "TreeView/TreeView.js"], {
                OnLoad: function () { new HelpAdmin("helpArea"); }
            });
        })();
    </script>
</asp:Content>
