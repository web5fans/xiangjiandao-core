using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;
using Xiangjiandao.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 删除Banner
/// </summary>
/// <param name="Id"></param>
public record DeleteBannerCommand(BannerId Id): ICommand<bool>;

/// <summary>
/// 删除Banner
/// </summary>
/// <param name="bannerRepository"></param>
/// <param name="logger"></param>
public class DeleteBannerCommandHandler(
    IBannerRepository bannerRepository,
    ILogger<DeleteBannerCommandHandler> logger) : ICommandHandler<DeleteBannerCommand, bool>
{
    public async Task<bool> Handle(DeleteBannerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("删除Banner Id:{Id}", request.Id);
        var banner = await bannerRepository.GetAsync(request.Id, cancellationToken);
        if (banner is null || banner.Deleted)
        {
            throw new KnownException("横幅不存在");
        }
        
        return banner.Delete();
    }
}