<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Classifications.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.Classifications"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="confidentialityLevels" class="small-12 medium-12 large-12 row align-center" 
        style="margin:0rem; margin-bottom:5rem; padding:0vw 6vw;">
    </div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["Confidentiality/ConfidentialityLevelsManager.js"], {
                OnLoad: function () { new ConfidentialityLevelsManager("confidentialityLevels"); }
            });
        })();
    </script>
</asp:Content>
