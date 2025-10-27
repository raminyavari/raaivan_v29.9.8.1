<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Polls.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.Polls"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="pollsArea" class="small-12 medium-12 large-12 row align-center" style="margin:0rem; padding:0vw 6vw;"></div>

    <script type="text/javascript">
        (function () {
            var pollsArea = document.getElementById("pollsArea");

            GlobalUtilities.load_files(["Polls/PollsManager.js"], {
                OnLoad: function () { new PollsManager("pollsArea"); }
            });
        })();
    </script>
</asp:Content>
