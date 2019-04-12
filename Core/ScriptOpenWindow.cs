using SiteServer.Plugin;

namespace SS.Advertisement.Core
{
	public class ScriptOpenWindow
	{
	    private readonly AdvertisementInfo _adInfo;

        public ScriptOpenWindow(AdvertisementInfo adInfo)
		{
            _adInfo = adInfo;
		}

		public string GetScript()
		{
            var sizeString = _adInfo.Width > 0 ? $",width={_adInfo.Width}"
                : string.Empty;
            sizeString += _adInfo.Height > 0 ? $",height={_adInfo.Height} " : string.Empty;

            return $@"
<script language=""javascript"" type=""text/javascript"">
function ad_open_win_{_adInfo.Id}() {{
	var popUpWin{_adInfo.Id} = open(""{Context.SiteApi.GetSiteUrl(_adInfo.SiteId, _adInfo.NavigationUrl)}"", (window.name!=""popUpWin{_adInfo.Id}"")?""popUpWin{_adInfo.Id}"":"""", ""toolbar=no,location=no,directories=no,resizable=no,copyhistory=yes{sizeString}"");
}}
try{{
	setTimeout(""ad_open_win_{_adInfo.Id}();"",50);
}}catch(e){{}}
</script>
";
		}
	}
}
