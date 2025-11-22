namespace Mango.Web.Models
{
    public class StripeRequestDto
    {
        public string? StripeSesstionUrl { get; set; }
        public string? StripeSesstionId { get; set; }
        public string? ApprovedUrl { get; set; }
        public string? CancelUrl { get; set; }
        public OrderHeaderDto? OrderHeader { get; set; }
    }
}
