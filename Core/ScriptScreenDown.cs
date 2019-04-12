using SiteServer.Plugin;

namespace SS.Advertisement.Core
{
	public class ScriptScreenDown
	{
	    private readonly AdvertisementInfo _adInfo;

        public ScriptScreenDown(AdvertisementInfo adInfo)
		{
		    _adInfo = adInfo;
        }

	    public string GetScript()
	    {
	        var sizeString = _adInfo.Width > 0
	            ? $"width={_adInfo.Width} "
	            : string.Empty;
	        sizeString += _adInfo.Height > 0 ? $"height={_adInfo.Height}" : string.Empty;

	        return $@"
<script language=""javascript"" type=""text/javascript"">
function ad_changediv(){{
    jQuery('#ad_hiddenLayer_{_adInfo.Id}').slideDown();
    setTimeout(""ad_hidediv()"",{_adInfo.Delay}000);
}}
function ad_hidediv(){{
    jQuery('#ad_hiddenLayer_{_adInfo.Id}').slideUp();
}}
jQuery(document).ready(function(){{
    jQuery('body').prepend('<div id=""ad_hiddenLayer_{_adInfo.Id}"" style=""display: none;""><center><a href=""{Context.SiteApi.GetSiteUrl(_adInfo.SiteId,
	            _adInfo.NavigationUrl)}"" target=""_blank""><img src=""{Context.SiteApi.GetSiteUrl(_adInfo.SiteId, _adInfo.ImageUrl)}"" {sizeString} border=""0"" /></a></center></div>');
    setTimeout(""ad_changediv()"",2000);
}});
</script>
";
	    }
	}
}
