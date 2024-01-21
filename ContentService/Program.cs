using ContentService.AsyncDataServices;
using ContentService.Data;
using ContentService.Models;
using ContentService.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddControllers();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration;

// Change the following line to use PostgreSQL provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddScoped<IContentRepo, ContentRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add RabbitMQ services
builder.Services.AddSingleton(serviceProvider =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(configuration["RabbitMQ:Url"]),
    };

    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();

    Action<string> onUidReceived = uid =>
    {
        var pictureModel = new Content();
        pictureModel.Uid = uid;
    };

    Action<string> onTweetReceived = tweetRequest =>
    {
        Console.WriteLine($"Received tweet request: {tweetRequest}");
    };

    return new RabbitMQListener(serviceProvider, connection, channel);
});

var app = builder.Build();

var rabbitMQListener = app.Services.GetRequiredService<RabbitMQListener>();
rabbitMQListener.StartListening(configuration);

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
