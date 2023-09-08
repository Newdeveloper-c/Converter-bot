using Telegram.Bot.Types;
using Telegram.Bot;
using LovePdf.Core;
using LovePdf.Model.Task;
using Telegram.Bot.Types.Enums;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private string currentProcessingImagesFolder = string.Empty;

    private async Task Images(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        string fileId;
        if (message.Type == MessageType.Photo)
            fileId = message.Photo.Last().FileId;
        else
            fileId = message.Document.FileId;
        
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;
        string ext = Path.GetExtension(filePath);

        if (ext != ".jpg" &&
            ext != ".jpeg" &&
            ext != ".png")
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "❌ Please send .jpg, .jpeg or .png images !!!");
            return;
        }

        GetImages(botClient, filePath, cancellationToken);
        taskReady = true;
    }

    private async Task GetImages(
        ITelegramBotClient botClient,
        string? filePath,
        CancellationToken cancellationToken)
    {
        string initialFileName = Path.GetFileName(filePath);
        string ext = Path.GetExtension(filePath);
        string fileName = FixCyrillic(initialFileName
            .Substring(0, initialFileName.IndexOf('.')))
            + AddDateTime();

        
        if (currentProcessingImagesFolder == string.Empty)
        {
            currentProcessingImagesFolder = $@".\Files\Images_{AddDateTime()}";
            Directory.CreateDirectory(currentProcessingImagesFolder);
        }
        
        await using (FileStream fileStream = System.IO.File.OpenWrite($"{currentProcessingImagesFolder}/{fileName}{ext}"))
        {
            await botClient.DownloadFileAsync(filePath, fileStream, cancellationToken);
        }
    }

    private async Task ImagesToPdfProcessing(
        ITelegramBotClient botClient,
        Message message,
        string pdfFileName)
    {
        if (currentProcessingImagesFolder == string.Empty)
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "❌ Something went wrong. Please try again 🤕");
            return;
        }

        string destinationPath = currentProcessingImagesFolder;
        var images = Directory.GetFiles(destinationPath);

        var api = new LovePdfApi(_options.PublicKey, _options.SecretKey);
        var task = api.CreateTask<ImageToPdfTask>();
        foreach (var image in images)
            task.AddFile(image);
        task.Process(new LovePdf.Model.TaskParams.ImageToPdfParams
        {
            Orientation = LovePdf.Model.Enums.Orientations.Portrait,
            Margin = 10,
            PageSize = LovePdf.Model.Enums.PageSizes.A4
            MergeAfter = true
    }) ;
        var bytes = await task.DownloadFileAsByteArrayAsync();

        await using (Stream stream = new MemoryStream(bytes))
        {
            await botClient.SendDocumentAsync(
                message.Chat.Id,
                InputFile.FromStream(stream, $"{pdfFileName}.pdf"),
                replyMarkup: BotTaskButtonMenu());
        }
    }
}
