using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Services.MessageBus;

public abstract class AbstractMessageBusSubscriber<TMessage> : AbstractService<AbstractMessageBusSubscriber<TMessage>>, IHostedService
{
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _disposed;

    protected bool IsRunning { get; private set; } = true;

    // Abstract property to define the queue/topic name in derived classes
    protected string TopicId { get; } = "email";

    protected AbstractMessageBusSubscriber(IConfiguration configuration, 
                                           ILogger<AbstractMessageBusSubscriber<TMessage>> logger) : base(configuration, logger)
    {

    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            ConnectionFactory factory = new()
            {
                HostName = "rabbitmq-service-queue.traincloud-dev.svc.cluster.local",
                Port = 5672,
                UserName = "testuser",
                Password = "pa$$word"
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // Declare the queue (ensures it exists with same settings as publisher)
            await _channel.QueueDeclareAsync(
                queue: TopicId,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            // Set prefetch count
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: cancellationToken
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                if (!IsRunning)
                    return;

                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = System.Text.Encoding.UTF8.GetString(body);

                    Logger.LogDebug($"Received message from queue '{TopicId}': {messageJson}");

                    // Deserialize the message
                    var message = JsonSerializer.Deserialize<TMessage>(messageJson);

                    if (message != null)
                    {
                        // Call the virtual method for processing
                        await OnMessageAsync(message);

                        // Acknowledge the message after successful processing
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        Logger.LogDebug($"Message processed and acknowledged from queue '{TopicId}'");
                    }
                    else
                    {
                        Logger.LogWarning($"Received null data after deserialization from queue '{TopicId}'");
                        // Reject without requeue
                        await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (JsonException jsonEx)
                {
                    Logger.LogError(jsonEx, $"Failed to deserialize message from queue '{TopicId}'");
                    // Reject without requeue (poison message)
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error processing message from queue '{TopicId}'");
                    // Reject and requeue for retry
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            // Start consuming messages
            await _channel.BasicConsumeAsync(
                queue: TopicId,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken
            );

            Logger.LogInformation($"Started listening to RabbitMQ queue: {TopicId}");
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, $"Failed to start RabbitMQ subscriber for queue '{TopicId}'");
            throw;
        }
    }

    private async Task DoWork()
    {
        while (IsRunning)
        {
            try
            {
                //Wait for RabbitMq Messages here
                ConnectionFactory factory = new() { HostName = "rabbitmq-service.traincloud-net.svc.cluster.local" };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
    }

    public virtual async Task OnMessageAsync(TMessage message)
    {
        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }
}
