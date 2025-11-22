using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Messages;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dtos;
using Mango.Services.EmailAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _options;

        public EmailService(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/> Cart email requested ");
            message.AppendLine("<br/> Total:" + cartDto.CartHeader.CartTotal+ "$");
            if(!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
            {
                message.AppendLine("<br/> Coupon applied: " + cartDto.CartHeader.CouponCode);
                message.AppendLine("<br/> Total discounted: " + cartDto.CartHeader.Discount + "$");
            }
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        public async Task EmailRegisteredUser(string userEmail)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/> Welcome to Mango Store! ");
            message.AppendLine("<br/> We are excited to have you on board. ");
            await LogAndEmail(message.ToString(), userEmail);
        }

        public async Task LogOrderCreated(RewardsMessage rewardsMessage)
        {
            string message = "<br/> Order created for user: " + rewardsMessage.UserId +
                " with Order Id: " + rewardsMessage.OrderId +
                " and Reward Amount: " + rewardsMessage.RewardAmount;

            await LogAndEmail(message, "admin@mango.com");
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new EmailLogger()
                {
                    Email = email,
                    Message = message,
                    DateSent = DateTime.Now
                };
                using (var db = new AppDbContext(_options))
                {
                    await db.EmailLoggers.AddAsync(emailLogger);
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
