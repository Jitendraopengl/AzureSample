using System;

namespace ReadMessageFromSubscriptionTopic
{
    class Program
    {
        static ISubscriptionClient subscriptionClient;
        static void Main(string[] args)
        {
            string sbConnectionString = "Endpoint=sb://mobilerechargeservicebus.servicebus.windows.net/;SharedAccessKeyName=TopicAccessPolicy;SharedAccessKey=2c65KcZEHGlyzrzxgkuf8ku4gfv5FdugVbvP7Hz5yBc=";
            string sbTopic = "rechargetopic";
            string sbSubscription = "jitu007";
            try
            {
                subscriptionClient = new SubscriptionClient(sbConnectionString, sbTopic, sbSubscription);

                var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                };
                subscriptionClient.RegisterMessageHandler(ReceiveMessagesAsync, messageHandlerOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                subscriptionClient.CloseAsync();
            }
        }

        static async Task ReceiveMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Subscribed message: {Encoding.UTF8.GetString(message.Body)}");

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine(exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }
    }
}
