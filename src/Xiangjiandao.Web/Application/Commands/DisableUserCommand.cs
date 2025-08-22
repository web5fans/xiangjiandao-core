using Microsoft.Extensions.Caching.Memory;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

public record DisableUserCommand(UserId Id) : ICommand<bool>;

public class DisableUserCommandHandler(
    IUserRepository userRepository,
    IMemoryCache memoryCache,
    ILogger<DisableUserCommandHandler> logger
) : ICommandHandler<DisableUserCommand, bool>
{
    public async Task<bool> Handle(DisableUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("禁用用户 Id:{Id}", request.Id);
        var user = await userRepository.GetAsync(request.Id, cancellationToken);
        if (user == null || user.Deleted)
        {
            throw new KnownException("用户不存在");
        }

        user.DisableUser();

        // 删除缓存
        var cacheDidKey = $"userData:{user.Did}";
        var cacheDomainKey = $"userData:{user.DomainName}";
        memoryCache.Remove(cacheDidKey);
        memoryCache.Remove(cacheDomainKey);
        return true;
    }
}