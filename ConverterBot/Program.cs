using ConverterBot;
using ConverterBot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;

const string BotKeyConfigKey = "Botkey";

var builder = WebApplication.CreateBuilder(args);

var token  = builder.Configuration.GetValue(BotKeyConfigKey, "");

builder.Services.AddSingleton<ITelegramBotClient>(pr => new TelegramBotClient(token!));
builder.Services.AddSingleton<IUpdateHandler, BotUpdateHandler>();
builder.Services.AddHostedService<BotBackgroundService>();

var app = builder.Build();

app.Run();
