using System.Text;
using AutomationBeachTenis.Response;
using AutomationBeachTenis.Services.GenericApiService;
using AutomationBeachTenis.Services.TournamentService;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace AutomationBeachTenis.Services.TelegramService
{
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly IConfiguration _configuration;
        public TelegramBotClient _telegramBotClient;

        public TelegramService(ILogger<TelegramService> logger,
            IGenericApiService genericApiService,
            ITournamentService tournamentService,
            IConfiguration configuration)
        {

            _logger = logger;
            _configuration = configuration;
            _telegramBotClient = new TelegramBotClient(_configuration["Telegram:BotToken"]);
        }

        public async Task SendMessageTelegramToChannel(StringBuilder message)
        {
            await _telegramBotClient.SendMessage(_configuration["Telegram:ChatId"], message.ToString(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        public async Task SendPhotoTelegramToChannel(Stream photoStream, string typePlay)
        {
            await _telegramBotClient.SendPhoto(_configuration["Telegram:ChatId"], photoStream, typePlay, ParseMode.Html);
        }

    }
}
