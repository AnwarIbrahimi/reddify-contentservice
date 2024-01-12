namespace ContentService.RabbitMQ
{
    public interface IRabbitMQHelper
    {
        void CloseConnection();
        void PublishMessage(string message);
    }
}
