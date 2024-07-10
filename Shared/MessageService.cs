using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace GowAcademy.Shared
{
    public class MessageService : IMessageService<ServiceBusMessage, ServiceBusReceivedMessage>
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusAdministrationClient _administrationClient;

        public MessageService(string connectionString)
        {
            _serviceBusClient = new ServiceBusClient(connectionString);
            _administrationClient = new ServiceBusAdministrationClient(connectionString);
        }

        #region Topicos
        public ServiceBusReceivedMessage GetTopicMessage(string topicName, string subscriptionName)
        {
            ServiceBusReceiver receiver = _serviceBusClient.CreateReceiver(topicName, subscriptionName);
            var message = receiver.ReceiveMessageAsync().Result;
            receiver.CompleteMessageAsync(message);
            return message;
        }
        public List<ServiceBusReceivedMessage> GetTopicMessages(int number, string topicName, string subscriptionName)
        {
            try
            {
                CreateSubscriptionForTopic(topicName, subscriptionName);
                ServiceBusReceiver receiver = _serviceBusClient.CreateReceiver(topicName, subscriptionName);
                var messages = receiver.PeekMessagesAsync(number).Result;
                return messages.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void PostTopicMessage(ServiceBusMessage message, string topicName)
        {
            try
            {
                if(!TopicExist(topicName))
                    CreateTopic(topicName);
                ServiceBusSender sender = _serviceBusClient.CreateSender(topicName);
                sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool SubscriptionExists(string topicName, string subscriptionName)
        {
            return _administrationClient.SubscriptionExistsAsync(topicName, subscriptionName).Result;
        }
        public void CreateSubscriptionForTopic(string topicName, string subscriptionName)
        {
            if (!TopicExist(topicName))
                CreateTopic(topicName);
            if (!SubscriptionExists(topicName, subscriptionName))
            {
                try
                {
                    var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName)
                    {
                        AutoDeleteOnIdle = TimeSpan.FromDays(7),
                        DefaultMessageTimeToLive = TimeSpan.FromDays(2),
                        EnableBatchedOperations = true,
                    };

                    var createdSubscription = _administrationClient.CreateSubscriptionAsync(subscriptionOptions);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public bool TopicExist(string topicName)
        {
            return _administrationClient.TopicExistsAsync(topicName).Result;
        }
        public void CreateTopic(string topicName)
        {
            if (!TopicExist(topicName))
            {
                try
                {
                    var topicOptions = new CreateTopicOptions(topicName)
                    {
                        AutoDeleteOnIdle = TimeSpan.FromDays(7),
                        DefaultMessageTimeToLive = TimeSpan.FromDays(2),
                        EnableBatchedOperations = true,
                    };

                    var createdTopic = _administrationClient.CreateTopicAsync(topicOptions);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        #endregion

        #region Filas

        public async Task<ServiceBusReceivedMessage> GetQueueMessage(string queueName)
        {
            if (QueueExist(queueName))
            {
                ServiceBusReceiver receiver = _serviceBusClient.CreateReceiver(queueName);
                var message = await receiver.ReceiveMessageAsync();
                await receiver.CompleteMessageAsync(message);
                return message;
            }
            return null;
        }

        public List<ServiceBusReceivedMessage> GetQueueMessages(int number, string queueName)
        {
            try
            {
                ServiceBusReceiver receiver = _serviceBusClient.CreateReceiver(queueName);
                var messages = receiver.PeekMessagesAsync(number).Result;
                return messages.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void PostQueueMessage(ServiceBusMessage message, string queueName)
        {
            try
            {
                if(!QueueExist(queueName))
                    CreateQueue(queueName);
                ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);
                sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool QueueExist(string queueName)
        {
            return _administrationClient.QueueExistsAsync(queueName).Result;
        }

        public void CreateQueue(string queueName)
        {
            if (!QueueExist(queueName))
            {
                try
                {
                    var queueOptions = new CreateQueueOptions(queueName)
                    {
                        AutoDeleteOnIdle = TimeSpan.FromDays(7),
                        DefaultMessageTimeToLive = TimeSpan.FromDays(2),
                        EnableBatchedOperations = true,
                    };
                    var createdQueue = _administrationClient.CreateQueueAsync(queueOptions).Result;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        #endregion
    }
}