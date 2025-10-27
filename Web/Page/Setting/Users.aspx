<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.Users"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="usersArea" class="small-12 medium-12 large-12 row align-center" 
        style="margin-bottom:5rem; padding:0 10vw 0 10vw;">
    </div>

    <script type="text/javascript">
        (function () {
            var initialJson = JSON.parse(document.getElementById("initialJson").value || "{}");

            var _do = function () {
                GlobalUtilities.loading(document.getElementById("usersArea"));
                GlobalUtilities.load_files(["USR/UsersManager.js"], { OnLoad: function () { new UsersManager("usersArea"); } });
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
