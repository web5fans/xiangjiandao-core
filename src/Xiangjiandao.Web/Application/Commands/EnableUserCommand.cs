using Microsoft.Extensions.Caching.Memory;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

public record EnableUserCommand(UserId Id) : ICommand<bool>;

public class EnableUserCommandHandler(
    IUserRepository userRepository,
    IMemoryCache memoryCache,
    ILogger<EnableUserCommandHandler> logger
) : ICommandHandler<EnableUserCommand, bool>
{
    public async Task<bool> Handle(EnableUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("启用用户 Id:{Id}", request.Id);
        var user = await userRepository.GetAsync(request.Id, cancellationToken);
        if (user == null || user.Deleted)
        {
            throw new KnownException("用户不存在");
        }

        user.EnableUser();

        // 删除缓存
        var cacheDidKey = $"userData:{user.Did}";
        var cacheDomainKey = $"userData:{user.DomainName}";
        memoryCache.Remove(cacheDidKey);
        memoryCache.Remove(cacheDomainKey);
        return true;
    }
}