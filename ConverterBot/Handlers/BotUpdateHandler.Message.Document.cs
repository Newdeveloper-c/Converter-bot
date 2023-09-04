using LovePdf.Core;
using LovePdf.Model.Task;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private async Task DocumentProcessing(
        ITelegramBotClient botClient,
        Message message,
        CancellationToken cancellationToken)
    {
        //getting office documets from user
        var fileId = message.Document.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;
        string destinationPath = $@".\Files\{message.Document.FileName}";
        await using (FileStream fileStream = System.IO.File.OpenWrite(destinationPath))
        {
            await botClient.DownloadFileAsync(filePath, fileStream, cancellationToken);
        }

        string editedDestinationPath = $@".\Files\Edited";
        FileConverting(destinationPath, editedDestinationPath);

        //sending ready pdf file to user
        var fileName = message.Document.FileName.Substring(0, message.Document.FileName.IndexOf('.'));
        await using (Stream stream = System.IO.File.OpenRead(editedDestinationPath + $@"\{fileName}.pdf"))
        {
            await botClient.SendDocumentAsync(
                message.Chat.Id,
                InputFile.FromStream(stream, $"{fileName}(edited).pdf"));
        }
    }

    void FileConverting(string srcPath, string destinationPath)
    {
        var api = new LovePdfApi(
            "project_public_372651370262b80a1740e9fab420a285_6z_q50b1f90ecce7a2c6f27f0bde39d059e88",
            "secret_key_47e9736218f50019a4b6b0807118ea2d_4DUvYb06cd5b5ad036c338761e9369783bd74");
        var task = api.CreateTask<OfficeToPdfTask>();
        task.AddFile(srcPath);
        task.Process();
        task.DownloadFile(destinationPath);
    }

}
