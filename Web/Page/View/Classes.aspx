<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Classes.aspx.cs" Inherits="RaaiVan.Web.Page.View.Classes" 
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="classesArea" class="small-12 medium-12 large-12 row" style="margin:0rem 0rem 5rem 0rem; padding:0vw 8vw;"></div>

    <script type="text/javascript">
        (function () {
            var initialJson = JSON.parse(document.getElementById("initialJson").value || "{}");
            
            var nodeTypes = (initialJson || {}).NodeTypes;
            
            GlobalUtilities.loading("classesArea");
            
            GlobalUtilities.load_files(["CN/AllNodesList.js"], {
                OnLoad: function () {
                    var anl = new AllNodesList("classesArea", {
                        InitialNodeTypes: nodeTypes,
                        InitialRelatedItem: initialJson.RelatedItem,
                        ShowAllNodeTypes: true,
                        SortByName: true
                    });

                    GlobalUtilities.onscrollend(document, { Offset: 50 }, function () { anl.more(); });
                }
            });
        })();
    </script>
</asp:Content>
