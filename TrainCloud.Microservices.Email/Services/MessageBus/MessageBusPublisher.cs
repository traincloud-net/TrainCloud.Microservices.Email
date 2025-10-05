using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public sealed class MessageBusPublisher : AbstractService<MessageBusPublisher>, IMessageBusPublisherService
{
    public MessageBusPublisher(IConfiguration configuration, 
                               ILogger<MessageBusPublisher> logger) 
        : base(configuration, logger)
    {

    }

    public async Task SendMessageAsync<TData>(string topicId, TData data)
    {
        ConnectionFactory factory = new() { HostName = "_hostname" };

        using IConnection connection = await factory.CreateConnectionAsync();
        using IChannel channel = await connection.CreateChannelAsync();

        // Declare the queue (creates if doesn't exist)
        await channel.QueueDeclareAsync(queue: topicId,
            durable: true,        // Survives broker restart
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // Serialize the message to JSON
        string dataJson = JsonSerializer.Serialize(data);
        var dataBytes = Encoding.UTF8.GetBytes(dataJson);

        //// Set message as persistent
        //var properties = channel.pr();
        //properties.Persistent = true;

       // Publish the message
       //await channel.BasicPublishAsync(exchange: "",
       //    routingKey: topicId,
       //    basicProperties: properties,
       //    body: dataBytes
       //);
    }
}
