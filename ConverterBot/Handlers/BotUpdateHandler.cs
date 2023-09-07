using ConverterBot.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler : IUpdateHandler
{
    private readonly ILogger<BotUpdateHandler> _logger;
    private readonly LovePdfOptions _options;

    public BotUpdateHandler(
        ILogger<BotUpdateHandler> logger,
        IOptions<LovePdfOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public Task HandlePollingErrorAsync(
        ITelegramBotClient botClient, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error while polling for updates");
        return Task.CompletedTask;
    }

    enum EBotTasks
    {
        None = 0,
        Word = 1,
        Excel = 2,
        PowerPoint = 3,
        PdfToWord = 4,
        PdfToExcel = 5,
        PdfToPowerPoint = 6,
        Image = 7
    }
    private bool isStarted = false;
    private bool needDirsCreated = false;
    private EBotTasks creatingTask = EBotTasks.None;

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Incoming message\n From: {update.Message?.From?.Username}\n" +
            $"Type: {update.Message.Type}\n" +
           $"Text: {update.Message?.Text}");

        if (!isStarted && update.Message?.Text != "/start")
        {
            await botClient.SendTextMessageAsync(
                update.Message.Chat.Id,
                "🟢 Please start the bot with /start 🙂");
            return;
        }

        if (!needDirsCreated)
        {
            Directory.CreateDirectory($@".\Files");
            Directory.CreateDirectory($@".\Files\Edited");
            needDirsCreated = true;
        }

        var handlers = update.Type switch
        {
            UpdateType.Message => MessageHandler(botClient, update.Message, cancellationToken),
            UpdateType.CallbackQuery => CallbackQueryHandler(botClient, update.CallbackQuery, cancellationToken),
            _ => DefaultUpdateType(botClient, update)
        };

        try
        {
            await handlers;
        }
        catch (Exception ex)
        {
            await HandlePollingErrorAsync(botClient, ex, cancellationToken);
        }
    }

    private async Task DefaultUpdateType(ITelegramBotClient botClient, Update update)
    {
        await botClient.SendTextMessageAsync(
                update.Message.Chat.Id,
                "❌ Please do not send improper messages !!!");
    }
}