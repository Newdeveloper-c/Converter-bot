using LovePdf.Core;
using LovePdf.Model.Task;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Text;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private string currentProcessingFilePath = string.Empty;
    private string currentProcessingImagesFolder = string.Empty;
    private string currentFileName = string.Empty;
    private bool taskReady = false;
    private async Task FileProcessing(
        ITelegramBotClient botClient,
        Message message,
        CancellationToken cancellationToken)
    {
        switch (creatingTask)
        {
            case EBotTasks.None:
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "🟢 Please select needed section from menu 📲");
                break;
            case EBotTasks.Word:
                await WordFile(botClient, message, cancellationToken);
                break;
            case EBotTasks.Excel:
                await ExcelFile(botClient, message, cancellationToken);
                break;
            case EBotTasks.PowerPoint:
                await PowerPointFile(botClient, message, cancellationToken);
                break;
            case EBotTasks.Image:
                await Images(botClient, message, cancellationToken);
                break;
            case EBotTasks.PdfToWord or
                 EBotTasks.PdfToExcel or
                 EBotTasks.PdfToPowerPoint:
                await PdfFile(botClient, message, cancellationToken);
                break;
            default:
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "🛑 Something went wrong please try again later 🫣");
                break;
        }

    }

    private async Task PdfFile(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var fileId = message.Document.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;
        string ext = Path.GetExtension(filePath);

        if (ext != ".pdf")
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "❌ Please send .pdf file !!!");
            return;
        }

        currentProcessingFilePath = await GetFile(botClient,
                                                  message,
                                                  message.Document.FileName,
                                                  filePath, cancellationToken);
        taskReady = true;
    }

    private async Task PowerPointFile(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var fileId = message.Document.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;
        string ext = Path.GetExtension(filePath);

        if (ext != ".ppt" &&
            ext != ".pptx")
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "❌ Please send .ppt or .pptx file !!!");
            return;
        }

        currentProcessingFilePath = await GetFile(botClient,
                                                  message,
                                                  message.Document.FileName,
                                                  filePath, cancellationToken);
        taskReady = true;
    }

    private async Task ExcelFile(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var fileId = message.Document.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;
        string ext = Path.GetExtension(filePath);

        if (ext != ".xls" &&
            ext != ".xlsx")
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "❌ Please send .xls or .xlsx file !!!");
            return;
        }

        currentProcessingFilePath = await GetFile(botClient,
                                                  message,
                                                  message.Document.FileName,
                                                  filePath, cancellationToken);
        taskReady = true;
    }

    private async Task WordFile(
        ITelegramBotClient botClient,
        Message message,
        CancellationToken cancellationToken)
    {
        var fileId = message.Document.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;
        var ext = Path.GetExtension(filePath);

        if(ext != ".doc" &&  
            ext != ".docx")
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "❌ Please send .doc or .docx file !!!");
            return;
        }

        currentProcessingFilePath = await GetFile(botClient, 
                                                  message,
                                                  message.Document.FileName, 
                                                  filePath, cancellationToken);
        taskReady = true;
    }

    private async Task<string> GetFile(
        ITelegramBotClient botClient,
        Message message,
        string initialFileName,
        string filePath,
        CancellationToken cancellationToken)
    {
        if (currentProcessingFilePath != string.Empty)
        {
            await botClient.SendTextMessageAsync(
                 message.Chat.Id,
                "‼️ Only last sended file will be converted.\n" +
                "Please click on Convert 🔄");
            DeleteTemps();
        }
            
        string ext = Path.GetExtension(filePath);
        string fileName = FixCyrillic(initialFileName
            .Substring(0, initialFileName.IndexOf('.')))
            + AddDateTime();
        currentFileName = fileName;
        string destinationPath = $@".\Files\{fileName + ext}";

        await using (FileStream fileStream = System.IO.File.OpenWrite(destinationPath))
        {
            await botClient.DownloadFileAsync(filePath, fileStream, cancellationToken);
        }

        return destinationPath;
    }

    private async Task OfficeToPdfProcessing(
        ITelegramBotClient botClient,
        Message message)
    {
        if(currentProcessingFilePath == string.Empty)
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "❌ Something went wrong. Please try again 🤕");
            return;
        }

        var api = new LovePdfApi(_options.PublicKey, _options.SecretKey);
        var task = api.CreateTask<OfficeToPdfTask>();

        task.AddFile(currentProcessingFilePath);
        task.Process();
        var bytes = await task.DownloadFileAsByteArrayAsync();


        await using (Stream stream = new MemoryStream(bytes))
        {
            await botClient.SendDocumentAsync(
                message.Chat.Id,
                InputFile.FromStream(stream, $"{currentFileName}.pdf"),
                replyMarkup: BotTaskButtonMenu());
        }
    }

    private async Task PdfToOfficeProcessing(
        ITelegramBotClient botClient,
        Message message)
    {
        //TODO use cloudconvert
        await botClient.SendTextMessageAsync(
        message.Chat.Id,
        "This feature is developing yet !!!");
    }
}
