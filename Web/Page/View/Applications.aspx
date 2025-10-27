<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Applications.aspx.cs" Inherits="RaaiVan.Web.Page.View.Applications"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="appsArea" class="small-12 medium-12 large-12 row" style="margin:0rem 0rem 5rem 0rem; padding:0vw 8vw;"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.loading("appsArea");

            GlobalUtilities.load_files(["USR/ApplicationsManager.js"], {
                OnLoad: function () { new ApplicationsManager("appsArea"); }
            });


            /////////////////////////--> editor test - to be removed <--/////////////////////////
            return;

            GlobalUtilities.load_files([
                "CKEditor5/ckeditor.js",
                "CKEditor5/translations/fa.js",
                "RVEditor/RVEditor.js"
            ], {
                OnLoad: function () {
                    var elems = GlobalUtilities.create_nested_elements([
                        {
                            Type: "div", Style: "padding:0 4vw;",
                            Childs: [{ Type: "div", Name: "editor", Childs: [{ Type: "text", TextValue: "Ramin Yavari" }] }]
                        },
                        {
                            Type: "div", Style: "margin:2rem; display:flex; flex-flow:row; justify-content:center;",
                            Childs: [{
                                Type: "div", Class: "rv-air-button rv-circle", Name: "get",
                                Style: "flex:0 0 auto; padding:0.3rem 2rem;",
                                Childs: [{ Type: "text", TextValue: "Get HTML" }]
                            }]
                        }
                    ], document.getElementById("appsArea").parentNode);

                    var editor = new RVEditor(elems["editor"], {
                        OwnerID: "47df3696-1d59-46d0-865e-49df23f28b08",
                        UploadOwnerType: "Node",
                        OnInit: function (obj) { editor = obj; }
                    });

                    elems["get"].onclick = function () {
                        _alert(editor.get_data());
                    };
                }
            });
            /////////////////////////--> end of editor test - to be removed <--/////////////////////////
        })();
    </script>
</asp:Content>
