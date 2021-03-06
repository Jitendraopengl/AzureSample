﻿using Microsoft.Azure.ServiceBus;
using System;
using System.Text;

namespace AddMsgInAzTopic
{
    class Program
    {
        static ITopicClient topicClient;
        static void Main(string[] args)
        {
            string sbConnectionString = "Endpoint=sb://mobilerechargeservicebus.servicebus.windows.net/;SharedAccessKeyName=TopicAccessPolicy;SharedAccessKey=2c65KcZEHGlyzrzxgkuf8ku4gfv5FdugVbvP7Hz5yBc=";
            string sbTopic = "rechargetopic";
           
            string messageBody = string.Empty;
            try
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Publish Offer");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Offers");
                Console.WriteLine("1. Recharge with 100 and get talk time of 110");
                Console.WriteLine("2. Get 5 GB data on recharge of 300. Validity 28 days");
                Console.WriteLine("3. 1000 SMS in recharge of 100");
                Console.WriteLine("-------------------------------------------------------");

                Console.WriteLine("Offer:");
                string offer = Console.ReadLine();

                Console.WriteLine("-------------------------------------------------------");

                switch (offer)
                {
                    case "1":
                        offer = "Recharge with 100 and get talk time of 110";
                        break;
                    case "2":
                        offer = "Get 5 GB data on recharge of 300. Validity 28 days";
                        break;
                    case "3":
                        offer = "1000 SMS in recharge of 100";
                        break;
                    default:
                        break;
                }

                messageBody = offer;
                topicClient = new TopicClient(sbConnectionString, sbTopic);

                var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                Console.WriteLine($"Message Published: {messageBody}");

                topicClient.SendAsync(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                topicClient.CloseAsync();
            }
        }
    }
}
