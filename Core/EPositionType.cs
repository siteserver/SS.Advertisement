using System;

namespace SS.Advertisement.Core
{
    public enum EPositionType
	{
		LeftTop,				//左上
		LeftBottom,				//左下
		RightTop,               //右上
        RightBottom             //右下
	}

    public static class EPositionTypeUtils
	{
	    public static string GetValue(EPositionType type)
		{
		    if (type == EPositionType.LeftTop)
			{
                return "LeftTop";
			}
		    if (type == EPositionType.LeftBottom)
		    {
		        return "LeftBottom";
		    }
		    if (type == EPositionType.RightTop)
		    {
		        return "RightTop";
		    }
		    if (type == EPositionType.RightBottom)
		    {
		        return "RightBottom";
		    }
		    throw new Exception();
		}

		public static string GetText(EPositionType type)
		{
		    if (type == EPositionType.LeftTop)
            {
                return "左上";
            }
		    if (type == EPositionType.LeftBottom)
		    {
		        return "左下";
		    }
		    if (type == EPositionType.RightTop)
		    {
		        return "右上";
		    }
		    if (type == EPositionType.RightBottom)
		    {
		        return "右下";
		    }
		    throw new Exception();
		}

	    private static bool Equals(EPositionType type, string typeStr)
	    {
	        return !string.IsNullOrEmpty(typeStr) && string.Equals(GetValue(type).ToLower(), typeStr.ToLower());
	    }

        public static bool Equals(string typeStr, EPositionType type)
        {
            return Equals(type, typeStr);
        }
	}
}
