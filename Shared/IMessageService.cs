namespace GowAcademy.Shared
{
    public interface IMessageService<T, Q>
    {
        public void PostTopicMessage(T message, string topicName);
        public Q GetTopicMessage(string topicName, string subscriptionName);
        public List<Q> GetTopicMessages(int number, string topicName, string subscriptionName);
        public void PostQueueMessage(T message, string queueName);
        public Task<Q> GetQueueMessage(string queueName);
        public List<Q> GetQueueMessages(int number, string queueName);
        bool TopicExist(string topicName);
        void CreateSubscriptionForTopic(string topicName, string subscriptionName);
        bool SubscriptionExists(string topicName, string subscriptionName);
    }
}