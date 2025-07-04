using AutomationBeachTenis.Response;
using AutomationBeachTenis.Services.GenericApiService;
using Microsoft.Extensions.Configuration;

namespace AutomationBeachTenis.Services.TournamentService
{
    public class TournamentService : ITournamentService
    {
        private readonly ILogger<TournamentService> _logger;
        private readonly IGenericApiService _genericApiService;
        private readonly IConfiguration _configuration;
        private string FromDateApiITFBeach { get; set; } = string.Empty;
        private string ToDateApiITFBeach { get; set; } = string.Empty;
        private string UrlITFBeachTournament => $"{_configuration["ITFBeach:TournamentListApi"]}&dateFrom={FromDateApiITFBeach}&dateTo={ToDateApiITFBeach}";
        private string UrlITFBeachTournamentOrderOfPlayIdApi => $"{_configuration["ITFBeach:TournamentOrderOfPlayDayIdApi"]}";

        public TournamentService(ILogger<TournamentService> logger,
            IGenericApiService genericApiService,
            IConfiguration configuration)
        {

            _logger = logger;
            _genericApiService = genericApiService;
            _configuration = configuration;
        }


        public async Task<ResponseApiTournamentITFBeachTenis> GetListTournamentOfDayFromGenericApi()
        {
            AdjustDateApiITF();
            _logger.LogInformation($"Iniciando chamada api de torneios ITF Beach tenis em {DateTime.Now}");
            _logger.LogInformation($"Url api de lista de torneios em {UrlITFBeachTournament}");
            var response = await _genericApiService.GetAsync<ResponseApiTournamentITFBeachTenis>(UrlITFBeachTournament);
            _logger.LogInformation($"Chamada com sucesso. Resposta da chamada api de torneios ITF Beach Tenis : {response}");
            if (response == null)
            {
                _logger.LogInformation($"Não foram encontrados torneios para serem realizados no dia de hoje");
                return new ResponseApiTournamentITFBeachTenis();
            }
            _logger.LogInformation($"Chamada para busca da ordem de partida por id dos torneios");
            await GetTournamentOrderOfPlayIdFromGenericApi(response);
            return response;
        }

        private async Task GetTournamentOrderOfPlayIdFromGenericApi(ResponseApiTournamentITFBeachTenis tournamentList)
        {
            var tournamentsToRemove = new List<ResponseTournament>();

            foreach (var tournament in tournamentList.TournamentList)
            {
                var urlOrderOfPlayTournamentAdjust = UrlITFBeachTournamentOrderOfPlayIdApi + tournament.TournamentKey.ToString();
                _logger.LogInformation($"Url api para busca de id da ordem do torneio {urlOrderOfPlayTournamentAdjust}");
                var response = await _genericApiService.GetAsync<List<ResponseTournamentOrderOfPlay>>(urlOrderOfPlayTournamentAdjust);
                if (response != null)
                {
                    var orderOfDayTodayId = response.Where(a => a.PlayDate.Day == DateTime.Now.Day
                                                                && a.PlayDate.Month == DateTime.Now.Month
                                                                && a.PlayDate.Year == DateTime.Now.Year).FirstOrDefault();

                    if (orderOfDayTodayId != null)
                    {
                        _logger.LogInformation($"Torneio {tournament.TournamentName} com ordem de partida id {orderOfDayTodayId.OrderOfPlayDayId}");
                        tournament.OrderOfPlayDayId = orderOfDayTodayId.OrderOfPlayDayId;
                    }
                    else
                    {
                        _logger.LogInformation($"Torneio não possui partida para o dia de hoje");
                        tournamentsToRemove.Add(tournament);
                    }
                }
            }

            foreach (var tournament in tournamentsToRemove)
            {
                tournamentList.TournamentList.Remove(tournament);
            }
        }

        private void AdjustDateApiITF()
        {
            var currentDate = DateTime.Now;
            FromDateApiITFBeach = $"{currentDate.Year}-{currentDate.Month:D2}-{currentDate.Day:D2}";
            ToDateApiITFBeach = $"{currentDate.Year}-{currentDate.Month:D2}-{currentDate.Day:D2}";
        }


    }
}
