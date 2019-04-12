using System.Text;
using SiteServer.Plugin;

namespace SS.Advertisement.Core
{
    public class ScriptFloating
    {
        private readonly AdvertisementInfo _adInfo;

        public ScriptFloating(AdvertisementInfo adInfo)
        {
            _adInfo = adInfo;
        }

        public string GetScript()
        {
            var closeUrl = Context.PluginApi.GetPluginUrl(Utils.PluginId, "assets/close.gif");
            var linkUrl = Context.SiteApi.GetSiteUrl(_adInfo.SiteId,
                _adInfo.NavigationUrl);
            var imageUrl = Context.SiteApi.GetSiteUrl(_adInfo.SiteId, _adInfo.ImageUrl);
            var width = _adInfo.Width == 0 ? 160 : _adInfo.Width;
            var height = _adInfo.Height == 0 ? 600 : _adInfo.Height;

            var closeDiv = _adInfo.IsCloseable
                ? $@"
<div class=""ads-float-close"" style=""width: {width}px; height: 18px; position: absolute; left: 0px; top: {height}px; background: url(&quot;{closeUrl}&quot;) right center no-repeat rgb(235, 235, 235); cursor: pointer;"" onclick=""document.getElementById('ad_{_adInfo.Id}').style.display = 'none';""></div>
"
                : string.Empty;

            var position = "left: 0px; top: 65px;";
            if (EPositionTypeUtils.Equals(_adInfo.PositionType, EPositionType.LeftTop))
            {
                position = $"left: {_adInfo.PositionX}px; top: {_adInfo.PositionY}px;";
            }
            else if (EPositionTypeUtils.Equals(_adInfo.PositionType, EPositionType.LeftBottom))
            {
                position = $"left: {_adInfo.PositionX}px; bottom: {_adInfo.PositionY}px;";
            }
            else if (EPositionTypeUtils.Equals(_adInfo.PositionType, EPositionType.RightTop))
            {
                position = $"right: {_adInfo.PositionX}px; top: {_adInfo.PositionY}px;";
            }
            else if (EPositionTypeUtils.Equals(_adInfo.PositionType, EPositionType.RightBottom))
            {
                position = $"right: {_adInfo.PositionX}px; bottom: {_adInfo.PositionY}px;";
            }

            var builder = new StringBuilder($@"
<div id=""ad_{_adInfo.Id}"" class=""ads-float ads-float-left"" style=""position: fixed; width: {width}px; height: {height}px; z-index: 10500; display: block; {position}""><div style=""width: {width}px; height: {height}px; position: absolute; left: 0px; top: 0px;""><a href=""{linkUrl}"" target=""_blank""><img src=""{imageUrl}"" width=""100%"" height=""100%"" border=""0""></a></div>{closeDiv}</div>
");

            var type = 1;
            if (ERollingTypeUtils.Equals(_adInfo.RollingType, ERollingType.FloatingInWindow))
            {
                type = 1;
            }
            else if (ERollingTypeUtils.Equals(_adInfo.RollingType, ERollingType.FollowingScreen))
            {
                type = 2;
            }
            else if (ERollingTypeUtils.Equals(_adInfo.RollingType, ERollingType.Static))
            {
                type = 3;
            }

            var positionX = string.Empty;
            var positionY = string.Empty;
            if (EPositionTypeUtils.Equals(_adInfo.PositionType, EPositionType.LeftTop))
            {
                positionX = _adInfo.PositionX.ToString();
                positionY = _adInfo.PositionY.ToString();
            }
            else if (EPositionTypeUtils.Equals(_adInfo.PositionType, EPositionType.LeftBottom))
            {
                positionX = _adInfo.PositionX.ToString();
                positionY =
                    $@"document.body.scrollTop+document.body.offsetHeight-{_adInfo.PositionY}-{_adInfo
                        .Height}";
            }
            else if (EPositionTypeUtils.Equals(_adInfo.PositionType, EPositionType.RightTop))
            {
                positionX =
                    $@"document.body.scrollLeft+document.body.offsetWidth-{_adInfo.PositionX}-{_adInfo
                        .Width}";
                positionY = _adInfo.PositionY.ToString();
            }
            else if (EPositionTypeUtils.Equals(_adInfo.PositionType, EPositionType.RightBottom))
            {
                positionX =
                    $@"document.body.scrollLeft+document.body.offsetWidth-{_adInfo.PositionX}-{_adInfo
                        .Width}";
                positionY =
                    $@"document.body.scrollTop+document.body.offsetHeight-{_adInfo.PositionY}-{_adInfo
                        .Height}";
            }

            var dateLimited = string.Empty;
            if (_adInfo.IsDateLimited && _adInfo.StartDate != null && _adInfo.EndDate != null)
            {
                dateLimited = $@"
    var sDate{_adInfo.Id} = new Date({_adInfo.StartDate.Value.Year}, {_adInfo.StartDate.Value.Month - 1}, {_adInfo.StartDate.Value.Day}, {_adInfo
                    .StartDate.Value.Hour}, {_adInfo.StartDate.Value.Minute});
    var eDate{_adInfo.Id} = new Date({_adInfo.EndDate.Value.Year}, {_adInfo.EndDate.Value.Month - 1}, {_adInfo.EndDate.Value.Day}, {_adInfo.EndDate.Value.Hour}, {_adInfo.EndDate.Value.Minute});
    ad{_adInfo.Id}.SetDate(sDate{_adInfo.Id}, eDate{_adInfo.Id});
";
            }

            builder.Append($@"
<script type=""text/javascript"">
    var ad{_adInfo.Id}=new Ad_Move(""ad_{_adInfo.Id}"");
    ad{_adInfo.Id}.SetLocation({positionX}, {positionY});
    ad{_adInfo.Id}.SetType({type});
    {dateLimited}
    ad{_adInfo.Id}.Run();
</script>
");

            return builder.ToString();
        }
    }
}
