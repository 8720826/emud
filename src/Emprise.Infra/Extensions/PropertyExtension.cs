using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Emprise.Infra.Extensions
{
    public static class PropertyExtension
    {
        /// <summary>
        /// 比较实体差异
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source">修改前</param>
        /// <param name="current">修改后</param>
        /// <returns></returns>
        public static string ComparisonTo<T1, T2>(this T1 source, T2 current)
        {
            string diff = "";
            Type t1 = source.GetType();
            Type t2 = current.GetType();
            PropertyInfo[] property2 = t2.GetProperties();
            //排除主键和基础字段
            List<string> exclude = new List<string>() { "Id" };
            foreach (PropertyInfo p in property2)
            {
                string name = p.Name;
                if (exclude.Contains(name)) { continue; }
                var value1 = t1.GetProperty(name)?.GetValue(source, null)?.ToString();
                var value2 = p.GetValue(current, null)?.ToString();
                if (value1 != value2)
                {
                    diff += $"[{name}]:'{value1}'=>'{value2}';\r\n";
                }
            }
            return diff;
        }
    }
}
