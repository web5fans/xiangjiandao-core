using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;
using Xiangjiandao.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 创建Banner
/// </summary>
public record CreateBannerCommand(): ICommand<BannerId>
{
    /// <summary>
    /// Banner 文件 Id
    /// </summary> 
    public string BannerFileId { get;  set; } = string.Empty;

    /// <summary>
    /// 链接地址
    /// </summary> 
    public string LinkAddress { get;  set; } = string.Empty;
}

/// <summary>
/// 创建Banner
/// </summary>
/// <param name="bannerRepository"></param>
/// <param name="logger"></param>
public class CreateBannerCommandHandler(
    IBannerRepository bannerRepository,
    ILogger<CreateBannerCommandHandler> logger) : ICommandHandler<CreateBannerCommand, BannerId>
{
    public async Task<BannerId> Handle(CreateBannerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateBannerCommand Handing ");
        var maxSort = await bannerRepository.LatestOrder(cancellationToken);
        var banner = Banner.Create(
            request.BannerFileId,
            request.LinkAddress,
            maxSort + 1
        );
        await bannerRepository.AddAsync(banner, cancellationToken);
        return banner.Id;
    }
}