using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Messages;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string ServiceBusConnectionString;
        private readonly string? OrderCreatedTopic;
        private readonly string? OrderCreatedRewardsSubscription;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;
        private ServiceBusProcessor? _RewardProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            _rewardService = rewardService;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            OrderCreatedTopic = _configuration.GetValue<string>("TopicsAndQueueNames:OrderCreatedTopic");
            OrderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicsAndQueueNames:OrderCreatedRewardsSubscription");
            var client = new ServiceBusClient(ServiceBusConnectionString);
            _RewardProcessor = client.CreateProcessor(OrderCreatedTopic, OrderCreatedRewardsSubscription);
        }

        public Task Start()
        {
            _RewardProcessor!.ProcessMessageAsync += OnOrderCreatedMessageReceived;
            _RewardProcessor.ProcessErrorAsync += ErrorHandler;
            return _RewardProcessor.StartProcessingAsync();
        }

        private async Task OnOrderCreatedMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardsMessage reward = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                await _rewardService.UpdateRewards(reward);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task Stop()
        {

            return _RewardProcessor!.StopProcessingAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
