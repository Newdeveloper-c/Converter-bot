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
                new KeyboardButton("📒➡️ Convert from Pdf")
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
                new KeyboardButton("📗 Excel -> Pdf 📒")
            },
            new[]
            {
                new KeyboardButton("📙 PowerPoint -> Pdf 📒")
            },
            new[]
            {
                new KeyboardButton("🏞 Images -> Pdf 📒")
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
                new KeyboardButton("📒 Pdf -> Excel 📗")
            },
            new[]
            {
                new KeyboardButton("📒 Pdf -> PowerPoint 📙")
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

    private IReplyMarkup? BotTaskButtonMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        new[]
        {
            new[]
            {
                new KeyboardButton("Convert 🔄")
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
