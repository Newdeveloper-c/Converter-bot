using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private async Task MessageHandler(
        ITelegramBotClient botClient, 
        Message? message, 
        CancellationToken cancellationToken)
    {
        var handlers = message.Type switch
        {
            MessageType.Text => TextProcessing(botClient, message),
            MessageType.Photo => FileProcessing(botClient, message, cancellationToken),
            MessageType.Document => FileProcessing(botClient, message, cancellationToken),
            _ => DefaultTypeHandling(botClient, message)
        };

        try
        {
            await handlers;
        }
        catch (Exception e)
        {
            await HandlePollingErrorAsync(botClient, e, cancellationToken);
        }
    }

    private async Task DefaultTypeHandling(ITelegramBotClient botClient, Message message)
    {
        if(createdTask == EBotTasks.None)
        {
            await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "🟢 Please select needed section from menu 📲");
            return;
        }

        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "❌ Please send file similar to selected menu !!!");
    }
}
