<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QATag.aspx.cs" Inherits="RaaiVan.Web.Page.View.QATag"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="contentArea" class="small-12 medium-12 large-12 row align-center" style="margin:0rem; padding:0vw 10vw;"></div>

    <script type="text/javascript">
        (function () {
            var initialJson = JSON.parse(document.getElementById("initialJson").value || "{}");
            
            GlobalUtilities.loading("contentArea");

            GlobalUtilities.load_files(["QA/QATagViewer.js"], {
                OnLoad: function () {
                    new QATagViewer("contentArea", {
                        Tag: { ID: initialJson.TagID, Name: Base64.decode(initialJson.Name) }
                    });
                }
            });
        })();
    </script>
</asp:Content>
