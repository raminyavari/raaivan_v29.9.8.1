<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="RaaiVan.Web.Page.View.Help" 
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="helpArea" class="small-12 medium-12 large-12 row" style="margin:0rem; padding:1rem 4vw;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["RaaiVanHelp/RaaiVanHelp.js"], {
                OnLoad: function () { new RaaiVanHelp("helpArea"); }
            });
        })();
    </script>
</asp:Content>
