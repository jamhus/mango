using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostApplicationLifetime.ApplicationStarted.Register(() => ServiceBusConsumer.Start());
            hostApplicationLifetime.ApplicationStopping.Register(() => ServiceBusConsumer.Stop());
            
            return app;
        }
    }
}
