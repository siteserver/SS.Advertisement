using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Advertisement.Core;
using Menu = SiteServer.Plugin.Menu;

namespace SS.Advertisement
{
    public class Plugin : PluginBase
    {
        public override void Startup(IService service)
        {
            var advertisementRepository = new AdvertisementRepository();

            service
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "广告管理",
                    IconClass = "fa fa-mouse-pointer",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "添加广告",
                            Href = "pages/advertisementAdd.html"
                        },
                        new Menu
                        {
                            Text = "广告列表",
                            Href = "pages/advertisement.html"
                        }
                    }
                })
                .AddDatabaseTable(advertisementRepository.TableName, advertisementRepository.TableColumns)
                ;

            service.BeforeStlParse += Service_BeforeStlParse;
        }

        private static void Service_BeforeStlParse(object sender, ParseEventArgs e)
        {
            AdvertisementManager.AddAdvertisements(e);
        }
    }
}