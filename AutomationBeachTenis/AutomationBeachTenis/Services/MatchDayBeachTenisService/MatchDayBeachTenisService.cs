using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using AutomationBeachTenis.Response;
using AutomationBeachTenis.Services.GenericApiService;
using AutomationBeachTenis.Services.TelegramService;
using AutomationBeachTenis.Services.TournamentService;
using CoreHtmlToImage;
using Telegram.Bot.Types;

namespace AutomationBeachTenis.Services.MatchDayBeachTenisService
{
    public class MatchDayBeachTenisService : IMatchDayBeachTenisService
    {
        private readonly ILogger<MatchDayBeachTenisService> _logger;
        private readonly IGenericApiService _genericApiService;
        private readonly ITournamentService _tournamentService;
        private readonly ITelegramService _telegramService;
        private readonly IConfiguration _configuration;
        private string UrlITFBeachMatch => $"{_configuration["ITFBeach:TournamentOrderOfPlayApi"]}";

        public MatchDayBeachTenisService(ILogger<MatchDayBeachTenisService> logger,
            IGenericApiService genericApiService,
            ITournamentService tournamentService,
            ITelegramService telegramService,
            IConfiguration configuration)
        {

            _logger = logger;
            _genericApiService = genericApiService;
            _tournamentService = tournamentService;
            _telegramService = telegramService;
            _configuration = configuration;
        }


        public async Task SendMatchListOfDayToTelegramChanel()
        {
            
            var responseTournamentList = await _tournamentService.GetListTournamentOfDayFromGenericApi();
            var tournamentList = responseTournamentList.TournamentList;
            
            if (tournamentList.Any())
            {
                foreach (var tournament in tournamentList)
                {
                    _logger.LogInformation($"Processando torneio: {tournament.TournamentName} - {tournament.Category}");
                    await ProcessTournamentMatches(tournament);
                }
            }
            else
            {
                var message = new StringBuilder();
                message.AppendLine($"🥱 <b>Nenhuma partida para o dia de hoje</b>");
                await _telegramService.SendMessageTelegramToChannel(message);
            }
        }

