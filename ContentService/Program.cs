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

    // Provide the necessary variables and methods to handle the UID
    //var pictureRepository = serviceProvider.GetRequiredService<IMediaRepo>();

    // Provide the callback function for handling the UID
    Action<string> onUidReceived = uid =>
    {
        // Use the received UID to handle the creation of a new picture
        var pictureModel = new Content(); // Replace with your actual logic
        pictureModel.Uid = uid;

        // Save the updated picture model to the database
        //pictureRepository.CreatePicture(pictureModel);
        //pictureRepository.saveChanges();
    };

    // Provide the callback function for handling the tweet request
    Action<string> onTweetReceived = tweetRequest =>
    {
        // Process the tweet request
        Console.WriteLine($"Received tweet request: {tweetRequest}");
        // Implement your logic to handle the tweet request
    };

    // Create and return RabbitMQListener with both callback functions
    return new RabbitMQListener(serviceProvider, connection, channel);
});

//var app = builder.Build();

//// Initialize RabbitMQListener to start listening for UID messages
//var rabbitMQListener = app.Services.GetRequiredService<RabbitMQListener>();

var app = builder.Build();

// Initialize RabbitMQListener to start listening for UID messages
var rabbitMQListener = app.Services.GetRequiredService<RabbitMQListener>();
rabbitMQListener.StartListening(configuration);

// Configure the HTTP request pipeline.
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
