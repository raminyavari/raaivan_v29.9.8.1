<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Map.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.Map"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="mapArea" class="small-12 medium-12 large-12 row" 
        style="margin:0rem; margin-bottom:5rem;">
    </div>

    <script type="text/javascript">
        (function () {
            var initialJson = GlobalUtilities.to_json(document.getElementById("initialJson").value || "{}") || {};
            
            GlobalUtilities.load_files(["CN/MapManager.js"], {
                OnLoad: function () {
                    new MapManager("mapArea", { IdentityFormID: initialJson.IdentityFormID });
                }
            });
        })();
    </script>
</asp:Content>