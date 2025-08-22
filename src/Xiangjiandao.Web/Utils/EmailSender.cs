using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Xiangjiandao.Web.Utils;

/// <summary>
/// 邮箱配置
/// </summary>
public record EmailOption
{
    /// <summary>
    /// 发送者名称
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// 发送者地址
    /// </summary>
    public string SenderAddress { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱主题
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱服务器域名
    /// </summary>
    public string EmailServerHost { get; set; } = string.Empty;
    
    /// <summary>
    /// 邮箱服务器端口
    /// </summary>
    public int EmailServerPort { get; set; }

    /// <summary>
    /// 邮箱服务器用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱服务器密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱 Tls 配置
    /// </summary>
    public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.StartTls;
}

/// <summary>
/// 邮件发送工具
/// </summary>
public class EmailSender(IOptions<EmailOption> options)
{
    /// <summary>
    /// 给指定邮箱发送邮件
    /// </summary>
    public async Task SendAsync(string content, string toName, string toAddress)
    {
        var message = new MimeMessage();
        var emailOption = options.Value;
        message.From.Add(new MailboxAddress(emailOption.SenderName, emailOption.SenderAddress));
        message.To.Add(new MailboxAddress(toName, toAddress));
        message.Subject = emailOption.Subject;

        message.Body = new TextPart("html")
        {
            Text = content,
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(emailOption.EmailServerHost, emailOption.EmailServerPort, emailOption.SecureSocketOptions);

        await client.AuthenticateAsync(emailOption.UserName, emailOption.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}