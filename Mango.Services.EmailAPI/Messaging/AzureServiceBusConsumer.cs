using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dtos;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        public readonly string ServiceBusConnectionString;
        public readonly string EmailCartQueue;
        public readonly string EmailRegistrationQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registrationProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            EmailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            EmailRegistrationQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailUserRegisteredQueue");

            var client = new ServiceBusClient(ServiceBusConnectionString);

            _emailCartProcessor = client.CreateProcessor(EmailCartQueue);
            _registrationProcessor = client.CreateProcessor(EmailRegistrationQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            if (_registrationProcessor != null)
            {
                _registrationProcessor.ProcessMessageAsync += OnUserRegisteredMessageReceived;
                _registrationProcessor.ProcessErrorAsync += ErrorHandler;
                await _registrationProcessor.StartProcessingAsync();
            }
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            if (_registrationProcessor != null)
            {
                await _registrationProcessor.StopProcessingAsync();
                await _registrationProcessor.DisposeAsync();
            }
        }

        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                await _emailService.EmailCartAndLog(cartDto);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnUserRegisteredMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string userEmail= JsonConvert.DeserializeObject<string>(body);

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                try
                {
                    await _emailService.EmailRegisteredUser(userEmail);
                    await args.CompleteMessageAsync(args.Message);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                try
                {
                    await _emailService.EmailRegisteredUser(userEmail);
                    await args.CompleteMessageAsync(args.Message);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                await args.DeadLetterMessageAsync(args.Message, "InvalidMessage", "Could not extract user email from message body");
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
