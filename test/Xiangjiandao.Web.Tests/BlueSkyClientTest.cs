using Newtonsoft.Json;
using Refit;
using Xiangjiandao.Web.Clients;

namespace Xiangjiandao.Web.Tests;

public class BlueSkyClientTest
{
    /// <summary>
    /// 测试域名
    /// </summary>
    private const string TestDomain = "https://web5.rivtower.cc";

    /// <summary>
    /// Admin 账户 Token
    /// </summary>
    private const string AdminToken = "Basic YWRtaW46ZjRkZjNkNGQyNzlmYTViODA0NjhmY2RmMDM1YzkyZDc=";

    /// <summary>
    /// Mod 用户标识符
    /// </summary>
    private const string ModUserIdentifier = "mod.web5.rivtower.cc";

    /// <summary>
    /// Mod 用户密码
    /// </summary>
    private const string ModUserPassword = "123";

    /// <summary>
    /// Refit 客户端实例
    /// </summary>
    private static IBlueSkyClient Client => RestService.For<IBlueSkyClient>(TestDomain, new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer()
        }
    );

    /// <summary>
    /// 创建账户
    /// </summary>
    [Fact]
    public async Task TestCreateAccount()
    {
        var req = new CreateAccountReq
        {
            Email = "zhushuliang@rivtower.top",
            Password = "12345678",
            Handle = "yunus4.web5.rivtower.cc",
            InviteCode = ""
        };
        try
        {
            var resp = await Client.CreateAccount(req);
        }
        catch (ApiException ex)
        {
            var exContent = ex.Content;
        }
    }

    /// <summary>
    /// 登录获取 Token 单元测试
    /// </summary>
    [Fact]
    public async Task TestCreateSession()
    {
        var req = new CreateSessionReq
        {
            Identifier = "cctoto.web5.rivtower.cc",
            Password = "12345678",
            AuthFactorToken = "",
            AllowTakendown = true
        };
        try
        {
            var session = await Client.CreateSession(req);
        }
        catch (ApiException ex)
        {
            var exContent = ex.Content;
        }
    }

    /// <summary>
    /// 更新用户密码
    /// </summary>
    [Fact]
    public async Task TestUpdateAccountPassword()
    {
        var req = new UpdateAccountPasswordReq
        {
            Did = "did:plc:23hkqlap3466vny6fqc3v577",
            Password = "12345678",
        };
        try
        {
            await Client.UpdateAccountPassword(req, AdminToken);
        }
        catch (ApiException ex)
        {
            var exContent = ex.Content;
        }
    }

    /// <summary>
    /// 删除账户
    /// </summary>
    [Fact]
    public async Task TestDeleteAccount()
    {
        var req = new DeleteAccountReq
        {
            Did = "did:plc:npy3rpuyeirmo4scpunrzosn"
        };
        try
        {
            await Client.DeleteAccount(req, AdminToken);
        }
        catch (ApiException ex)
        {
            var exContent = ex.Content;
        }
    }

    /// <summary>
    /// 测试 EmitEvent
    /// </summary>
    [Fact]
    public async Task TestEmitEvent()
    {
        const string uri = "at://did:plc:ltq57f74fub2mtqf4bpzt45f/app.bsky.feed.post/3lrwe6ljpdc2y";
        const string cid = "bafyreibywkysf4i3ygtbnhxbthooqz773cgteqrcobm4kfqwgtpsdszh3e";

        // 先创建 Mod 用户 Session
        var createSessionReq = new CreateSessionReq
        {
            Identifier = ModUserIdentifier,
            Password = ModUserPassword,
            AuthFactorToken = "",
            AllowTakendown = true
        };
        CreateSessionResp session;
        try
        {
            session = await Client.CreateSession(createSessionReq);
        }
        catch (ApiException ex)
        {
            var exContent = ex.Content;
            return;
        }

        var emitEventReq = new EmitEventReq
        {
            Subject = new SubjectModel
            {
                Type = SubjectModeType.StrongRef,
                Uri = uri,
                Cid = cid,
            },
            CreatedBy = session.Did,
            Event = new EventModel
            {
                Type = EventModelType.ModEventLabel,
                CreateLabelVals = ["blacklist"]
            }
        };

        var serializeObject = JsonConvert.SerializeObject(emitEventReq);

        try
        {
            var emitEventResp = await Client.EmitEvent(
                req: emitEventReq,
                authorization: "Bearer " + session.AccessJwt,
                atprotoProxy: session.Did + "#atproto_labeler",
                atprotoAcceptLabelers: session.Did
            );
        }
        catch (ApiException ex)
        {
            var exContent = ex.Content;
        }
    }
}