<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="RaaiVan.Web.Page.View.Search"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="searchArea" class="small-12 medium-12 large-12 row align-center" style="margin:0rem; margin-bottom:5rem; padding:0vw 10vw;"></div>

    <script type="text/javascript">
        (function () {
            var searchText = Base64.decode((JSON.parse(document.getElementById("initialJson").value).SearchText || "").replace(' ', '+'));
            
            GlobalUtilities.load_files(["SearchManager/SearchManager.js"], {
                OnLoad: function () {
                    var sm = new SearchManager("searchArea", {
                        Options: { Modules: (window.RVGlobal || {}).Modules },
                        SearchInput: null, SearchButton: null, InitialSearch: searchText
                    });

                    GlobalUtilities.onscrollend(document, { Offset: 100 }, function () { if (sm) sm.search(); });
                }
            });
        })();
    </script>
</asp:Content>
