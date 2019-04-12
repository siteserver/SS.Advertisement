using System;

namespace SS.Advertisement.Core
{
	public enum ERollingType
	{
		Static,							//静止不动
		FollowingScreen,				//跟随窗体滚动
		FloatingInWindow				//在窗体中不断移动
	}

	public static class ERollingTypeUtils
	{
		public static string GetValue(ERollingType type)
		{
		    if (type == ERollingType.Static)
			{
				return "Static";
			}
		    if (type == ERollingType.FollowingScreen)
		    {
		        return "FollowingScreen";
		    }
		    if (type == ERollingType.FloatingInWindow)
		    {
		        return "FloatingInWindow";
		    }
		    throw new Exception();
		}

		public static string GetText(ERollingType type)
		{
		    if (type == ERollingType.Static)
			{
				return "静止不动";
			}
		    if (type == ERollingType.FollowingScreen)
		    {
		        return "跟随窗体滚动";
		    }
		    if (type == ERollingType.FloatingInWindow)
		    {
		        return "在窗体中不断移动";
		    }
		    throw new Exception();
		}

	    private static bool Equals(ERollingType type, string typeStr)
		{
		    return !string.IsNullOrEmpty(typeStr) && string.Equals(GetValue(type).ToLower(), typeStr.ToLower());
		}

        public static bool Equals(string typeStr, ERollingType type)
        {
            return Equals(type, typeStr);
        }
	}
}
