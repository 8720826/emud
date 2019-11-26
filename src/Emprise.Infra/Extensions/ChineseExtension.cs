using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Infra.Extensions
{
    public static class ChineseExtension
    {
        public static string NumberToChinese(this int num)
        {
            var len = num.ToString().Length;

            var dw2 = new List<string> { "", "万", "亿" };//大單位
            var dw1 = new List<string> { "十", "百", "千" };//小單位
            var dw = new List<string> { "", "一", "二", "三", "四", "五", "六", "七", "八", "九" };//整數部分用
            var k1 = 0;//计小單位
            var k2 = 0;//计大單位
            var str = "";

            for (var i = 1; i <= len; i++)
            {
                var n = num.ToString().CharAt(len - i);
                if (n == "0")
                {
                    if (k1 != 0)
                        str = str.Substring(1, str.Length - 1);
                }
                str = "{0}{1}".FormatTo(dw[n.ConverToInt()], str);//加數字
                if (len - i - 1 >= 0)//在數字范围内
                {
                    if (k1 != 3)//加小單位
                    {
                        str = "{0}{1}".FormatTo(dw1[k1], str);//加數字

                        k1++;
                    }
                    else//不加小單位，加大單位
                    {
                        k1 = 0;
                        var temp = str.CharAt(0);
                        if (temp == "万" || temp == "亿")//若大單位前没有數字则舍去大單位
                            str = str.Substring(1, str.Length - 1);
                        str = "{0}{1}".FormatTo(dw2[k2], str);
                    }
                }
                if (k1 == 3)//小單位到千则大單位进一
                {
                    k2++;
                }

            }
            if (str.Length >= 2)
            {
                if (str.Substring(0, 2) == "一十") str = str.Substring(1, str.Length - 1);
            }
            return str;
        }
        public static int ConverToInt(this string str)
        {
            int.TryParse(str, out int num);
            return num;
        }

        public static string FormatTo(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
        public static string CharAt(this string s, int index)
        {
            if ((index >= s.Length) || (index < 0))
                return "";
            return s.Substring(index, 1);
        }
    }
}
