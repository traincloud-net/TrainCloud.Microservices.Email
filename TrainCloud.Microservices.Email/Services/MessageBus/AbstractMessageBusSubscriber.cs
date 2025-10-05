namespace TrainCloud.Microservices.Core.Services.MessageBus;

public abstract class AbstractMessageBusSubscriber<TMessage> : AbstractService<AbstractMessageBusSubscriber<TMessage>>, IHostedService
{
    protected bool IsRunning { get; private set; } = true;

    protected AbstractMessageBusSubscriber(IConfiguration configuration, 
                                           ILogger<AbstractMessageBusSubscriber<TMessage>> logger) : base(configuration, logger)
    {

    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task newTask = new(async () => await DoWork());

        newTask.Start();
        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        while (IsRunning)
        {
            try
            {
                //Wait for RabbitMq Messages here
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
