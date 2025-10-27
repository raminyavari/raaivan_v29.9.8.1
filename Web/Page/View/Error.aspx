<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="RaaiVan.Web.Page.View.Error"
    MasterPageFile="~/Page/Master/MainMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainMasterBodySection" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" />

    <div class="small-12 medium-12 large-12" style="width:100vw; height:100vh;">
        <div style="display:table; width:100%; height:100%;">
            <div id="errorArea" style="display:table-cell; vertical-align:middle; text-align:center;"></div>
        </div>
    </div>

    <script type="text/javascript">
        (function () {
            var initialJson = JSON.parse(document.getElementById("initialJson").value);

            GlobalUtilities.create_nested_elements([
                {
                    Type: "div", Style: "text-align:center; margin-bottom:1rem;",
                    Childs: [{
                        Type: "img", Style: "max-width:20rem;", Link: RVAPI.HomePageURL(),
                        Attributes: [{ Name: "src", Value: GlobalUtilities.icon("RaaiVanLogo.png") }]
                    }]
                },
                {
                    Type: "div", Style: "font-size:2rem; margin-bottom:1rem;",
                    Childs: [{ Type: "text", TextValue: RVDic.AnErrorOccurred }]
                },
                {
                    Type: "div", Style: "font-size:2rem; color:blue;" + (initialJson.Code ? "" : "display:none;"),
                    Childs: [{ Type: "text", TextValue: RVDic.ErrorCode + ": " + initialJson.Code }]
                }
            ], document.getElementById("errorArea"));
        })();
    </script>
</asp:Content>
