using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Infra.Extensions
{
    public static class GameExtension
    {
        public static string ToAge(this int age)
        {
            if (age < 10 * 12)
            {
                return "看起来不到十岁。";
            }
            else
            {
                return $"看起来约{(age / 12).NumberToChinese()}多岁。";
            }
        }

        public static string ToGender(this GenderEnum gender)
        {
            switch (gender)
            {
                case GenderEnum.男:
                    return "他";
                    break;
                case GenderEnum.女:
                    return "她";
                    break;
                default:
                    return "它";
                    break;
            }
        }

        public static string ToPer(this int per,int age, GenderEnum gender)
        {
            string str;

            if (gender == GenderEnum.女)
            {
                if (per >= 40) str = "超凡脱俗，娇艳绝伦，貌如西子胜三分！";
                else if (per >= 37)
                {
                    if (age > 22) str = "朱唇不涂一点红，回眸一笑百媚生！";
                    else str = "清丽绝俗，冰清玉洁，有如画中天仙！";
                }
                else if (per >= 33)
                {
                    if (age > 22) str = "容貌丰美，气质高雅，堪称人间仙子！";
                    else str = "容貌娇美，姣花照水，堪称人间仙子！";
                }
                else if (per >= 30)
                {
                    if (age > 22) str = "脸若银盆，眼如水杏，相貌绝美！";
                    else str = "美奂绝伦，一笑倾城，再笑倾国！";
                }
                else if (per >= 27) str = "羞花闭月，宛若天仙！";
                else if (per >= 24) str = "花颜月貌，柔媚娇俏！";
                else if (per >= 20) str = "楚楚动人，有几分姿色！";
                else if (per >= 17) str = "相貌平庸，很是一般。";
                else if (per >= 14) str = "五官挪位，貌似无盐。";
                else str = "一塌糊涂，不是人样！";
            }
            else
            {
                if (per >= 40) str = "英姿勃发，一表人才，称为古往今来第一人！";
                else if (per >= 37) str = "玉树临风，风流倜傥，堪称绝世美男！";
                else if (per >= 33) str = "清秀俊雅，相貌非凡，真是人中龙凤！";
                else if (per >= 30)
                {
                    if (age < 16) str = "貌似美女，脸若冠玉，弱不禁风！";
                    else str = "貌似潘安，容比宋玉，仪表堂堂！";
                }
                else if (per >= 27) str = "相貌出众，堪称美男！";
                else if (per >= 24) str = "英俊潇洒，气质非凡。";
                else if (per >= 20) str = "五官端正，相貌平平。";
                else if (per >= 17) str = "五官不正，满脸麻子。";
                else if (per >= 14) str = "牛眼驴唇，面目狰狞。";
                else str = "有如雷公下凡，八戒返魂！";
            }
            return "长得" + str + "\n";
        }


        public static string ToKunFuLevel(this int exp,int playerExp)
        {
            if (playerExp - exp * 1.5 <= 0)
            {
                return "的武功看不出深浅。";
            }

            var level = (int)Math.Round(Math.Pow(exp, 0.236) / 1.5, MidpointRounding.AwayFromZero);
            var levels = new[] { "不堪一击", "毫不足虑", "不足挂齿", "初学乍练", "勉勉强强", "初窥门径", "初出茅庐", "略知一二", "普普通通", "平平淡淡", "平淡无奇", "粗通皮毛", "半生不熟", "马马虎虎", "略有小成", "已有小成", "鹤立鸡群", "驾轻就熟", "青出于蓝", "融会贯通", "心领神会", "炉火纯青", "了然于胸", "略有大成", "已有大成", "豁然贯通", "出类拔萃", "无可匹敌", "技冠群雄", "神乎其技", "出神入化", "非同凡响", "傲视群雄", "登峰造极", "无与伦比", "所向披靡", "一代宗师", "精深奥妙", "神功盖世", "举世无双", "惊世骇俗", "撼天动地", "震古铄今", "超凡入圣", "威镇寰宇", "空前绝后", "天人合一", "深藏不露", "深不可测" };

            string description;
            if (level <= 0)
            {
                description = "不堪一击";
            }
            else if (level >= levels.Length)
            {
                description = "深不可测";
            }
            else
            {
                description = levels[level - 1];
            }

            return $"的武功看上去{description}。";
        }
    }
}
