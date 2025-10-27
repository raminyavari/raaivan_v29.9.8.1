<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RaaiVan.Web.Page.View.Login"
    MasterPageFile="~/Page/Master/MainMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainMasterBodySection" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" />

    <div id="contentArea"></div>

    <script type="text/javascript">
        (function () {
            if (GlobalUtilities.browser_version().Name == "msie")
                alert("با این مرورگر، بخشی از قابلیت های «رای ون » را از دست خواهید داد! اگر تجربه کاربری بهتری را می خواهید، لطفا آدرس این صفحه را در یکی از مرورگرهای زیر بارگذاری فرمایید:\nFirefox 10+, Chrome 10+, Opera 21+", { Original: true });

            var initialJson = GlobalUtilities.to_json(document.getElementById("initialJson").value) || {};

            document.title = RVDic.Login + " - " + document.title;

            GlobalUtilities.load_files(["USR/LoginPageInitializer.js"], {
                OnLoad: function () { new LoginPageInitializer("contentArea", initialJson); }
            });

            if (initialJson.DBConnectionError)
                alert(RVDic.MSG[initialJson.DBConnectionError] || initialJson.DBConnectionError, { Timeout: 20000 });
        })();
    </script>
</asp:Content>
