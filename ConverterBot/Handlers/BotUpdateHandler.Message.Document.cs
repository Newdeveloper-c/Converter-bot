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
    private bool taskReady = false;
    private async Task FileProcessing(
        ITelegramBotClient botClient,
        Message message,
        CancellationToken cancellationToken)
    {

        switch (createdTask)
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
            case EBotTasks.PdfToWord:
            case EBotTasks.PdfToExcel:
            case EBotTasks.PdfToPowerPoint:
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
                                                  message.Document.FileName, 
                                                  filePath, cancellationToken);
        taskReady = true;
    }

    private async Task<string> GetFile(
        ITelegramBotClient botClient,
        string initialFileName,
        string filePath,
        CancellationToken cancellationToken)
    {
        string ext = Path.GetExtension(filePath);
        string fileName = FixCyrillic(initialFileName
            .Substring(0, initialFileName.IndexOf('.')))
            + AddDateTime();

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

        string destinationPath = $@".\Files\Edited";
        string fileName = Path.GetFileName(currentProcessingFilePath);
        currentProcessingFilePath = string.Empty;

        var api = new LovePdfApi(_options.PublicKey, _options.SecretKey);
        var task = api.CreateTask<OfficeToPdfTask>();
        task.AddFile(currentProcessingFilePath);
        task.Process();
        task.DownloadFile(destinationPath);

        await using (Stream stream = System.IO.File.OpenRead(destinationPath + $@"\{fileName}.pdf"))
        {
            await botClient.SendDocumentAsync(
                message.Chat.Id,
                InputFile.FromStream(stream, $"{fileName}.pdf"),
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

    private string AddDateTime()
    {
        DateTime now = DateTime.UtcNow;
        string format = "yyyyMMddHHmmss";
        string formattedDateTime = now.ToString(format);
        return formattedDateTime;
    }

    private static readonly Dictionary<char, string> ConvertedLetters =
        new Dictionary<char, string>
    {
        {'а', "a"},
        {'б', "b"},
        {'в', "v"},
        {'г', "g"},
        {'д', "d"},
        {'е', "e"},
        {'ё', "yo"},
        {'ж', "zh"},
        {'з', "z"},
        {'и', "i"},
        {'й', "j"},
        {'к', "k"},
        {'л', "l"},
        {'м', "m"},
        {'н', "n"},
        {'о', "o"},
        {'п', "p"},
        {'р', "r"},
        {'с', "s"},
        {'т', "t"},
        {'у', "u"},
        {'ф', "f"},
        {'х', "h"},
        {'ц', "c"},
        {'ч', "ch"},
        {'ш', "sh"},
        {'щ', "sch"},
        {'ъ', "j"},
        {'ы', "i"},
        {'ь', "j"},
        {'э', "e"},
        {'ю', "yu"},
        {'я', "ya"},
        {'А', "A"},
        {'Б', "B"},
        {'В', "V"},
        {'Г', "G"},
        {'Д', "D"},
        {'Е', "E"},
        {'Ё', "Yo"},
        {'Ж', "Zh"},
        {'З', "Z"},
        {'И', "I"},
        {'Й', "J"},
        {'К', "K"},
        {'Л', "L"},
        {'М', "M"},
        {'Н', "N"},
        {'О', "O"},
        {'П', "P"},
        {'Р', "R"},
        {'С', "S"},
        {'Т', "T"},
        {'У', "U"},
        {'Ф', "F"},
        {'Х', "H"},
        {'Ц', "C"},
        {'Ч', "Ch"},
        {'Ш', "Sh"},
        {'Щ', "Sch"},
        {'Ъ', "J"},
        {'Ы', "I"},
        {'Ь', "J"},
        {'Э', "E"},
        {'Ю', "Yu"},
        {'Я', "Ya"}
    };

    private static string FixCyrillic(string source)
    {
        var result = new StringBuilder();
        foreach (var letter in source)
        {
            if (ConvertedLetters.ContainsKey(letter))
                result.Append(ConvertedLetters[letter]);
            else
                result.Append(letter);
        }
        return result.ToString();
    }
}
