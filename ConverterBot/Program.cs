using ConverterBot;
using ConverterBot.Handlers;
using ConverterBot.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;

const string BotKeyConfigKey = "Botkey";
const string LovePdfConfigKeys = "LovePdf";
var builder = WebApplication.CreateBuilder(args);

var token  = builder.Configuration.GetValue(BotKeyConfigKey, "");

builder.Services.Configure<LovePdfOptions>(builder.Configuration.GetSection(LovePdfConfigKeys));
builder.Services.AddSingleton<ITelegramBotClient>(pr => new TelegramBotClient(token!));
builder.Services.AddSingleton<IUpdateHandler, BotUpdateHandler>();
builder.Services.AddHostedService<BotBackgroundService>();

var app = builder.Build();

app.Run();
