<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Posts.aspx.cs" Inherits="RaaiVan.Web.Page.View.Posts"
    MasterPageFile="~/Page/Master/TopMaster.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="initialJson" runat="server" ClientIDMode="Static" Value="" />

    <div id="postArea" class="small-12 medium-12 large-12" style="padding:0 10% 5rem 10%; margin:0 auto;"></div>

    <script type="text/javascript">
        (function () {
            var initialJson = JSON.parse(document.getElementById("initialJson").value);
            var postArea = document.getElementById("postArea");

            var elems = GlobalUtilities.create_nested_elements([
                { Type: "div", Style: "margin-bottom:1.5rem; display:none;", Name: "ownerInfoArea" },
                { Type: "div", Name: "idDivSharingArea" }
            ], postArea);

            var container = elems["idDivSharingArea"];
            var infoArea = elems["ownerInfoArea"];

            var ownerId = initialJson.OwnerID;
            var ownerType = initialJson.OwnerType;
            var postId = initialJson.PostID;

            if (postId || ownerId) jQuery(infoArea).fadeIn(0);

            GlobalUtilities.load_files(["SharingManager/SharingManager.js"], {
                OnLoad: function () {
                    if (ownerId || postId) {
                        new SharingManager({
                            PostsContainerDivID: container, OwnerInfoArea: infoArea, OwnerObjectID: ownerId,
                            InitialFill: true, OwnerType: ownerType, PostID: postId, Permissions: null,
                            RealTime: true, RealTimeFeedID: postId || ownerId
                        });
                    }
                    else {
                        var sm = new SharingManager({
                            PostsContainerDivID: container, OwnerObjectID: (window.RVGlobal || {}).CurrentUserID,
                            InitialFill: true, OwnerType: "User", Permissions: { AddPost: true }, NewPostArea: "Advanced", News: true,
                            OnLoad: function () {
                                GlobalUtilities.onscrollend(document, { Offset: 10 }, function () { sm.get_posts(); });
                            }
                        });
                    }
                }
            });
        })();
    </script>
</asp:Content>
