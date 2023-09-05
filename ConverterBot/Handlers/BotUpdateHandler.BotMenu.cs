using Telegram.Bot.Types.ReplyMarkups;

namespace ConverterBot.Handlers;

public partial class BotUpdateHandler
{
    public IReplyMarkup? BotMainMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        new[]
        {
            new[]
            {
                new KeyboardButton("Convert to Pdf ➡️📒")
            },
            new[]
            {
                new KeyboardButton("📒➡️ Convert Pdf to")
            },
            new[]
            {
                new KeyboardButton("Help 🆘")
            }
        })
        {
            ResizeKeyboard = true
        };

        return replyKeyboard;
    }

    public IReplyMarkup? BotConvertToPdfMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        new[]
        {
            new[]
            {
                new KeyboardButton("📘 Word -> Pdf 📒")
            },
            new[]
            {
                new KeyboardButton("📗 Exsel -> Pdf 📒")
            },
            new[]
            {
                new KeyboardButton("📙 PowerPoint -> Pdf 📒")
            },
            new[]
            {
                new KeyboardButton("Go back ⬅️")
            }
        })
        {
            ResizeKeyboard = true
        };

        return replyKeyboard;
    }

    public IReplyMarkup? BotConvertPdfToMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        new[]
        {
            new[]
            {
                new KeyboardButton("📒 Pdf -> Word 📘")
            },
            new[]
            {
                new KeyboardButton("Go back ⬅️")
            }
        })
        {
            ResizeKeyboard = true
        };

        return replyKeyboard;
    }

    private IReplyMarkup? BotBackButtonMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        new[]
        {
            new[]
            {
                new KeyboardButton("Go back ⬅️")
            }
        })
        {
            ResizeKeyboard = true
        };

        return replyKeyboard;
    }
}
