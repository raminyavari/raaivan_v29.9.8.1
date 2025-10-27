
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemoteSearch.aspx.cs" Inherits="RaaiVan.Web.Page.View.RemoteSearch"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contentArea" class="small-12 medium-12 large-12" style="padding:1rem 10vw 5rem 10vw;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["RemoteServers/RemoteSearch.js"], {
                OnLoad: function () { new RemoteSearch("contentArea"); }
            });

            GlobalUtilities.append_goto_top_button();
        })();
    </script>
</asp:Content>
