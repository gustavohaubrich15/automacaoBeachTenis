using AutomationBeachTenis.Response;

namespace AutomationBeachTenis.Services.TournamentService
{
    public interface ITournamentService
    {
        Task<ResponseApiTournamentITFBeachTenis> GetListTournamentOfDayFromGenericApi();
    }
}
