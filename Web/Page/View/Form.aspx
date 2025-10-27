<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="RaaiVan.Web.Page.View.Form"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="formArea" class="small-12 medium-12 large-12 row align-center rv-form"></div>

    <script type="text/javascript">
        (function () {
            var initialJson = JSON.parse(document.getElementById("initialJson").value);

            var isPoll = initialJson.IsPoll === true;
            var poll = initialJson.Poll || {};

            var instanceId = initialJson.InstanceID || "";
            var refInstanceId = initialJson.RefInstanceID || "";

            GlobalUtilities.loading(document.getElementById("formArea"));

            GlobalUtilities.load_files(["API/FGAPI.js", "FormsManager/FormViewer.js"], {
                OnLoad: function () {
                    if (!isPoll)
                        new FormViewer("formArea", { InstanceID: instanceId, RefInstanceID: refInstanceId });
                    else {
                        FGAPI.GetPollInstance({
                            CopyFromPollID: poll.IsCopyOfPollID, PollID: poll.PollID,
                            OwnerID: poll.OwnerID, UseExistingPoll: true, ParseResults: true,
                            ResponseHandler: function (result) {
                                if (result.ErrorText) alert(RVDic.MSG[result.ErrorText] || result.ErrorText);
                                else if (result.Succeed) {
                                    instanceId = result.InstanceID;
                                    if (result.Poll) poll = result.Poll;

                                    new FormViewer("formArea", {
                                        InstanceID: instanceId, LimitOwnerID: poll.OwnerID, ShowAllIfNoLimit: true,
                                        PollAbstract: true, Editable: true, FooterSaveButton: true, HideHeader: false,
                                        HideDescription: true, FillButton: false, Poll: poll, IsWorkFlowAdmin: false,
                                        OnInit: function () { this.goto_edit_mode(); },
                                        OnAfterSave: function () { }
                                    });
                                }
                            }
                        });
                    }
                }
            });
        })();
    </script>
</asp:Content>
