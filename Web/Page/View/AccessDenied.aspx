<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="RaaiVan.Web.Page.View.AccessDenied" 
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="contentArea" style="width:100vw; height:calc(100vh - 8rem);"></div>

    <script type="text/javascript">
        (function () {
            GlobalUtilities.create_nested_elements([{
                Type: "div", Style: "display:flex; flex-flow:column; align-items:center; justify-content:center; height:100%;",
                Childs: [
                    {
                        Type: "div", Style: "text-align:center; margin-bottom:1rem;",
                        Childs: [{
                            Type: "img", Style: "max-width:20rem;", Link: RVAPI.HomePageURL(),
                            Attributes: [{ Name: "src", Value: window.RVGlobal.LogoURL }]
                        }]
                    },
                    {
                        Type: "div", Style: "font-size:2rem; margin-bottom:1rem; color:blue;",
                        Childs: [{ Type: "text", TextValue: RVDic.MSG.AccessDenied }]
                    }
                ]
            }], document.getElementById("contentArea"));
        })();
    </script>
</asp:Content>
