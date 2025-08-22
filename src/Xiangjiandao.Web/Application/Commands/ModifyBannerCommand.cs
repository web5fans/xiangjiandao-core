using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;
using Xiangjiandao.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改Banner
/// </summary>
/// <param name="Id"></param>
/// <param name="BannerFileId"></param>
/// <param name="LinkAddress"></param>
public record ModifyBannerCommand(BannerId Id, string BannerFileId, string LinkAddress) : ICommand<bool>;

/// <summary>
/// 修改Banner
/// </summary>
/// <param name="bannerRepository"></param>
/// <param name="logger"></param>
public class ModifyBannerCommandHandler(
    IBannerRepository bannerRepository,
    ILogger<ModifyBannerCommandHandler> logger) : ICommandHandler<ModifyBannerCommand, bool>
{
    public async Task<bool> Handle(ModifyBannerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("修改Banner Id:{Id}, BannerFileId:{BannerFileId}, LinkAddress:{LinkAddress}", request.Id, request.BannerFileId, request.LinkAddress);
        var banner = await bannerRepository.GetAsync(request.Id, cancellationToken);
        if (banner is null || banner.Deleted)
        {
            throw new KnownException("横幅不存在");
        }

        banner.Modify(request.BannerFileId, request.LinkAddress);

        return true;
    }
}