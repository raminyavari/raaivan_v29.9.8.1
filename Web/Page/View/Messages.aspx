<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Messages.aspx.cs" Inherits="RaaiVan.Web.Page.View.Messages" 
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="messagesArea" class="small-12 medium-12 large-12 row" style="margin:0rem 0rem 5rem 0rem; padding:0vw 10vw;"></div>

    <script type="text/javascript">
        (function () {
            var _div = document.getElementById("messagesArea");

            GlobalUtilities.loading(_div);

            GlobalUtilities.load_files(["MSG/Messaging.js"], {
                OnLoad: function () { new Messaging(_div); }
            });
        })();
    </script>
</asp:Content>
