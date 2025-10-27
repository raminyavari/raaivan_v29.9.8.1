<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="RaaiVan.Web.Page.View.ChangePassword" 
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="contentArea" class="small-10 medium-8 large-6 rv-border-radius-1 SoftBackgroundColor" style="padding:1rem; margin:5rem auto;"></div>

    <script type="text/javascript">
        (function () {
            var settings = GlobalUtilities.to_json(document.getElementById("initialJson").value) || {};

            GlobalUtilities.load_files(["USR/ChangePasswordDialog.js"], {
                OnLoad: function () {
                    new ChangePasswordDialog("contentArea", GlobalUtilities.extend(settings || {}, {
                        OnPasswordChange: function () { window.location.href = RVAPI.HomePageURL(); }
                    }));
                }
            });
        })();
    </script>
</asp:Content>