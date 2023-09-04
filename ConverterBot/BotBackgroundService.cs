using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace ConverterBot;

public class BotBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;
    private readonly ILogger<BotBackgroundService> _logger;   

    public BotBackgroundService(
        ITelegramBotClient botClient, 
        IUpdateHandler updateHandler,
        ILogger<BotBackgroundService> logger)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bot = await _botClient.GetMeAsync( stoppingToken);

        _logger.LogInformation(JsonSerializer.Serialize(
            bot,
            new JsonSerializerOptions
            {
                WriteIndented = true,
            }));

        _botClient.StartReceiving(
            _updateHandler.HandleUpdateAsync,
            _updateHandler.HandlePollingErrorAsync,
            new ReceiverOptions
            {
                ThrowPendingUpdates = true
            },
            cancellationToken: stoppingToken);
    }
}