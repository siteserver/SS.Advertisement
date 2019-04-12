using System;
using System.Web.UI.WebControls;

namespace SS.Advertisement.Core
{
    public enum EAdvertisementType
    {
        FloatImage,				//漂浮广告
        ScreenDown,             //全屏下推
        OpenWindow,             //弹出窗口
    }

    public static class EAdvertisementTypeUtils
    {
        public static string GetValue(EAdvertisementType type)
        {
            switch (type)
            {
                case EAdvertisementType.ScreenDown:
                    return "ScreenDown";
                case EAdvertisementType.OpenWindow:
                    return "OpenWindow";
                default:
                    return "FloatImage";
            }
        }

        public static string GetText(EAdvertisementType type)
        {
            switch (type)
            {
                case EAdvertisementType.ScreenDown:
                    return "全屏下推";
                case EAdvertisementType.OpenWindow:
                    return "弹出窗口";
                default:
                    return "漂浮广告";
            }
        }

        public static EAdvertisementType GetEnumType(string typeStr)
        {
            var retVal = EAdvertisementType.FloatImage;

            if (Equals(EAdvertisementType.FloatImage, typeStr))
            {
                retVal = EAdvertisementType.FloatImage;
            }
            else if (Equals(EAdvertisementType.ScreenDown, typeStr))
            {
                retVal = EAdvertisementType.ScreenDown;
            }
            else if (Equals(EAdvertisementType.OpenWindow, typeStr))
            {
                retVal = EAdvertisementType.OpenWindow;
            }

            return retVal;
        }

        private static bool Equals(EAdvertisementType type, string typeStr)
        {
            return !string.IsNullOrEmpty(typeStr) && string.Equals(GetValue(type).ToLower(), typeStr.ToLower());
        }

        public static bool Equals(string typeStr, EAdvertisementType type)
        {
            return Equals(type, typeStr);
        }
    }
}
