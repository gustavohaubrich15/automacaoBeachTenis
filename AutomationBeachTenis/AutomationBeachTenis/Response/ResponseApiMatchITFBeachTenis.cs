using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AutomationBeachTenis.Response
{
   
    public class ResponseApiMatchITFBeachTenisCourt
    {
        [JsonPropertyName("courtName")]
        public string CourtName { get; set; } = string.Empty;

        [JsonPropertyName("matches")]
        public List<ResponseMatch> Matches { get; set; } = new List<ResponseMatch>();
    }

    public class ResponseMatch
    {
        [JsonPropertyName("schedule")]
        public string Schedule { get; set; } = string.Empty;

        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;

        [JsonPropertyName("round")]
        public string Round { get; set; } = string.Empty;

        [JsonPropertyName("matchNumber")]
        public int MatchNumber { get; set; }

        [JsonPropertyName("matchDescription")]
        public string MatchDescription { get; set; } = string.Empty;

        [JsonPropertyName("roundGroupDesc")]
        public string RoundGroupDesc { get; set; } = string.Empty;

        [JsonPropertyName("eventDesc")]
        public string EventDesc { get; set; } = string.Empty;

        [JsonPropertyName("classification")]
        public string Classification { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("matchId")]
        public long MatchId { get; set; }

        [JsonPropertyName("teams")]
        public List<ResponseTeam> Teams { get; set; } = new List<ResponseTeam>();
    }

    public class ResponseTeam
    {
        [JsonPropertyName("players")]
        public List<ResponsePlayer> Players { get; set; } = new List<ResponsePlayer>();

        [JsonPropertyName("scores")]
        public List<ResponseScore?> Scores { get; set; } = new List<ResponseScore?>();

        [JsonPropertyName("isWinner")]
        public bool IsWinner { get; set; }

        [JsonPropertyName("seeding")]
        public int? Seeding { get; set; }
    }

    public class ResponseScore
    {
        [JsonPropertyName("score")]
        public int? ScoreValue { get; set; }

        [JsonPropertyName("losingScore")]
        public int? LosingScore { get; set; }
    }

    public class ResponsePlayer
    {
        [JsonPropertyName("playerId")]
        public long PlayerId { get; set; }

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; } = string.Empty;

        [JsonPropertyName("familyName")]
        public string FamilyName { get; set; } = string.Empty;

        [JsonPropertyName("nationality")]
        public string Nationality { get; set; } = string.Empty;

        [JsonPropertyName("profileLink")]
        public string ProfileLink { get; set; } = string.Empty;
    }
}
