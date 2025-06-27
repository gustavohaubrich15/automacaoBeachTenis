using System.Text.Json.Serialization;


namespace AutomationBeachTenis.Response
{
    [Serializable]
    public class ResponseApiTournamentITFBeachTenis
    {
        [JsonPropertyName("items")]
        public List<ResponseTournament> TournamentList { get; set; } = new List<ResponseTournament>();
    }

    [Serializable]
    public class ResponseTournament
    {
        [JsonPropertyName("tournamentName")]
        public string TournamentName { get; set; } = string.Empty;

        [JsonPropertyName("dates")]
        public string Dates { get; set; } = string.Empty;

        [JsonPropertyName("location")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("prizeMoney")]
        public string PrizeMoney { get; set; } = string.Empty;

        [JsonPropertyName("indoorOrOutDoor")]
        public string IndoorOrOutDoor { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("promotionalName")]
        public string PromotionalName { get; set; } = string.Empty;

        [JsonPropertyName("hostNation")]
        public string HostNation { get; set; } = string.Empty;

        [JsonPropertyName("hostNationCode")]
        public string HostNationCode { get; set; } = string.Empty;

        [JsonPropertyName("venue")]
        public string Venue { get; set; } = string.Empty;

        [JsonPropertyName("tournamentKey")]
        public string TournamentKey { get; set; } = string.Empty;

        [JsonPropertyName("tennisCategoryCode")]
        public string TennisCategoryCode { get; set; } = string.Empty;

        [JsonPropertyName("tournamentLink")]
        public string TournamentLink { get; set; } = string.Empty;

        [JsonPropertyName("liveStreamingUrl")]
        public string LiveStreamingUrl { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }

        public long  OrderOfPlayDayId { get; set; }

    }

    [Serializable]
    public class ResponseTournamentOrderOfPlay
    {
        [JsonPropertyName("orderOfPlayDayId")]
        public long OrderOfPlayDayId { get; set; }

        [JsonPropertyName("playDate")]
        public DateTime PlayDate { get; set; }

        [JsonPropertyName("playDateString")]
        public String PlayDateString { get; set; } = string.Empty ;
    }
}
