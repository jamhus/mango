namespace Mango.Services.OrderAPI.Models.Dtos
{
    public class RewardDto
    {
        public string UserId { get; set; }
        public int RewardAmount { get; set; }
        public int OrderId { get; set; }
    }
}
