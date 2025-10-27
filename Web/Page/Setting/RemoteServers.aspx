<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemoteServers.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.RemoteServers"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="remoteServers" class="small-12 medium-12 large-12" style="margin:0; margin-bottom:5rem; padding:0 6vw;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["RemoteServers/RemoteServerSettings.js"], {
                OnLoad: function () {
                    new RemoteServerSettings("remoteServers");
                }
            });
        })();
    </script>
</asp:Content>