        private async Task ProcessTournamentMatches(ResponseTournament tournament)
        {
            var matchCourtList = await GetMatchDayTournamentITFFromGenericApi(tournament);
            var message = new StringBuilder();
            if (matchCourtList.Any())
            {
                message.AppendLine($"🏖️ {tournament.PromotionalName} 🎾");
                message.AppendLine($"<b> {tournament.TournamentName} </b>");
                message.AppendLine($"📍 {tournament.Location}, {tournament.HostNation} ");
                message.AppendLine($"💵 Prize Money: <i>{tournament.PrizeMoney}</i>");
                message.AppendLine($"📅 Jogos de {DateTime.Now:dd/MM/yyyy}");
                await _telegramService.SendMessageTelegramToChannel(message);
            }

            foreach (var matchCourt in matchCourtList)
            {
                foreach (var match in matchCourt.Matches)
                {
                    var html = new StringBuilder();
                    html.AppendLine("<!DOCTYPE html>");
                    html.AppendLine("<html lang=\"pt-br\">");
                    html.AppendLine("<head>");
                    html.AppendLine("  <meta charset=\"UTF-8\" />");
                    html.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
                    html.AppendLine("  <title>Placar Beach Tennis</title>");
                    html.AppendLine("  <style>");
                    html.AppendLine("    body { font-family: Arial, sans-serif; background: #f9fafb; padding: 20px; }");
                    html.AppendLine("    .match-container { max-width: 450px; background: #fff; border: 1px solid #ddd; border-radius: 10px; padding: 15px 20px; margin-bottom: 30px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }");
                    html.AppendLine("    .match-header { margin-bottom: 10px; }");
                    html.AppendLine("    .match-header h3 { margin: 0; font-size: 16px; color: #333; }");
                    html.AppendLine("    .match-header p { margin: 2px 0; font-size: 14px; color: #666; }");
                    html.AppendLine("    .team { margin-bottom: 12px; overflow: hidden; }");
                    html.AppendLine("    .avatars { margin-right: 10px; float: left; }");
                    html.AppendLine("    .avatar { width: 40px; height: 40px; border-radius: 50%; overflow: hidden; border: 2px solid #ccc; display: inline-block; margin-right: 5px; }");
                    html.AppendLine("    .team.winner .avatar { border-color: #28a745; }");
                    html.AppendLine("    .avatar img { width: 100%; height: 100%; object-fit: cover; display: block; }");
                    html.AppendLine("    .player-info { overflow: hidden; margin-left: 60px; }");
                    html.AppendLine("    .player-name { font-weight: 600; font-size: 14px; margin-bottom: 2px; max-width: 150px; word-break: break-word; white-space: normal; }");
                    html.AppendLine("    .team.winner .player-name { color: #28a745; }");
                    html.AppendLine("    .flag-name { white-space: nowrap; }");
                    html.AppendLine("    .flag { width: 20px; height: 14px; border-radius: 3px; border: 1px solid #ccc; vertical-align: middle; margin-right: 6px; }");
                    html.AppendLine("    .ranking { color: #555; font-size: 12px; }");
                    html.AppendLine("    .scores { font-family: 'Courier New', Courier, monospace; font-weight: bold; font-size: 16px; min-width: 70px; text-align: right; float: right; }");
                    html.AppendLine("    .score { display: inline-block; width: 18px; text-align: center; margin-left: 8px; }");
                    html.AppendLine("    .vs { text-align: center; margin: 10px 0; font-weight: bold; color: #555; clear: both; }");
                    html.AppendLine("  </style>");
                    html.AppendLine("</head>");
                    html.AppendLine("<body>");
                    html.AppendLine("<div class=\"match-container\">");
                    html.AppendLine($"  <div class=\"match-header\">");
                    html.AppendLine($"    <h3>{matchCourt.CourtName}</h3>");
                    html.AppendLine($"    <p>{match.RoundGroupDesc} - {match.EventDesc}</p>");
                    html.AppendLine($"    <p>{match.Schedule}</p>");
                    html.AppendLine($"  </div>");
                    foreach (var team in match.Teams)
                    {
                        var isWinnerClass = team.IsWinner ? "team winner" : "team";
                        html.AppendLine($"  <div class=\"{isWinnerClass}\">");
                        html.AppendLine("    <div class=\"avatars\">");
                        foreach (var player in team.Players)
                        {
                            html.AppendLine($"      <div class=\"avatar\"><img src=\"https://i.pravatar.cc/40?u={player.GivenName}\" /></div>");
                        }
                        html.AppendLine("    </div>");
                        html.AppendLine("    <div class=\"player-info\">");
                        foreach (var player in team.Players)
                        {
                            html.AppendLine("      <div class=\"player-name\">");
                            html.AppendLine($"        <span class=\"flag-name\"><img class=\"flag\" src=\"https://flagcdn.com/w20/{ToFlagString(player.Nationality.ToLower())}.png\" /> {ToTitleCase(player.GivenName)} {ToTitleCase(player.FamilyName)}</span>");
                            html.AppendLine("        <span class=\"ranking\"></span>");
                            html.AppendLine("      </div>");
                        }
                        html.AppendLine("    </div>");
                        html.AppendLine("    <div class=\"scores\">");
                        if (team.IsWinner)
                        {
                            html.AppendLine("      <svg fill=\"#42c328\" width=\"24px\" height=\"24px\" viewBox=\"0 0 1920 1920\" xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M1827.701 303.065 698.835 1431.801 92.299 825.266 0 917.564 698.835 1616.4 1919.869 395.234z\" fill-rule=\"evenodd\"></path></svg>");
                        }
                        foreach (var score in team.Scores.Where(a => a != null))
                        {
                            html.AppendLine($"      <div class=\"score\">{score?.ScoreValue}</div>");
                        }
                        html.AppendLine("    </div>");
                        html.AppendLine("  </div>");
                    }
                    html.AppendLine("</div>");
                    html.AppendLine("</body>");
                    html.AppendLine("</html>");

                    await ConvertHTMLDetailMatchToPhotoAndSendToTelegram(html.ToString(), match.MatchDescription);
                }
            }

        }

        private async Task<List<ResponseApiMatchITFBeachTenisCourt>> GetMatchDayTournamentITFFromGenericApi(ResponseTournament tournament)
        {
            var urlITFMatchAdjust = UrlITFBeachMatch + tournament.OrderOfPlayDayId.ToString();
            _logger.LogInformation($"Iniciando chamada API de partidas ITF beach tenis em {DateTime.Now}");
            var response = await _genericApiService.GetAsync<List<ResponseApiMatchITFBeachTenisCourt>>(urlITFMatchAdjust);
            _logger.LogInformation($"Chamada com sucesso. Resposta da chamada API de partidas no dia: {response}");
            if(response == null)
            {
                return new List<ResponseApiMatchITFBeachTenisCourt>();
           }
            return response;
        }

        private async Task ConvertHTMLDetailMatchToPhotoAndSendToTelegram(string html, string matchDescription)
        {
            var converter = new HtmlConverter();
            var bytes = converter.FromHtmlString(html,460);
            using var ms = new MemoryStream(bytes);
            string typePlay = $"<b>Play</b> {(matchDescription == "WD" ? "Feminino" : "Masculino")}";
            await _telegramService.SendPhotoTelegramToChannel(ms, typePlay);
        }

        private string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        private string ToFlagString(string str)
        {
            return str.Length > 2 ? str.Substring(0, 2) : str;
        }

    }
}
