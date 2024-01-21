using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using ContentService.Models;
using ContentService.Data;
using Newtonsoft.Json;

namespace ContentService.RabbitMQ
{
    public class RabbitMQListener
    {
        private readonly IContentRepo _contentRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly IModel _channel;
        private readonly IConnection _connection;

        public RabbitMQListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public RabbitMQListener(IServiceProvider serviceProvider, IConnection connection, IModel channel)
        {
            _serviceProvider = serviceProvider;
            _connection = connection;
            _channel = channel;
        }

        public RabbitMQListener(IConnection connection, IModel channel, IContentRepo contentRepo)
        {
            _connection = connection;
            _channel = channel;
            _contentRepo = contentRepo;
        }

        public RabbitMQListener()
        {
        }

        private IContentRepo GetcontentRepo()
        {
            using var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IContentRepo>();
        }

        public void StartListening(IConfiguration configuration)
        {
            Console.WriteLine("RabbitMQListener is now listening for user deletion messages...");
            if (_channel == null)
            {
                Console.WriteLine("_channel is null. Ensure it is properly initialized.");
                return;
            }

            _channel.QueueDeclare(queue: "user_deletion_queue",
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                HandleUserDeletion(ea, configuration);
            };

            _channel.BasicConsume(queue: "user_deletion_queue",
                                   autoAck: true,
                                   consumer: consumer);
        }

        public void deleteUsersTweets(IConnection _connection, IModel _channel, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMQ:Url"]),
            };

            _channel.QueueDeclare(queue: "user_deletion_queue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                byte[] messageBodyBytes = ea.Body.ToArray();
                string messageBody = Encoding.UTF8.GetString(messageBodyBytes);

                Console.WriteLine("Received user deletion request:");

                var messageObject = JsonConvert.DeserializeObject<Content>(messageBody);
                string uidAuth = messageObject.Uid;

                Console.WriteLine($"UID: {uidAuth}");

                _contentRepo.DeleteContentsByUserId(uidAuth);

                Console.WriteLine($"Contents related to User with ID {uidAuth} deleted.");
            };

            _channel.BasicConsume(queue: "user_deletion_queue",
                                   autoAck: true,
                                   consumer: consumer);
        }

        public void HandleUserDeletion(BasicDeliverEventArgs e, IConfiguration configuration)
        {
            try
            {
                byte[] messageBodyBytes = e.Body.ToArray();
                string messageBody = Encoding.UTF8.GetString(messageBodyBytes);
                Console.WriteLine($"Received message: {messageBody}");

                string userId = messageBody;
                Console.WriteLine($"Processing deletion for User ID: {userId}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var contentRepo = scope.ServiceProvider.GetRequiredService<IContentRepo>();
                    contentRepo.DeleteContentsByUserId(userId);
                }
                Console.WriteLine($"Media related to User with ID {userId} deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling user deletion message: {ex.Message}");
            }
        }
    }
}
