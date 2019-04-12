using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace SS.Advertisement.Core
{
    public static class Utils
    {
        public const string PluginId = "SS.Advertisement";

        public static List<int> StringCollectionToIntList(string collection)
        {
            var list = new List<int>();
            if (!string.IsNullOrEmpty(collection))
            {
                var array = collection.Split(',');
                foreach (var s in array)
                {
                    int.TryParse(s.Trim(), out var i);
                    list.Add(i);
                }
            }
            return list;
        }

        public static string MaxLengthText(string inputString, int maxLength, string endString = "...")
        {
            var retVal = inputString;
            try
            {
                if (maxLength > 0)
                {
                    var decodedInputString = HttpUtility.HtmlDecode(retVal);
                    retVal = decodedInputString;

                    var totalLength = maxLength * 2;
                    var length = 0;
                    var builder = new StringBuilder();

                    var isOneBytesChar = false;
                    var lastChar = ' ';

                    if (!string.IsNullOrEmpty(retVal))
                    {
                        foreach (var singleChar in retVal.ToCharArray())
                        {
                            builder.Append(singleChar);

                            if (IsTwoBytesChar(singleChar))
                            {
                                length += 2;
                                if (length >= totalLength)
                                {
                                    lastChar = singleChar;
                                    break;
                                }
                            }
                            else
                            {
                                length += 1;
                                if (length == totalLength)
                                {
                                    isOneBytesChar = true;//已经截取到需要的字数，再多截取一位
                                }
                                else if (length > totalLength)
                                {
                                    lastChar = singleChar;
                                    break;
                                }
                                else
                                {
                                    isOneBytesChar = !isOneBytesChar;
                                }
                            }
                        }
                    }
                    if (isOneBytesChar && length > totalLength)
                    {
                        builder.Length--;
                        var theStr = builder.ToString();
                        retVal = builder.ToString();
                        if (char.IsLetter(lastChar))
                        {
                            for (var i = theStr.Length - 1; i > 0; i--)
                            {
                                var theChar = theStr[i];
                                if (!IsTwoBytesChar(theChar) && char.IsLetter(theChar))
                                {
                                    retVal = retVal.Substring(0, i - 1);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            //int index = retVal.LastIndexOfAny(new char[] { ' ', '\t', '\n', '\v', '\f', '\r', '\x0085' });
                            //if (index != -1)
                            //{
                            //    retVal = retVal.Substring(0, index);
                            //}
                        }
                    }
                    else
                    {
                        retVal = builder.ToString();
                    }

                    var isCut = decodedInputString != retVal;
                    retVal = HttpUtility.HtmlEncode(retVal);

                    if (isCut && endString != null)
                    {
                        retVal += endString;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return retVal;
        }

        private static Encoding Gb2312 { get; } = Encoding.GetEncoding("gb2312");

        private static bool IsTwoBytesChar(char chr)
        {
            // 使用中文支持编码
            return Gb2312.GetByteCount(new[] { chr }) == 2;
        }

        public static bool IsImage(string fileExtName)
        {
            var retVal = false;
            if (string.IsNullOrEmpty(fileExtName)) return false;
            fileExtName = fileExtName.ToLower().Trim();
            if (!fileExtName.StartsWith("."))
            {
                fileExtName = "." + fileExtName;
            }
            if (fileExtName == ".bmp" || fileExtName == ".gif" || fileExtName == ".jpg" || fileExtName == ".jpeg" || fileExtName == ".png" || fileExtName == ".pneg" || fileExtName == ".webp")
            {
                retVal = true;
            }
            return retVal;
        }

        public static string ObjectCollectionToString(ICollection collection)
        {
            var builder = new StringBuilder();
            if (collection == null) return builder.ToString();

            foreach (var obj in collection)
            {
                builder.Append(obj.ToString().Trim()).Append(",");
            }
            if (builder.Length != 0) builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
    }
}
