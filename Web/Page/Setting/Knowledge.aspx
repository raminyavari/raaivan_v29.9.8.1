<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Knowledge.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.Knowledge"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="knowledgeArea" class="small-12 medium-12 large-12 row" 
        style="margin:0rem; margin-bottom:5rem; padding:0vw 6vw;">
    </div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["KnowledgeTypes/KnowledgeTypesManager.js"], {
                OnLoad: function () { new KnowledgeTypesManager("knowledgeArea"); }
            });
        })();
    </script>
</asp:Content>