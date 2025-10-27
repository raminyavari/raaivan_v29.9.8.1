<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserGroups.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.UserGroups"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="userGroupsArea" class="small-12 medium-12 large-12 row align-center" 
        style="margin:0rem; margin-bottom:5rem; padding:0vw 6vw;">
    </div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["USR/UserGroupsManager.js"], {
                OnLoad: function () {
                    new UserGroupsManager("userGroupsArea");
                }
            });
        })();
    </script>
</asp:Content>