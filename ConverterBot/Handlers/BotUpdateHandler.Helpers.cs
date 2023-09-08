using System.Text;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
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

    private void DeleteTemps()
    {
        if (currentProcessingFilePath != string.Empty)
        {
            System.IO.File.Delete(currentProcessingFilePath);
            currentProcessingFilePath = string.Empty;
            currentFileName = string.Empty;
        }

        if (currentProcessingImagesFolder != string.Empty)
        {
            Directory.Delete(currentProcessingImagesFolder, true);
            currentProcessingImagesFolder = string.Empty;
        }
    }
}
