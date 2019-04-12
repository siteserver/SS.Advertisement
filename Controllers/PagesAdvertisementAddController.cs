using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Advertisement.Core;

namespace SS.Advertisement.Controllers
{
    [RoutePrefix("pages/advertisementAdd")]
    public class PagesAdvertisementAddController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                var advertisementId = request.GetQueryInt("advertisementId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId))
                {
                    return Unauthorized();
                }

                var advertisementInfo = advertisementId > 0
                    ? AdvertisementManager.Repository.Get(advertisementId)
                    : new AdvertisementInfo
                    {
                        AdvertisementType = EAdvertisementTypeUtils.GetValue(EAdvertisementType.FloatImage),
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(1),
                        RollingType = ERollingTypeUtils.GetValue(ERollingType.FollowingScreen),
                        PositionType = EPositionTypeUtils.GetValue(EPositionType.LeftTop),
                        PositionX = 10,
                        PositionY = 120,
                        IsCloseable = true
                    };

                var advertisementTypes = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(EAdvertisementTypeUtils.GetValue(EAdvertisementType.FloatImage),
                        EAdvertisementTypeUtils.GetText(EAdvertisementType.FloatImage)),
                    new KeyValuePair<string, string>(EAdvertisementTypeUtils.GetValue(EAdvertisementType.ScreenDown),
                        EAdvertisementTypeUtils.GetText(EAdvertisementType.ScreenDown)),
                    new KeyValuePair<string, string>(EAdvertisementTypeUtils.GetValue(EAdvertisementType.OpenWindow),
                        EAdvertisementTypeUtils.GetText(EAdvertisementType.OpenWindow))
                };

                var channels = new List<KeyValuePair<int, string>>();
                var channelIdList = Context.ChannelApi.GetChannelIdList(siteId);
                var isLastNodeArray = new bool[channelIdList.Count];
                foreach (var theChannelId in channelIdList)
                {
                    var channelInfo = Context.ChannelApi.GetChannelInfo(siteId, theChannelId);

                    var title = GetChannelListBoxTitle(siteId, channelInfo.Id, channelInfo.ChannelName, channelInfo.ParentsCount, channelInfo.LastNode, isLastNodeArray);
                    channels.Add(new KeyValuePair<int, string>(channelInfo.Id, title));
                }

                var positionTypes = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(EPositionTypeUtils.GetValue(EPositionType.LeftTop),
                        EPositionTypeUtils.GetText(EPositionType.LeftTop)),
                    new KeyValuePair<string, string>(EPositionTypeUtils.GetValue(EPositionType.LeftBottom),
                        EPositionTypeUtils.GetText(EPositionType.LeftBottom)),
                    new KeyValuePair<string, string>(EPositionTypeUtils.GetValue(EPositionType.RightTop),
                        EPositionTypeUtils.GetText(EPositionType.RightTop)),
                    new KeyValuePair<string, string>(EPositionTypeUtils.GetValue(EPositionType.RightBottom),
                        EPositionTypeUtils.GetText(EPositionType.RightBottom))
                };

                var rollingTypes = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(ERollingTypeUtils.GetValue(ERollingType.FollowingScreen),
                        ERollingTypeUtils.GetText(ERollingType.FollowingScreen)),
                    new KeyValuePair<string, string>(ERollingTypeUtils.GetValue(ERollingType.Static),
                        ERollingTypeUtils.GetText(ERollingType.Static)),
                    new KeyValuePair<string, string>(ERollingTypeUtils.GetValue(ERollingType.FloatingInWindow),
                        ERollingTypeUtils.GetText(ERollingType.FloatingInWindow))
                };

                var adminToken = Context.AdminApi.GetAccessToken(request.AdminId, request.AdminName, TimeSpan.FromDays(1));

                return Ok(new
                {
                    Value = advertisementInfo,
                    AdvertisementTypes = advertisementTypes,
                    Channels = channels,
                    PositionTypes = positionTypes,
                    RollingTypes = rollingTypes,
                    AdminToken = adminToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static string GetChannelListBoxTitle(int siteId, int channelId, string nodeName, int parentsCount, bool isLastNode, IList<bool> isLastNodeArray)
        {
            var str = string.Empty;
            if (channelId == siteId)
            {
                isLastNode = true;
            }
            if (isLastNode == false)
            {
                isLastNodeArray[parentsCount] = false;
            }
            else
            {
                isLastNodeArray[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, isLastNodeArray[i] ? "　" : "│");
            }

            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, Utils.MaxLengthText(nodeName, 8));

            return str;
        }

        [HttpPost, Route(RouteActionsUpload)]
        public IHttpActionResult Upload()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId))
                {
                    return Unauthorized();
                }

                var imageUrl = string.Empty;
                var width = 0;
                var height = 0;

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var filePath = Context.SiteApi.GetUploadFilePath(siteId, postFile.FileName);

                    if (!Utils.IsImage(Path.GetExtension(filePath)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    imageUrl = Context.SiteApi.GetSiteUrlByFilePath(filePath);

                    var image = Image.FromFile(filePath);
                    width = image.Width;
                    height = image.Height;
                }

                return Ok(new
                {
                    Value = imageUrl,
                    Width = width,
                    Height = height
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId))
                {
                    return Unauthorized();
                }

                var adInfo = request.GetPostObject<AdvertisementInfo>();

                adInfo.ChannelIdCollectionToChannel =
                    Utils.ObjectCollectionToString(
                        request.GetPostObject<List<int>>("channelIdListToChannel"));
                adInfo.ChannelIdCollectionToContent =
                    Utils.ObjectCollectionToString(
                        request.GetPostObject<List<int>>("channelIdListToContent"));

                if (adInfo.Id > 0)
                {
                    AdvertisementManager.Repository.Update(adInfo);
                }
                else
                {
                    if (AdvertisementManager.Repository.IsExists(adInfo.AdvertisementName, siteId))
                    {
                        return BadRequest("保存失败，已存在相同名称的广告！");
                    }

                    adInfo.SiteId = siteId;
                    adInfo.AddDate = DateTime.Now;
                    AdvertisementManager.Repository.Insert(adInfo);
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
