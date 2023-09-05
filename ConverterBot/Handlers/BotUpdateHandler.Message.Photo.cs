using Telegram.Bot.Types;
using Telegram.Bot;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private async Task PhotoProcessing(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        //TODO photo -> pdf
        throw new NotImplementedException();
    }
}
