<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewQuestion.aspx.cs" Inherits="RaaiVan.Web.Page.View.NewQuestion"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="questionArea" class="small-12 medium-12 large-12 row align-center" 
        style="margin:0; margin-bottom:5rem; padding:0vw 10vw;">
    </div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["QA/NewQuestion.js"], {
                OnLoad: function () { new NewQuestion("questionArea"); }
            });
        })();
    </script>
</asp:Content>
