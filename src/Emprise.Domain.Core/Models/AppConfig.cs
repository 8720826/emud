using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Models
{
    public class AppConfig
    {
        public EmailConfig Email { get; set; }

        public ConnectionStringConfig ConnectionStrings { get; set; }

        public AliyunConfig Aliyun { get; set; }

        public SiteConfig Site { get; set; }
        
    }


    public class EmailConfig
    {
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string AccountName { get; set; }

        public string Password { get; set; }

        public string FromAlias { get; set; }
    }

    public class AliyunConfig
    {
        public string RegionId { get; set; }

        public string AccessKeyId { get; set; }

        public string Secret { get; set; }

    }

    public class ConnectionStringConfig
    {
        public string Redis { get; set; }

        public string Mysql { get; set; }

        public string MsSql { get; set; }
    }

    public class SiteConfig
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string BeiAn { get; set; }

        public string WelcomeWords { get; set; }

        public int BornRoomId { get; set; }
        

        /// <summary>
        /// 是否需要填写邮箱
        /// </summary>
        public bool IsNeedEmail { get; set; }

        /// <summary>
        /// 是否需要验证邮箱，开启后需要配置邮件服务
        /// </summary>
        public bool IsNeedVerifyEmail { get; set; }

        public bool IsApiEnable { get; set; }

        public string ApiKey { get; set; }



    }
}
