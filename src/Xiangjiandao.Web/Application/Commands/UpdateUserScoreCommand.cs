using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 更新用户稻米
/// </summary>
/// <param name="Id"></param>
/// <param name="Score"></param>
public record UpdateUserScoreCommand(UserId Id, long Score) : ICommand<bool>;

/// <summary>
/// 更新用户稻米处理
/// </summary>
/// <param name="userRepository"></param>
/// <param name="logger"></param>
public class UpdateUserScoreCommandHandler(
    IUserRepository userRepository,
    ILogger<UpdateUserScoreCommandHandler> logger) : ICommandHandler<UpdateUserScoreCommand, bool>
{ 
    public async Task<bool> Handle(UpdateUserScoreCommand command, CancellationToken cancellationToken)
    { 
        logger.LogInformation("更新用户稻米");
        var user = await userRepository.GetAsync(command.Id, cancellationToken);
        if (user == null || user.Deleted)
        {
            throw new KnownException("用户不存在");
        }

        if (user.Score + command.Score < 0)
        {
            throw new KnownException("稻米不足");
        }
        user.UpdateScore(command.Score);
        return true;
    }
}