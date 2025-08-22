using System.Security.Cryptography;
using System.Text;
using JWT.Builder;

namespace Xiangjiandao.Web.Utils
{
    static class StringUtils
    {
        public static string GetSubFromToken(string token)
        {
            if(string.IsNullOrEmpty(token))
                return string.Empty;
#pragma warning disable S5659
            var payload = JwtBuilder.Create().DoNotVerifySignature().Decode<Dictionary<string, object>>(token);
#pragma warning restore S5659
            return payload["sub"]?.ToString() ?? string.Empty;
        }

        //计算jwt token的md5值
        public static string GetTokenMd5(string token)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(token));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        public static string GenRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return Enumerable.Range(0, length)
                .Aggregate(new StringBuilder(), (sb, _) => sb.Append(chars[Random.Shared.Next(chars.Length)]))
                .ToString(); 
        }
    }
}