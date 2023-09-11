using Telegram.Bot.Types;
using Telegram.Bot;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private bool getName = false;
    private async Task TextProcessing(
        ITelegramBotClient botClient,
        Message message)
    {
        if (getName)
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "🔄 Converting 🔄\n" +
                "‼️ Be patientю This may take some time ‼️",
                replyMarkup: BotBackButtonMenu());
            await ImagesToPdfProcessing(botClient, message, message.Text ?? "file");
            getName = false;
            DeleteTemps();
            taskReady = false;
            return;
        }

        switch (message.Text)
        {
            case "/start":
                isStarted = true;
                creatingTask = EBotTasks.None;
                taskReady = false;
                getName = false;
                await BotStart(botClient, message);
                break;

            case "Convert to Pdf ➡️📒":
                creatingTask = EBotTasks.None;
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Select needed section",
                    replyMarkup: BotConvertToPdfMenu());
                break;

            case "📒➡️ Convert from Pdf":
                creatingTask = EBotTasks.None;
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "🛠  This section is developing yet. Please wait for updates  ‼️");
                    //$"Select needed section",
                    //replyMarkup: BotConvertPdfToMenu());
                break;

            case "/help" or "Help 🆘":
                creatingTask = EBotTasks.None;
                await BotHelp(botClient, message);
                break;

            case "📘 Word -> Pdf 📒":
                creatingTask = EBotTasks.Word;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "📗 Excel -> Pdf 📒":
                creatingTask = EBotTasks.Excel;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "📙 PowerPoint -> Pdf 📒":
                creatingTask = EBotTasks.PowerPoint;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "🏞 Images -> Pdf 📒":
                creatingTask = EBotTasks.Image;
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Please send your images 🏞\n" +
                    "Then click Convert 🔄",
                    replyMarkup: BotTaskButtonMenu());
                break;

            case "📒 Pdf -> Word 📘":
                creatingTask = EBotTasks.PdfToWord;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "📒 Pdf -> Excel 📗":
                creatingTask = EBotTasks.PdfToExcel;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "📒 Pdf -> PowerPoint 📙":
                creatingTask = EBotTasks.PdfToPowerPoint;
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Please send your file 📄\n" +
                "Then click Convert 🔄",
                replyMarkup: BotTaskButtonMenu());
                break;

            case "Convert 🔄":
                if (taskReady)
                {
                    switch (creatingTask)
                    {
                        case EBotTasks.None:
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "‼️ Please send your file or images for converting ‼️");
                            break;

                        case EBotTasks.Word or EBotTasks.Excel or EBotTasks.PowerPoint:
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "🔄 Converting 🔄\n" +
                                "‼️ Be patientю This may take some time ‼️",
                                replyMarkup: BotBackButtonMenu());

                            await OfficeToPdfProcessing(botClient, message);
                            break;

                        case EBotTasks.Image:
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Please, give name for pdf file:");
                            getName = true;
                            return;

                        case EBotTasks.PdfToWord or EBotTasks.PdfToPowerPoint or EBotTasks.PdfToExcel:
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "🔄 Converting 🔄\n" +
                                "‼️ Be patientю This may take some time ‼️",
                                replyMarkup: BotBackButtonMenu());

                            await PdfToOfficeProcessing(botClient, message);
                            break;
                    }
                    DeleteTemps();
                    taskReady = false;
                }
                else
                {
                    string answer = "‼️ Please send your file ‼️";
                    if (creatingTask == EBotTasks.Image)
                        answer = "‼️ Please send your images ‼️";
                    await botClient.SendTextMessageAsync(
                        message.Chat.Id,
                        answer);
                }
                break;

            case "Go back ⬅️":
                switch (creatingTask)
                {
                    case EBotTasks.None:
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Select needed section",
                            replyMarkup: BotMainMenu());
                        break;

                    case EBotTasks.Word or EBotTasks.Excel or EBotTasks.PowerPoint:
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Select needed section",
                            replyMarkup: BotConvertToPdfMenu());
                        break;

                    case EBotTasks.PdfToWord or EBotTasks.PdfToPowerPoint or EBotTasks.PdfToExcel:
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Select needed section",
                            replyMarkup: BotConvertPdfToMenu());
                        break;

                    case EBotTasks.Image:
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Select needed section",
                            replyMarkup: BotConvertToPdfMenu());
                        break;
                }
                DeleteTemps();
                creatingTask = EBotTasks.None;
                taskReady = false;
                break;

            default:
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "🟠 Please choose section from menu for converting your files " +
                    "or get help with /help ");
                break;
        }
    }

    private async Task BotHelp(
        ITelegramBotClient botClient,
        Message message)
    {
        //TODO write full info about the bot.
        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "🛠  This section is developing yet. Please wait for updates  ‼️",
            replyMarkup: BotMainMenu());
    }

    private async Task BotStart(
        ITelegramBotClient botClient,
        Message message)
    {
        await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    $"📣  Hello {message.Chat.FirstName}. Its great to see you here\n" +
                    $"🧡  I am File-Converter telegram bot\n" +
                    $"🟢  You can start converting your files with just clicking need menu button and sending your file\n" +
                    $"🟡  If something is not clear, do not hesitate to call for help with /help",
                    replyMarkup: BotMainMenu());
    }
}
