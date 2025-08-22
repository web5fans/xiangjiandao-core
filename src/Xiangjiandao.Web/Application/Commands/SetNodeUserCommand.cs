using Microsoft.Extensions.Caching.Memory;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

public record SetNodeUserCommand(UserId Id): ICommand<bool>;

public class SetNodeUserCommandHandler(
    IUserRepository userRepository,
    IMemoryCache memoryCache,
    ILogger<SetNodeUserCommandHandler> logger
): ICommandHandler<SetNodeUserCommand, bool>
{
    public async Task<bool> Handle(SetNodeUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("设置节点用户 Id:{Id}", request.Id);
        var user = await userRepository.GetAsync(request.Id, cancellationToken);
        if (user == null || user.Deleted)
        {
            throw new KnownException("用户不存在");
        }

        if (user.Disable)
        {
            throw new KnownException("用户已被禁用，请先启用用户");
        }

        user.SetNodeUser();
        // 删除缓存
        var cacheDidKey = $"userData:{user.Did}";
        var cacheDomainKey = $"userData:{user.DomainName}";
        memoryCache.Remove(cacheDidKey);
        memoryCache.Remove(cacheDomainKey);
        return true;
    }
}