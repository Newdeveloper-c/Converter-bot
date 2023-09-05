using LovePdf.Core;
using LovePdf.Model.Task;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Text;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    private static int fileCount = 1;

    private async Task DocumentProcessing(
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
            case EBotTasks.OfficeToPdf:
                await OfficeToPdfProcessing(botClient, message, cancellationToken);
                break;
            case EBotTasks.PdfToDoc:
                await PdfToDocProcessing(botClient, message, cancellationToken);
                break;
            default:
                await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "🛑 Something went wrong please try again 🫣");
                break;
        }
        createdTask = EBotTasks.None;
    }

    private async Task OfficeToPdfProcessing(
        ITelegramBotClient botClient,
        Message message,
        CancellationToken cancellationToken)
    {
        //getting office documets from user
        var fileId = message.Document.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;

        Directory.CreateDirectory($@".\Files");
        Directory.CreateDirectory($@".\Files\Edited");
        string ext = Path.GetExtension(filePath);
        string fileName = FixCyrillic(message.Document
            .FileName
            .Substring(0, message.Document.FileName.IndexOf('.'))) 
            + AddDateTime();
        
        string destinationPath = $@".\Files\{fileName + ext}";
        await using (FileStream fileStream = System.IO.File.OpenWrite(destinationPath))
        {
            await botClient.DownloadFileAsync(filePath, fileStream, cancellationToken);
        }

        string editedDestinationPath = $@".\Files\Edited";
        FileConverting(destinationPath, editedDestinationPath);

        //sending ready pdf file to 
        await using (Stream stream = System.IO.File.OpenRead(editedDestinationPath + $@"\{fileName}.pdf"))
        {
            await botClient.SendDocumentAsync(
                message.Chat.Id,
                InputFile.FromStream(stream, $"{fileName}.pdf"));
        }
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
            if(ConvertedLetters.ContainsKey(letter))
                result.Append(ConvertedLetters[letter]);
            else
                result.Append(letter);
        }
        return result.ToString();
    }

    void FileConverting(string srcPath, string destinationPath)
    {
        var api = new LovePdfApi(_options.PublicKey, _options.SecretKey);
        var task = api.CreateTask<OfficeToPdfTask>();
        task.AddFile(srcPath);
        task.Process();
        task.DownloadFile(destinationPath);
    }

    private async Task PdfToDocProcessing(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
        message.Chat.Id,
        "This section is developing yet !!!");
    }
}
