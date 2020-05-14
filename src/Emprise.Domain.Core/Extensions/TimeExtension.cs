using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Extensions
{
    public static class TimeExtension
    {

        /// <summary>
        /// 获取时间差
        /// </summary>
        /// <param name="dtOld">要减的时间</param>
        /// <param name="dtNow">大的时间</param>
        /// <returns></returns>
        public static string ToFriendlyTime(this DateTime time)
        {
            TimeSpan tsOld = new TimeSpan(time.Ticks);
            TimeSpan tsNow = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan datediff = tsOld.Subtract(tsNow).Duration();
            if (Convert.ToInt32(datediff.Days) <= 0)
            {
                if (Convert.ToInt32(datediff.Hours) <= 0)
                {
                    if (Convert.ToInt32(datediff.Minutes) > 0)
                    {
                        return datediff.Minutes.ToString() + "分钟前";
                    }
                    else
                    {
                        return "刚刚";
                    }
                }
                else
                {
                    return datediff.Hours.ToString() + "小时前";
                }
            }
            else
            {
                if (Convert.ToInt32(datediff.Days) < 365)
                {
                    return datediff.Days.ToString() + "天前";
                }
                else
                {
                    int year = Convert.ToInt32(datediff.Days) / 365;
                    return year.ToString() + "年前";
                }
            }

        }
    }
}
