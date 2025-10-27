<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemSettings.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.SystemSettings"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="settingsArea" class="small-12 medium-12 large-12 row align-center rv-form"></div>

    <script type="text/javascript">
        (function () {
            var initialJson = JSON.parse(document.getElementById("initialJson").value || "{}");

            var _do = function () {
                GlobalUtilities.loading("settingsArea");

                GlobalUtilities.load_files(["RVAdmin/SystemSettings.js"], {
                    OnLoad: function () {
                        new SystemSettings("settingsArea", { Modules: (window.RVGlobal || {}).Modules });
                    }
                });
            };

            if (!initialJson.Reauthenticate) return _do();

            var processing = false;

            var promptObj = GlobalUtilities.prompt(RVDic.Checks.PleaseEnterYourPassword, {
                Placeholder: RVDic.Password, Center: true, Stick: true,
                HideCancelButton: true, DoNotClose: true, IsPassword: true
            }, function (value) {
                value = value === false ? value : GlobalUtilities.trim(value);

                if (processing || !value) return;
                processing = true;

                UsersAPI.ValidatePassword({
                    Password: Base64.encode(value), ParseResults: true,
                    ResponseHandler: function (result) {
                        if ((result || {}).Result === true) {
                            promptObj.Close();
                            _do();
                        }
                        else alert(RVDic.MSG.PasswordIsNotValid, null, function () { promptObj.Clear(); });

                        processing = false;
                    }
                });
            });
        })();
    </script>
</asp:Content>