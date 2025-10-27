<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QA.aspx.cs" Inherits="RaaiVan.Web.Page.View.QA"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="qaArea" class="small-12 medium-12 large-12 row" style="margin:0; padding:0vw 10vw;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.loading("qaArea");

            GlobalUtilities.load_files(["QA/QuestionsPanel.js"], {
                OnLoad: function () { new QuestionsPanel("qaArea"); }
            });
        })();
    </script>
</asp:Content>
