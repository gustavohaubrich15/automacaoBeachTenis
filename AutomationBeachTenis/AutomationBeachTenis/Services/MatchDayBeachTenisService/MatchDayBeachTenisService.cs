using AutomationBeachTenis.Response;
using AutomationBeachTenis.Services.GenericApiService;
using AutomationBeachTenis.Services.TournamentService;

namespace AutomationBeachTenis.Services.MatchDayBeachTenisService
{
    public class MatchDayBeachTenisService : IMatchDayBeachTenisService
    {
        private readonly ILogger<MatchDayBeachTenisService> _logger;
        private readonly IGenericApiService _genericApiService;
        private readonly ITournamentService _tournamentService;
        private readonly IConfiguration _configuration;
        private string UrlITFBeachMatch => $"{_configuration["ITFBeach:TournamentOrderOfPlayApi"]}";

        public MatchDayBeachTenisService(ILogger<MatchDayBeachTenisService> logger,
            IGenericApiService genericApiService,
            ITournamentService tournamentService,
            IConfiguration configuration)
        {

            _logger = logger;
            _genericApiService = genericApiService;
            _tournamentService = tournamentService;
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
                    await ProcessTournamentMatches(tournament);
                }
            }
        }

        private async Task ProcessTournamentMatches(ResponseTournament tournament)
        {
            var matchCourtList = await GetMatchDayTournamentWTAFromGenericApi(tournament);

            if (matchCourtList.Any())
            {
                _logger.LogInformation(tournament.PromotionalName);
                _logger.LogInformation(tournament.TournamentName);
                _logger.LogInformation($"{tournament.Location}, {tournament.HostNation}  --  Prize Money: {tournament.PrizeMoney} ");
                _logger.LogInformation($"Jogos do dia de {DateTime.Now.ToString("dd/mm/yyyyy")}");
            }
            bool firstPlayer = true;
            int teamSequence = 1;

            foreach (var matchCourt in matchCourtList)
            {
                _logger.LogInformation("");
                _logger.LogInformation(matchCourt.CourtName);
                foreach (var match in matchCourt.Matches)
                {
                    _logger.LogInformation("");
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation($"{match.RoundGroupDesc}, {match.EventDesc}");
                    _logger.LogInformation($"{match.Schedule}");
                    teamSequence = 1;
                    foreach (var team in match.Teams)
                    {
                        firstPlayer = true;
                        foreach (var player in team.Players)
                        {
                            _logger.LogInformation($"{player.Nationality} {player.GivenName} {player.FamilyName} {(team.IsWinner && firstPlayer ? "✔️" : "")}   {(firstPlayer ? string.Join(" ", team.Scores.Select(a => a?.ScoreValue)) : "")}");
                            firstPlayer = false;
                        }
                        if(teamSequence == 1)
                        {
                            _logger.LogInformation("VS");
                        }
                        teamSequence++;
                    }
                    
                }
            }
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
