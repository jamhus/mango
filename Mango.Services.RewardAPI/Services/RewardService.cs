
using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Messages;
using Mango.Services.RewardAPI.Models;
using Mango.Services.RewardAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _options;

        public RewardService(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            Reward reward = new Reward
            {
                UserId = rewardsMessage.UserId,
                RewardsAmount = rewardsMessage.RewardAmount,
                OrderId = rewardsMessage.OrderId,
                RewardsDate = DateTime.Now
            };

            using (var _db = new AppDbContext(_options))
            {
                await _db.Rewards.AddAsync(reward);
                await _db.SaveChangesAsync();
            }
        }
    }
}
