using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using LovePdf.Core;
using LovePdf.Model.Task;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private async Task MessageHandler(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
    {
        var handlers = message.Type switch
        {
            MessageType.Text => TextProcessing(botClient, message, cancellationToken),
            MessageType.Photo => PhotoProcessing(botClient, message, cancellationToken),
            MessageType.Audio => throw new NotImplementedException(),
            MessageType.Video => throw new NotImplementedException(),
            MessageType.Document => DocumentProcessing(botClient, message, cancellationToken),
            MessageType.Unknown => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
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

    private async Task PhotoProcessing(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task TextProcessing(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        
    }
}
