using System;
using System.Collections.Generic;
using SiteServer.Plugin;

namespace SS.Advertisement.Core
{
    public static class AdvertisementManager
    {
        public static AdvertisementRepository Repository => new AdvertisementRepository();

        private static bool IsAdvertisementExists(ParseEventArgs args)
        {
            var lists = GetAdvertisementLists(args.SiteId);

            if (args.TemplateType == TemplateType.IndexPageTemplate || args.TemplateType == TemplateType.ChannelTemplate)
            {
                var list = lists[0];
                return list.Contains(args.ChannelId);
            }

            if (args.TemplateType == TemplateType.ContentTemplate)
            {
                var list = lists[1];
                return list.Contains(args.ContentId);
            }

            return false;
        }

        private static List<int>[] GetAdvertisementLists(int siteId)
        {
            var cacheKey = GetCacheKey(siteId);
            lock (LockObject)
            {
                if (CacheUtils.Get<List<int>[]>(cacheKey) == null)
                {
                    var lists = Repository.GetAdvertisementLists(siteId);
                    CacheUtils.InsertMinutes(cacheKey, lists, 30);
                    return lists;
                }
                return CacheUtils.Get<List<int>[]>(cacheKey);
            }
        }

        public static void RemoveCache(int siteId)
        {
            var cacheKey = GetCacheKey(siteId);
            CacheUtils.Remove(cacheKey);
        }

        private static string GetCacheKey(int siteId)
        {
            return CacheKeyPrefix + siteId;
        }

        private static readonly object LockObject = new object();
        private const string CacheKeyPrefix = "SS.Advertisement.Core.Advertisement.";

        public static void AddAdvertisements(ParseEventArgs args)
        {
            if (!IsAdvertisementExists(args)) return;

            var advertisementInfoList = Repository.GetAdvertisementInfoList(args.SiteId);

            foreach (var adInfo in advertisementInfoList)
            {
                if (adInfo.IsDateLimited)
                {
                    if (DateTime.Now < adInfo.StartDate || DateTime.Now > adInfo.EndDate)
                    {
                        continue;
                    }
                }
                var isToDo = false;
                if (args.TemplateType == TemplateType.IndexPageTemplate || args.TemplateType == TemplateType.ChannelTemplate)
                {
                    if (!string.IsNullOrEmpty(adInfo.ChannelIdCollectionToChannel))
                    {
                        var nodeIdArrayList = Utils.StringCollectionToIntList(adInfo.ChannelIdCollectionToChannel);
                        if (nodeIdArrayList.Contains(args.ChannelId))
                        {
                            isToDo = true;
                        }
                    }
                }
                else if (args.TemplateType == TemplateType.ContentTemplate)
                {
                    if (!string.IsNullOrEmpty(adInfo.ChannelIdCollectionToContent))
                    {
                        var nodeIdArrayList = Utils.StringCollectionToIntList(adInfo.ChannelIdCollectionToContent);
                        if (nodeIdArrayList.Contains(args.ContentId))
                        {
                            isToDo = true;
                        }
                    }
                }

                if (!isToDo) continue;

                var scripts = string.Empty;
                if (EAdvertisementTypeUtils.Equals(adInfo.AdvertisementType, EAdvertisementType.FloatImage))
                {
                    args.HeadCodes["JsAdFloating"] =
                        $@"<script type=""text/javascript"" src=""{Context.PluginApi.GetPluginUrl(Utils.PluginId, "assets/adFloating.js")}""></script>";

                    var floatScript = new ScriptFloating(adInfo);
                    scripts = floatScript.GetScript();
                }
                else if (EAdvertisementTypeUtils.Equals(adInfo.AdvertisementType, EAdvertisementType.ScreenDown))
                {
                    if (!args.HeadCodes.ContainsKey("Jquery"))
                    {
                        args.HeadCodes["Jquery"] =
                            $@"<script type=""text/javascript"" src=""{Context.PluginApi.GetPluginUrl(Utils.PluginId, "assets/jquery-1.9.1.min.js")}""></script>";
                    }

                    var screenDownScript = new ScriptScreenDown(adInfo);
                    scripts = screenDownScript.GetScript();
                }
                else if (EAdvertisementTypeUtils.Equals(adInfo.AdvertisementType, EAdvertisementType.OpenWindow))
                {
                    var openWindowScript = new ScriptOpenWindow(adInfo);
                    scripts = openWindowScript.GetScript();
                }

                args.BodyCodes[adInfo.AdvertisementType + "_" + adInfo.AdvertisementName] = scripts;
            }
        }
    }
}
