<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Configuration.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.Configuration"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="settingsArea" class="small-12 medium-12 large-12 row" style="margin:0 0 3rem 0; padding:0vw 4vw;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["RVAdmin/AdminPanelInitializer.js"], {
                OnLoad: function () { new AdminPanelInitializer(document.getElementById("settingsArea")); }
            });
        })();
    </script>
</asp:Content>
