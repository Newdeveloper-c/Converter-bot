using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Requests;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private bool getName = false;
    private async Task TextProcessing(
        ITelegramBotClient botClient,
        Message message)
    {
        string? fileName = string.Empty;
        if(getName)
        {
            fileName = message.Text;
            getName = false;
            
            await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Please send your images 🏞\n" +
                    "Then click Convert 🔄");
            return;
        }
        switch (message.Text)
        {
            
            case "/start":
                isStarted = true;
                createdTask = EBotTasks.None;
                taskReady = false;
                await StartTheBot(botClient, message);
                break;

            case "Convert to Pdf ➡️📒":
                createdTask = EBotTasks.None;
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Select needed section",
                    replyMarkup: BotConvertToPdfMenu());
                break;

            case "📒➡️ Convert from Pdf":
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
                createdTask = EBotTasks.Word;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;
                
            case "📗 Excel -> Pdf 📒":
                createdTask = EBotTasks.Excel;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;
                
            case "📙 PowerPoint -> Pdf 📒":
                createdTask = EBotTasks.PowerPoint;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "🏞 Images -> Pdf 📒":
                createdTask = EBotTasks.Image;
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Please, give name for pdf file:",
                    replyMarkup: BotTaskButtonMenu());
                getName = true;
                break;

            case "📒 Pdf -> Word 📘":
                createdTask = EBotTasks.PdfToWord;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "📒 Pdf -> Excel 📗":
                createdTask = EBotTasks.PdfToExcel;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "📒 Pdf -> PowerPoint 📙":
                createdTask = EBotTasks.PdfToPowerPoint;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "Convert 🔄":
                if (taskReady)
                {
                    switch (createdTask)
                    {
                        case EBotTasks.Word:
                        case EBotTasks.Excel:
                        case EBotTasks.PowerPoint:
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "🔄 Converting 🔄",
                                replyMarkup: BotBackButtonMenu());

                            await OfficeToPdfProcessing(botClient, message);
                            break;
                        case EBotTasks.Image:
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "🔄 Converting 🔄",
                                replyMarkup: BotBackButtonMenu());

                            await ImagesToPdfProcessing(botClient, message, fileName ?? "file");
                            break;
                        case EBotTasks.PdfToWord:
                        case EBotTasks.PdfToExcel:
                        case EBotTasks.PdfToPowerPoint:
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "🔄 Converting 🔄",
                                replyMarkup: BotBackButtonMenu());

                            await PdfToOfficeProcessing(botClient, message);
                            break;
                    }
                    taskReady = false;

                }
                else
                {
                    string answer = "‼️ Please send your file ‼️";
                    if (createdTask == EBotTasks.Image)
                        answer = "‼️ Please send your images ‼️";
                    await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                answer);
                }
                    
                    
                break;

            case "Go back ⬅️":
                if(createdTask == EBotTasks.None)
                    await botClient.SendTextMessageAsync(
                        message.Chat.Id,
                        $"Select needed section",
                        replyMarkup: BotMainMenu());
                else if(createdTask == EBotTasks.Word ||
                        createdTask == EBotTasks.Excel ||
                        createdTask == EBotTasks.PowerPoint)
                    await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Select needed section",
                            replyMarkup: BotConvertToPdfMenu());
                else if(createdTask == EBotTasks.PdfToWord ||
                        createdTask == EBotTasks.PdfToPowerPoint ||
                        createdTask == EBotTasks.PdfToExcel)
                    await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Select needed section",
                            replyMarkup: BotConvertPdfToMenu());
                else
                    await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Select needed section",
                            replyMarkup: BotConvertToPdfMenu());
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
        //TODO write full info about bot. But after finishing it !!!
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
