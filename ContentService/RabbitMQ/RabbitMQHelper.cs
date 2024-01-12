using RabbitMQ.Client;
using System;
using System.Text;

public class RabbitMQHelper
{
    private readonly IModel _channel;

    public RabbitMQHelper(IModel channel)
    {
        _channel = channel;
    }

    public void SendMessage(string message)
    {
        // Send a message
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "", routingKey: "contents_queue", basicProperties: null, body: body);

        Console.WriteLine($" [x] Sent '{message}'");
    }
}

