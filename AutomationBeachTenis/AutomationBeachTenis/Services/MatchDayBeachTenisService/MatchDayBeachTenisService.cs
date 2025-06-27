using System.Text;
using AutomationBeachTenis.Response;
using AutomationBeachTenis.Services.GenericApiService;
using AutomationBeachTenis.Services.TelegramService;
using AutomationBeachTenis.Services.TournamentService;

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

            if (tournamentList != null)
            {
                foreach (var tournament in tournamentList)
                {
                    _logger.LogInformation($"Processando torneio: {tournament.TournamentName} - {tournament.Category}");
                    var message = await ProcessTournamentMatches(tournament);
                    if(message != null)
                    {
                       await _telegramService.SendMessageTelegramToChannel(message);
                    }
                }
            }
        }

        private async Task<StringBuilder> ProcessTournamentMatches(ResponseTournament tournament)
        {
            var matchCourtList = await GetMatchDayTournamentWTAFromGenericApi(tournament);
            var message = new StringBuilder();
            if (matchCourtList.Any())
            {
                message.AppendLine(tournament.PromotionalName);
                message.AppendLine(tournament.TournamentName);
                message.AppendLine($"{tournament.Location}, {tournament.HostNation}  --  Prize Money: {tournament.PrizeMoney}");
                message.AppendLine($"Jogos do dia de {DateTime.Now:dd/MM/yyyy}");
                message.AppendLine();
            }
            bool firstPlayer = true;
            int teamSequence = 1;

            foreach (var matchCourt in matchCourtList)
            {
                message.AppendLine($"Quadra: {matchCourt.CourtName}");
                foreach (var match in matchCourt.Matches)
                {
                    message.AppendLine();
                    message.AppendLine("--------------------------");
                    message.AppendLine($"{match.RoundGroupDesc}, {match.EventDesc}");
                    message.AppendLine($"{match.Schedule}");
                    teamSequence = 1;
                    foreach (var team in match.Teams)
                    {
                        firstPlayer = true;
                        foreach (var player in team.Players)
                        {
                            message.AppendLine($"{player.Nationality} {player.GivenName} {player.FamilyName} {(team.IsWinner && firstPlayer ? "✔️" : "")}   {(firstPlayer ? string.Join(" ", team.Scores.Select(a => a?.ScoreValue)) : "")}");
                            firstPlayer = false;
                        }
                        if(teamSequence == 1)
                        {
                            message.AppendLine("VS");
                        }
                        teamSequence++;
                    }
                    
                }
            }
            return message;
        }


        private async Task<List<ResponseApiMatchITFBeachTenisCourt>> GetMatchDayTournamentWTAFromGenericApi(ResponseTournament tournament)
        {
            var urlITFMatchAdjust = UrlITFBeachMatch + tournament.OrderOfPlayDayId.ToString();
            _logger.LogInformation($"Iniciando chamada API de partidas WTA em {DateTime.Now}");
            var response = await _genericApiService.GetAsync<List<ResponseApiMatchITFBeachTenisCourt>>(urlITFMatchAdjust);
            _logger.LogInformation($"Chamada com sucesso. Resposta da chamada API de partidas no dia: {response}");
            if(response == null)
            {
                return new List<ResponseApiMatchITFBeachTenisCourt>();
           }
            return response;
        }

    }
}
