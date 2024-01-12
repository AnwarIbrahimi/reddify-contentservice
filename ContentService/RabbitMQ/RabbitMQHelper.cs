using ContentService.RabbitMQ;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Text;

public class RabbitMQHelper : IRabbitMQHelper
{
    private readonly IConfiguration _configuration;
    private readonly string _queueName = "content_queue";
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQHelper(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory() 
        {
            Uri = new Uri(_configuration["RabbitMQ:Url"])
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: _queueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
    }

    public void PublishMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
                              routingKey: _queueName,
                              basicProperties: null,
                              body: body);

        Console.WriteLine($" [x] Sent {message}");
    }

    public void CloseConnection()
    {
        _channel.Close();
        _connection.Close();
    }
}
