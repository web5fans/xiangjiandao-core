using AlibabaCloud.SDK.Dysmsapi20170525;
using AlibabaCloud.SDK.Dysmsapi20170525.Models;
using AlibabaCloud.TeaUtil.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UUIDNext;
using Xiangjiandao.Web.Application.Dto;
using Xiangjiandao.Web.Options;

namespace Xiangjiandao.Web.Clients;

public interface IAliYunSmsClient
{
    public Task SendSmsAsync(string phone, string code);
}

public class AliYunSmsClient(
    IOptions<AliYunSmsOptions> aliYunSmsOptions,
    Client aliYunSdkClient) : IAliYunSmsClient
{
    public async Task SendSmsAsync(string phone, string code)
    {
        var sendSmsRequest = new SendSmsRequest
        {
            PhoneNumbers = phone,
            SignName = aliYunSmsOptions.Value.SignName,
            TemplateCode = aliYunSmsOptions.Value.TemplateCode,
            TemplateParam = JsonConvert.SerializeObject(new AliyunSmsCodeDto
            {
                Code = code
            }, new JsonSerializerSettings
            {
                // 设置为驼峰命名
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }),
            OutId = Uuid.NewRandom().ToString()
        };
        // 复制代码运行请自行打印 API 的返回值
        await aliYunSdkClient.SendSmsWithOptionsAsync(sendSmsRequest, new RuntimeOptions());
    }
}