<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QAWorkflow.aspx.cs" Inherits="RaaiVan.Web.Page.Setting.QAWorkflow"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div class="small-12 medium-12 large-12 row align-center" style="margin:0rem; padding:0vw 4vw;">
        <div id="tabs" class="small-12 medium-12 large-12" style="margin-bottom:1rem;"></div>
        <div id="wfArea" class="small-12 medium-12 large-12 row" style="margin:0rem;"></div>
        <div id="faqArea" class="small-12 medium-12 large-12 row" style="margin:0rem;"></div>
    </div>

    <script type="text/javascript">
        (function () {
            var tabsArea = document.getElementById("tabs");
            var wfArea = document.getElementById("wfArea");
            var faqArea = document.getElementById("faqArea");

            var tabs = [];

            var wfInited = false;
            var faqInited = false;

            tabs.push({
                Page: wfArea, Title: RVDic.WorkFlows, FixedPage: true,
                OnActive: function () {
                    if (wfInited) return;
                    wfInited = true;

                    GlobalUtilities.loading(wfArea);

                    GlobalUtilities.load_files(["QA/QAWorkFlows.js"], {
                        OnLoad: function () { new QAWorkFlows(wfArea); }
                    });
                }
            });

            tabs.push({
                Page: faqArea, Title: "FAQ", FixedPage: true,
                OnActive: function () {
                    if (faqInited) return;
                    faqInited = true;

                    GlobalUtilities.loading(faqArea);

                    GlobalUtilities.load_files(["QA/FAQAdmin.js"], {
                        OnLoad: function () { new FAQAdmin(faqArea); }
                    });
                }
            });

            GlobalUtilities.load_files(["TabsManager/TabsManager.js"], {
                OnLoad: function () {
                    (new TabsManager({ ContainerDiv: tabsArea, Pages: tabs })).goto_page(0);
                }
            });
        })();
    </script>
</asp:Content>
