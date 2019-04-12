using System;
using Datory;

namespace SS.Advertisement.Core
{
    [Table("ss_Advertisement")]
    public class AdvertisementInfo : Entity
    {
        [TableColumn]
        public string AdvertisementName { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string AdvertisementType { get; set; }

        [TableColumn]
        public bool IsDateLimited { get; set; }

        [TableColumn]
        public DateTime? StartDate { get; set; }

        [TableColumn]
        public DateTime? EndDate { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }

        [TableColumn(Text = true)]
        public string ChannelIdCollectionToChannel { get; set; }

        [TableColumn(Text = true)]
        public string ChannelIdCollectionToContent { get; set; }

        [TableColumn(Text = true, Extend = true)]
        private string Settings { get; set; }

        public bool IsCloseable { get; set; }

        public string PositionType { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public string RollingType { get; set; }

        public string NavigationUrl { get; set; }

        public string ImageUrl { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int Delay { get; set; }
    }
}
