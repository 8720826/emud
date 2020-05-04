using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Emprise.Domain.Core.Models
{
    public class AppConfig
    {
        public EmailConfig Email { get; set; }


        public AliyunConfig Aliyun { get; set; }

        public SiteConfig Site { get; set; }
        
    }


    public class EmailConfig
    {
        [DisplayName("SMTP服务地址")]
        public string SmtpServer { get; set; }

        [DisplayName("SMTP端口")]
        public int SmtpPort{ get; set; }

        [DisplayName("SMTP密码")]
        public string Password { get; set; }

        [DisplayName("邮件发信帐号")]
        public string AccountName { get; set; }


        [DisplayName("邮件发信人")]
        public string FromAlias { get; set; }
    }

    public class AliyunConfig
    {
        [DisplayName("阿里云Endpoint")]
        public string Endpoint { get; set; }

        [DisplayName("阿里云RegionId")]
        public string RegionId { get; set; }

        [DisplayName("阿里云BucketName")]
        public string BucketName { get; set; }

        [DisplayName("阿里云AccessKeyId")]
        public string AccessKeyId { get; set; }

        [DisplayName("阿里云AccessKeySecret")]
        public string AccessKeySecret { get; set; }

    }

    public class SiteConfig 
    {
        [DisplayName("站点名")]
        public string Name { get; set; }

        [DisplayName("站点网址")]
        public string Url { get; set; }

        [DisplayName("备案号")]
        public string BeiAn { get; set; }

        [DisplayName("游戏欢迎词")]
        public string WelcomeWords { get; set; }

        /// <summary>
        /// 出生地房间id，该id必须存在
        /// </summary>
        [DisplayName("出生房间Id")]
        public int BornRoomId { get; set; }
        

        /// <summary>
        /// 是否开启远程api，开启后，后台管理操作将更新游戏缓存。当不需要使用后台时，建议关闭
        /// </summary>
        [DisplayName("是否开启远程api")]
        public bool IsApiEnable { get; set; }

        /// <summary>
        /// 远程api验证密钥，请妥善保管，切勿泄漏！！！
        /// </summary>
        [DisplayName("远程api验证密钥")]
        public string ApiKey { get; set; }



    }
}
