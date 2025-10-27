<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentTrees.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.DocumentTrees"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="treesArea" class="small-12 medium-12 large-12 row align-center" 
        style="margin:0rem; margin-bottom:5rem; padding:0vw 6vw;">
    </div>

    <script type="text/javascript">
        (function () {
            var _div = document.getElementById("treesArea");

            GlobalUtilities.loading(_div);

            GlobalUtilities.load_files(["DCT/DocumentTreesManager.js"], {
                OnLoad: function () { new DocumentTreesManager(_div); }
            });
        })();
    </script>
</asp:Content>