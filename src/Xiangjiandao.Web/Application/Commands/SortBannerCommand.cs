using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;
using Xiangjiandao.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// Banner排序
/// </summary>
/// <param name="BannerIds"></param>
public record SortBannerCommand(List<BannerId> BannerIds): ICommand<bool>;

/// <summary>
/// Banner排序
/// </summary>
/// <param name="bannerRepository"></param>
/// <param name="logger"></param>
public class SortBannerCommandHandler(
    IBannerRepository bannerRepository,
    ILogger<SortBannerCommandHandler> logger) : ICommandHandler<SortBannerCommand, bool>
{
    public async Task<bool> Handle(SortBannerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Banner排序");
        var banners = await bannerRepository.GetByIds(request.BannerIds, cancellationToken);
        var length = request.BannerIds.Count;

        foreach (var bannerId in request.BannerIds)
        {
            var banner = banners.First(x => x.Id == bannerId);
            if (banner.Deleted)
            {
                throw new KnownException("横幅不存在");
            }
            banner.UpdateSort(length);
            length --;
        }

        return true;
    }
}