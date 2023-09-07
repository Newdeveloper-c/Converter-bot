using Telegram.Bot.Types;
using Telegram.Bot;
using LovePdf.Core;
using LovePdf.Model.Task;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private async Task Images(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var fileId = message.Document.FileId;
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

        currentProcessingFilePath = await GetImages(botClient,
                                                  message,
                                                  filePath, cancellationToken);
        taskReady = true;
    }

    private async Task<string> GetImages(
        ITelegramBotClient botClient,
        Message message,
        string? filePath,
        CancellationToken cancellationToken)
    {
        string initialFileName = Path.GetFileName(filePath);
        string ext = Path.GetExtension(filePath);
        string fileName = FixCyrillic(initialFileName
            .Substring(0, initialFileName.IndexOf('.')))
            + AddDateTime();

        string destinationFolder;
        if (currentProcessingImagesFolder == string.Empty)
        {
            destinationFolder = $@".\Files\Images_{AddDateTime()}";
            Directory.CreateDirectory(destinationFolder);
        }
        else
            destinationFolder = currentProcessingImagesFolder;

        var destinationPath = destinationFolder + fileName + ext;
        await using (FileStream fileStream = System.IO.File.OpenWrite(destinationPath))
        {
            await botClient.DownloadFileAsync(filePath, fileStream, cancellationToken);
        }

        return destinationPath;
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
        currentProcessingFilePath = string.Empty;

        var api = new LovePdfApi(_options.PublicKey, _options.SecretKey);
        var task = api.CreateTask<ImageToPdfTask>();
        foreach (var image in images)
            task.AddFile(image);
        task.Process();
        task.DownloadFile(destinationPath);

        await using (Stream stream = System.IO.File.OpenRead(destinationPath + $@"\{pdfFileName}.pdf"))
        {
            await botClient.SendDocumentAsync(
                message.Chat.Id,
                InputFile.FromStream(stream, $"{pdfFileName}.pdf"),
                replyMarkup: BotTaskButtonMenu());
        }
    }
}
