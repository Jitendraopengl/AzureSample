using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadMsgFromAzQueue
{
    class Program
    {
        static QueueClient queueClient;
        static void Main(string[] args)
        {
            string sbConnectionString = "Endpoint=sb://mobilerechargeservicebus.servicebus.windows.net/;SharedAccessKeyName=RechargeAccesspolicy;SharedAccessKey=mxx7P4Pb9UtFt0pqu1kU7etj0gskrzft5fy2szcp6M0=";

            string sbQueueName = "rechargemq";
           
            string messageBody = string.Empty;
            try
            {
                queueClient = new QueueClient(sbConnectionString, sbQueueName);
                var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                };
                queueClient.RegisterMessageHandler(ReceiveMessagesAsync, messageHandlerOptions);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                queueClient.CloseAsync();
            }


        }
        static async Task ReceiveMessagesAsync(Message message, CancellationToken token)
        {
            //Console.WriteLine($"Received message: {Encoding.UTF8.GetString(message.Body)}");

            //await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            try
            {
                Console.WriteLine($"Received message: {Encoding.UTF8.GetString(message.Body)}");

                int i = 0;
                i = i / Convert.ToInt32(message);

                await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await queueClient.AbandonAsync(message.SystemProperties.LockToken);
            }
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine(exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }
    }
       
}
