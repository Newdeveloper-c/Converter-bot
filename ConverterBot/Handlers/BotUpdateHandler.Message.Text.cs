using Telegram.Bot.Types;
using Telegram.Bot;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private async Task TextProcessing(
        ITelegramBotClient botClient,
        Message message)
    {

        switch (message.Text)
        {
            case "/start":
                isStarted = true;
                createdTask = EBotTasks.None;
                await StartTheBot(botClient, message);
                break;

            case "Convert to Pdf ➡️📒":
                createdTask = EBotTasks.None;
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Select needed section",
                    replyMarkup: BotConvertToPdfMenu());
                break;

            case "📒➡️ Convert Pdf to":
                createdTask = EBotTasks.None;
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Select needed section",
                    replyMarkup: BotConvertPdfToMenu());
                break;

            case "/help":
            case "Help 🆘":
                createdTask = EBotTasks.None;
                await SendHelpInfo(botClient, message);
                break;

            case "📘 Word -> Pdf 📒":
            case "📗 Exsel -> Pdf 📒":
            case "📙 PowerPoint -> Pdf 📒":
                createdTask = EBotTasks.OfficeToPdf;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄",
                replyMarkup: BotBackButtonMenu());
                break;

            case "📒 Pdf -> Word 📘":
                createdTask = EBotTasks.PdfToDoc;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄",
                replyMarkup: BotBackButtonMenu());
                break;

            case "Go back ⬅️":
                if(createdTask == EBotTasks.None)
                    await botClient.SendTextMessageAsync(
                        message.Chat.Id,
                        $"Select needed section",
                        replyMarkup: BotMainMenu());
                else if(createdTask == EBotTasks.OfficeToPdf)
                    await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Select needed section",
                            replyMarkup: BotConvertToPdfMenu());
                else
                    await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Select needed section",
                            replyMarkup: BotConvertPdfToMenu());
                createdTask = EBotTasks.None;
                break;

            default:
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "🟠 Please choose section from menu for converting your files " +
                    "or get help with typing /help 🤗");
                break;
        }

    }

    private async Task SendHelpInfo(
        ITelegramBotClient botClient,
        Message message)
    {
        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Here all information about this bot. This section is developing yet !!!",
            replyMarkup: BotMainMenu());
    }

    private async Task StartTheBot(
        ITelegramBotClient botClient,
        Message message)
    {
        await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    $"📣 Hello {message.Chat.FirstName}. Its great to see you here 😃\n" +
                    $"🧡 I am files converter telegram bot 😊\n" +
                    $"🟢 You can start converting your files with just clinking need menu button and sending your file 🙃\n" +
                    $"🟡 if something is not clear, do not hesitate to call for help with /help 😇",
                    replyMarkup: BotMainMenu());
    }
}
