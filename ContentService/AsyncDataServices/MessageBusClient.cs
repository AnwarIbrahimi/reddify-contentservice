using ContentService.DTO;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace ContentService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration["RabbitMQ:Url"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare the exchange
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                // Declare the queue
                _channel.QueueDeclare(queue: "contents_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                // Bind the queue to the exchange
                _channel.QueueBind(queue: "contents_queue", exchange: "trigger", routingKey: "");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
                throw;
            }
        }


        public void PublishNewContent(ContentPublishedDTO contentPublishedDTO)
        {
            var message = JsonSerializer.Serialize(contentPublishedDTO);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connectionis closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger",
                            routingKey: "contents_queue",
                            basicProperties: null,
                            body: body);
            Console.WriteLine($"--> We have sent {message}");

        }

        private void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
}
