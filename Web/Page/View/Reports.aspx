<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="RaaiVan.Web.Page.View.Reports"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <link rel="stylesheet" href="../../Script/Kendo/kendo.common.css" type="text/css" />
    <link rel="stylesheet" href="../../Script/Kendo/kendo.blueopal.css" type="text/css" />
    <script type="text/javascript" src="../../Script/Kendo/kendo.core.js"></script>
    <script type="text/javascript" src="../../Script/Kendo/kendo.data.js"></script>
    <script type="text/javascript" src="../../Script/Kendo/kendo.reorderable.js"></script>
    <script type="text/javascript" src="../../Script/Kendo/kendo.resizable.js"></script>
    <script type="text/javascript" src="../../Script/Kendo/kendo.selectable.js"></script>
    <script type="text/javascript" src="../../Script/Kendo/kendo.sortable.js"></script>
    <script type="text/javascript" src="../../Script/Kendo/kendo.pager.js"></script>
    <script type="text/javascript" src="../../Script/Kendo/kendo.userevents.js"></script>
    <script type="text/javascript" src="../../Script/Kendo/kendo.draganddrop.js"></script>
    <script type="text/javascript" src="../../Script/Kendo/kendo.grid.js"></script>

    <div id="reportsArea" class="small-12 medium-12 large-12" style="margin:0rem; padding:0vw 6vw; margin-bottom:5rem;"></div>

    <script type="text/javascript">
        (function () {
            var reports = JSON.parse(document.getElementById("initialJson").value).Reports || {};
            var container = document.getElementById("reportsArea");

            var modules = (window.RVGlobal || {}).Modules || {};
            modules.KW = modules.KW || modules.DCT;

            var elems = GlobalUtilities.create_nested_elements([
                { Type: "div", Class: "small-10 medium-8 large-6", Name: "serverSelector", Style: "margin:0 auto 1rem auto; display:none;" },
                { Type: "div", Class: "small-12 medium-12 large-12 row align-center", Name: "reportsContainer", Style: "margin:0;" }
            ], container);

            var loadArr = [
                { Root: "Kendo/", Childs: ["kendo.common.css", "kendo.blueopal.css"] },
                { Root: "Kendo/kendo.", Ext: "js", Childs: ["core", "data"] },
                "API/ReportsAPI.js",
                "GridManager/GridManager.js",
                "Reports/ReportsManager.js",
                "RemoteServers/RemoteServerSelector.js"
            ];

            GlobalUtilities.load_files(loadArr, {
                OnLoad: function () {
                    var arr = [{
                        Root: "Kendo/kendo.", Ext: "js",
                        Childs: ["resizable", "selectable", "reorderable", "sortable", "pager", "userevents", "draganddrop", "grid"]
                    }];

                    GlobalUtilities.load_files(arr, {
                        OnLoad: function () {
                            new RemoteServerSelector(elems["serverSelector"], { AutoHide: true });
                            new ReportsManager(elems["reportsContainer"], { Reports: reports, Modules: modules });
                        }
                    });
                }
            });
        })();
    </script>
</asp:Content>
