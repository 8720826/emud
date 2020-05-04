using Aliyun.OSS;
using Aliyun.OSS.Util;
using Emprise.Admin.Models;
using Emprise.Domain.Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Admin.Helper
{

    public interface IOssHelper
    {
        Task<OssTokenModel> GetToken();
    }
    public class OssHelper: IOssHelper
    {
        private readonly AppConfig _appConfig;

        public OssHelper(IOptionsMonitor<AppConfig> appConfig)
        {
            _appConfig = appConfig.CurrentValue;
        }

        public async Task<OssTokenModel> GetToken()
        {
            var targetDir = $"{ DateTime.Now:yyyyMMdd}/{Guid.NewGuid()}";

            OssClient client = new OssClient(_appConfig.Aliyun.Endpoint, _appConfig.Aliyun.AccessKeyId, _appConfig.Aliyun.AccessKeySecret);
            //密钥过期时间为10分钟
            var expiration = DateTime.Now.AddSeconds(10);
            var policyConds = new PolicyConditions();
            policyConds.AddConditionItem("bucket", _appConfig.Aliyun.BucketName);
            policyConds.AddConditionItem(MatchMode.StartWith, PolicyConditions.CondKey, targetDir);
            //限制传输文件大小10M
            policyConds.AddConditionItem(PolicyConditions.CondContentLengthRange, 1, 10240000);
            var postPolicy = client.GeneratePostPolicy(expiration, policyConds);
            var encPolicy = Convert.ToBase64String(Encoding.UTF8.GetBytes(postPolicy));
            var signature = ComputeSignature(_appConfig.Aliyun.AccessKeySecret, encPolicy);
            var tempRet = new OssTokenModel
            {
                Key = targetDir,
                Bucket = _appConfig.Aliyun.BucketName,
                OSSAccessKeyId = _appConfig.Aliyun.AccessKeyId,
                Policy = encPolicy,
                Signature = signature,
                Endpoint = $"https://{_appConfig.Aliyun.BucketName}.oss-cn-hangzhou.aliyuncs.com"
            };
            return await Task.FromResult(tempRet);
        }

        private string ComputeSignature(string key, string data)
        {
            using (var algorithm = KeyedHashAlgorithm.Create("HmacSHA1".ToUpperInvariant()))
            {
                algorithm.Key = Encoding.UTF8.GetBytes(key.ToCharArray());
                return Convert.ToBase64String(
                    algorithm.ComputeHash(Encoding.UTF8.GetBytes(data.ToCharArray())));
            }
        }
    }

    public class UploadResult
    {
        public bool IsSuccess { get; set; }

        public string Path { get; set; }

        public string ErrorMessage { get; set; }
    }
}
