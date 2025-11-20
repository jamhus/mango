
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://mangoweb-jh.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vbtclaSNq+B8iserMVRa7TXSf3kFEyrnb+ASbKdYjAo=";
        public async Task PublishMessage(object message, string topic_queue_name)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topic_queue_name);
            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage busMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };
            await sender.SendMessageAsync(busMessage);
            await sender.DisposeAsync();
        }
    }
}
