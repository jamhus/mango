
using Mango.Services.RewardAPI.Messages;

namespace Mango.Services.RewardAPI.Services.Interfaces
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}
