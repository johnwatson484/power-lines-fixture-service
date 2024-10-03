using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using Microsoft.EntityFrameworkCore;
using PowerLinesMessaging;
using PowerLinesFixtureService.Messaging;
using Microsoft.Extensions.Options;

namespace PowerLinesFixtureService.Analysis;

public class AnalysisService : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IAnalysisApi analysisApi;
    private readonly MessageOptions messageOptions;
    private Connection connection;
    private Sender sender;
    private Timer timer;
    private readonly int frequencyInMinutes;

    public AnalysisService(IServiceScopeFactory serviceScopeFactory, IAnalysisApi analysisApi, IOptions<MessageOptions> messageOptions, int frequencyInMinutes = 1)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.analysisApi = analysisApi;
        this.messageOptions = messageOptions.Value;
        this.frequencyInMinutes = frequencyInMinutes;
    }

    public override Task StartAsync(CancellationToken stoppingToken)
    {
        CreateConnection();
        CreateSender();

        return base.StartAsync(stoppingToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        timer = new Timer(GetMatchOdds, null, TimeSpan.Zero, TimeSpan.FromMinutes(frequencyInMinutes));
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

    protected void CreateSender()
    {
        var options = new SenderOptions
        {
            Name = messageOptions.AnalysisQueue,
            QueueName = messageOptions.AnalysisQueue,
            QueueType = QueueType.ExchangeFanout
        };

        sender = connection.CreateSenderChannel(options);
    }

    protected void GetMatchOdds(object state)
    {
        var lastResultDate = GetLastResultDate();

        if (lastResultDate.HasValue)
        {
            CheckPendingFixtures(lastResultDate.Value);
        }
    }

    private DateTime? GetLastResultDate()
    {
        return Task.Run(() => analysisApi.GetLastResultDate()).Result;
    }

    private void CheckPendingFixtures(DateTime lastResultDate)
    {
        List<Fixture> pendingFixtures;
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            pendingFixtures = dbContext.Fixtures.AsNoTracking().Include(x => x.MatchOdds).Where(x => x.Date >= DateTime.Today
            && (x.MatchOdds == null
            || x.MatchOdds.Calculated == null
            || x.MatchOdds.Calculated < lastResultDate)
            ).ToList();
        }

        if (pendingFixtures.Count > 0)
        {
            SendFixturesForAnalysis(pendingFixtures);
        }
    }

    private void SendFixturesForAnalysis(List<Fixture> fixtures)
    {
        foreach (var fixture in fixtures)
        {
            sender.SendMessage(new AnalysisMessage(fixture));
        }
    }
}
