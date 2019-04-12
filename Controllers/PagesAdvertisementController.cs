using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Advertisement.Core;

namespace SS.Advertisement.Controllers
{
    [RoutePrefix("pages/advertisement")]
    public class PagesAdvertisementController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetList()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                var advertisementType = request.GetQueryString("advertisementType");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId))
                {
                    return Unauthorized();
                }

                var types = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(string.Empty, "<所有类型>"),
                    new KeyValuePair<string, string>(EAdvertisementTypeUtils.GetValue(EAdvertisementType.FloatImage),
                        EAdvertisementTypeUtils.GetText(EAdvertisementType.FloatImage)),
                    new KeyValuePair<string, string>(EAdvertisementTypeUtils.GetValue(EAdvertisementType.ScreenDown),
                        EAdvertisementTypeUtils.GetText(EAdvertisementType.ScreenDown)),
                    new KeyValuePair<string, string>(EAdvertisementTypeUtils.GetValue(EAdvertisementType.OpenWindow),
                        EAdvertisementTypeUtils.GetText(EAdvertisementType.OpenWindow))
                };

                var advertisementInfoList = string.IsNullOrEmpty(advertisementType)
                    ? AdvertisementManager.Repository.GetAdvertisementInfoList(siteId)
                    : AdvertisementManager.Repository.GetAdvertisementInfoList(siteId,
                        EAdvertisementTypeUtils.GetEnumType(advertisementType));

                foreach (var advertisementInfo in advertisementInfoList)
                {
                    advertisementInfo.Set("display", GetDisplay(siteId, advertisementInfo));
                    advertisementInfo.Set("type", EAdvertisementTypeUtils.GetText(EAdvertisementTypeUtils.GetEnumType(advertisementInfo.AdvertisementType)));
                }

                return Ok(new
                {
                    Value = advertisementInfoList,
                    Types = types
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private string GetDisplay(int siteId, AdvertisementInfo adInfo)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(adInfo.ChannelIdCollectionToChannel))
            {
                builder.Append("栏目：");
                var channelIdArrayList = Utils.StringCollectionToIntList(adInfo.ChannelIdCollectionToChannel);
                foreach (var channelId in channelIdArrayList)
                {
                    builder.Append(Context.ChannelApi.GetChannelName(siteId, channelId));
                    builder.Append(",");
                }
                builder.Length--;
            }
            if (!string.IsNullOrEmpty(adInfo.ChannelIdCollectionToContent))
            {
                if (builder.Length > 0)
                {
                    builder.Append(" | ");
                }
                builder.Append("内容：");
                var channelIdList = Utils.StringCollectionToIntList(adInfo.ChannelIdCollectionToContent);
                foreach (var channelId in channelIdList)
                {
                    builder.Append(Context.ChannelApi.GetChannelName(siteId, channelId));
                    builder.Append(",");
                }
                builder.Length--;
            }
            return builder.ToString();
        }

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                var advertisementId = request.GetQueryInt("advertisementId");
                var advertisementType = request.GetQueryString("advertisementType");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId))
                {
                    return Unauthorized();
                }

                AdvertisementManager.Repository.Delete(siteId, advertisementId);

                var advertisementInfoList = string.IsNullOrEmpty(advertisementType)
                    ? AdvertisementManager.Repository.GetAdvertisementInfoList(siteId)
                    : AdvertisementManager.Repository.GetAdvertisementInfoList(siteId,
                        EAdvertisementTypeUtils.GetEnumType(advertisementType));

                foreach (var advertisementInfo in advertisementInfoList)
                {
                    advertisementInfo.Set("display", GetDisplay(siteId, advertisementInfo));
                    advertisementInfo.Set("type", EAdvertisementTypeUtils.GetText(EAdvertisementTypeUtils.GetEnumType(advertisementInfo.AdvertisementType)));
                }

                return Ok(new
                {
                    Value = advertisementInfoList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
