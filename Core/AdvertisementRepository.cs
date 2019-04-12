using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;

namespace SS.Advertisement.Core
{
    public class AdvertisementRepository : Repository<AdvertisementInfo>
    {
        public AdvertisementRepository(): base(Context.Environment.Database) { }

        private static class Attr
        {
            public const string Id = nameof(AdvertisementInfo.Id);
            public const string AdvertisementName = nameof(AdvertisementInfo.AdvertisementName);
            public const string SiteId = nameof(AdvertisementInfo.SiteId);
            public const string AdvertisementType = nameof(AdvertisementInfo.AdvertisementType);
        }

        public override int Insert(AdvertisementInfo adInfo)
        {
            adInfo.Id = base.Insert(adInfo);
            AdvertisementManager.RemoveCache(adInfo.SiteId);
            return adInfo.Id;
        }

        public override bool Update(AdvertisementInfo adInfo)
        {
            var updated = base.Update(adInfo);
            AdvertisementManager.RemoveCache(adInfo.SiteId);
            return updated;
        }

        public void Delete(int siteId, int advertisementId)
        {
            base.Delete(advertisementId);
            AdvertisementManager.RemoveCache(siteId);
        }

        public bool IsExists(string advertisementName, int siteId)
        {
            return Exists(Q.Where(Attr.AdvertisementName, advertisementName).Where(Attr.SiteId, siteId));
        }

        public IList<AdvertisementInfo> GetAdvertisementInfoList(int siteId)
        {
            return GetAll(Q.Where(Attr.SiteId, siteId).OrderByDesc(Attr.Id));
        }

        public IList<AdvertisementInfo> GetAdvertisementInfoList(int siteId, EAdvertisementType advertisementType)
        {
            return GetAll(Q.Where(Attr.SiteId, siteId).Where(Attr.AdvertisementType, EAdvertisementTypeUtils.GetValue(advertisementType)).OrderByDesc(Attr.Id));
        }

        public List<int>[] GetAdvertisementLists(int siteId)
        {
            var advertisementInfoList = GetAdvertisementInfoList(siteId);
            //var sqlString =
            //    $"SELECT ChannelIdCollectionToChannel, ChannelIdCollectionToContent, FileTemplateIdCollection FROM siteserver_Advertisement WHERE SiteId = {siteId}";

            var list1 = new List<int>();
            var list2 = new List<int>();

            foreach (var advertisementInfo in advertisementInfoList)
            {
                var collection1 = advertisementInfo.ChannelIdCollectionToChannel;
                var collection2 = advertisementInfo.ChannelIdCollectionToContent;

                if (!string.IsNullOrEmpty(collection1))
                {
                    var list = Utils.StringCollectionToIntList(collection1);
                    foreach (var id in list)
                    {
                        if (!list1.Contains(id))
                        {
                            list1.Add(id);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(collection2))
                {
                    var list = Utils.StringCollectionToIntList(collection2);
                    foreach (var id in list)
                    {
                        if (!list2.Contains(id))
                        {
                            list2.Add(id);
                        }
                    }
                }
            }

            return new[] { list1, list2 };
        }
    }
}
