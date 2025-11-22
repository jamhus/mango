namespace Mango.Services.RewardAPI.Messages
{
    public class RewardsMessage
    {
        public string UserId { get; set; }
        public int RewardAmount { get; set; }
        public int OrderId { get; set; }
    }
}
