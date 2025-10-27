<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataImport.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.DataImport"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="dataimport" class="small-12 medium-12 large-12" style="margin-bottom:5rem; padding:0vw 6vw;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.load_files(["RVAdmin/DataImportManager.js"], {
                OnLoad: function () {
                    new DataImportManager(document.getElementById("dataimport"));
                }
            });
        })();
    </script>
</asp:Content>