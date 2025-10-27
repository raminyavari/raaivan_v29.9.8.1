<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="RaaiVan.Web.Page.View.Dashboard" 
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contentArea" class="small-12 medium-12 large-12 row" style="margin:0rem 0rem 5rem 0rem; padding:0vw 8vw;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.loading("contentArea");

            GlobalUtilities.load_files(["Notifications/DashboardsCountViewer.js"], {
                OnLoad: function () { new DashboardsCountViewer("contentArea"); }
            });
        })();
    </script>
</asp:Content>
