<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.Notifications"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="notmsgAdmin" class="small-12 medium-12 large-12 row" 
        style="margin:0rem; margin-bottom:5rem; padding:0vw 6vw;">
    </div>

    <script type="text/javascript">
        (function () {
            var _div = document.getElementById("notmsgAdmin");

            GlobalUtilities.loading(_div);

            GlobalUtilities.load_files(["Notifications/SendMessageAdminSetting.js"], {
                OnLoad: function () { new SendMessageAdminSetting(_div); }
            });
        })();
    </script>
</asp:Content>
