using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using PowerLinesMessaging;
using Microsoft.Extensions.Options;

namespace PowerLinesFixtureService.Messaging;

public class MessageService(IOptions<MessageOptions> messageOptions, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private IConnection connection;
    private Consumer fixtureConsumer;
    private Consumer oddsConsumer;
    private readonly MessageOptions messageOptions = messageOptions.Value;
    private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        CreateConnection();
        CreateFixtureConsumer();
        CreateOddsConsumer();

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        fixtureConsumer.Listen(new Action<string>(ReceiveFixtureMessage));
        oddsConsumer.Listen(new Action<string>(ReceiveOddsMessage));
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        connection.CloseConnection();
    }

    protected void CreateConnection()
    {
        var options = new ConnectionOptions
        {
            Host = messageOptions.Host,
            Port = messageOptions.Port,
            Username = messageOptions.Username,
            Password = messageOptions.Password
        };
        connection = new Connection(options);
    }

    protected void CreateFixtureConsumer()
    {
        var options = new ConsumerOptions
        {
            Name = messageOptions.FixtureQueue,
            QueueName = messageOptions.FixtureQueue,
            SubscriptionQueueName = messageOptions.FixtureSubscription,
            QueueType = QueueType.ExchangeFanout
        };

        fixtureConsumer = connection.CreateConsumerChannel(options);
    }

    protected void CreateOddsConsumer()
    {
        var options = new ConsumerOptions
        {
            Name = messageOptions.OddsQueue,
            QueueName = messageOptions.OddsQueue,
            SubscriptionQueueName = messageOptions.OddsSubscription,
            QueueType = QueueType.ExchangeDirect,
            RoutingKey = "power-lines-fixture-service"
        };

        oddsConsumer = connection.CreateConsumerChannel(options);
    }

    private void ReceiveFixtureMessage(string message)
    {
        var fixture = JsonConvert.DeserializeObject<Fixture>(message);
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            dbContext.Fixtures.Add(fixture);
            dbContext.SaveChanges();
        }
        catch (DbUpdateException)
        {
            Console.WriteLine("{0} v {1} exists, skipping", fixture.HomeTeam, fixture.AwayTeam);
        }
    }

    private void ReceiveOddsMessage(string message)
    {
        var matchOdds = JsonConvert.DeserializeObject<MatchOdds>(message);
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.MatchOdds.Upsert(matchOdds)
            .On(x => new { x.FixtureId })
            .Run();
    }
}
