using System.Text;
using AutomationBeachTenis.Response;
using AutomationBeachTenis.Services.GenericApiService;
using AutomationBeachTenis.Services.TournamentService;
using Telegram.Bot;

namespace AutomationBeachTenis.Services.TelegramService
{
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly IGenericApiService _genericApiService;
        private readonly IConfiguration _configuration;
        public TelegramBotClient _telegramBotClient;
        private string UrlTelegram => $"{_configuration["Telegram:SendMessageApi"]}{_configuration["Telegram:BotToken"]}/sendMessage";

        public TelegramService(ILogger<TelegramService> logger,
            IGenericApiService genericApiService,
            ITournamentService tournamentService,
            IConfiguration configuration)
        {

            _logger = logger;
            _genericApiService = genericApiService;
            _configuration = configuration;
            _telegramBotClient = new TelegramBotClient(_configuration["Telegram:BotToken"]);
        }

        public async Task SendMessageTelegramToChannel(StringBuilder message)
        {
            var fullMessage = message.ToString();
            const int maxLength = 4000;

            for (int i = 0; i < fullMessage.Length; i += maxLength)
            {
                var chunk = fullMessage.Substring(i, Math.Min(maxLength, fullMessage.Length - i));
                await SendMessage(chunk);
            }
        }

        private async Task SendMessage(string chunk)
        {
            await _telegramBotClient.SendMessage(_configuration["Telegram:ChatId"], chunk);
        }

    }
}
