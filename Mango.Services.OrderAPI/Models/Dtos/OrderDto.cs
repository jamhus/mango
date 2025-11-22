namespace Mango.Services.OrderAPI.Models.Dtos
{
    public class OrderDto
    {
        public OrderHeaderDto? OrderHeader { get; set; }
        public IEnumerable<OrderDetailsDto>? OrderDetails { get; set; }
    }
}
