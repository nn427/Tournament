namespace TournamentAPI.DTOs
{
	public class GetTournamentResponse
	{
		public string id { get; set; } = null!;
		public string tournamentName { get; set; } = null!;
		public string displayName { get; set; } = string.Empty;
		public string distributorId { get; set; } = null!;
		public string storeId { get; set; } = null!;
		public byte dartType { get; set; }
		public byte status { get; set; }
		public byte isDeleted { get; set; }
		public string tournamentDate { get; set; } = null!;
		public byte dailySequence { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime? endDate { get; set; }
		public string tournamentId { get; set; } = null!;
		public byte setLegType { get; set; }
		public byte setCount { get; set; }
		public byte legCount { get; set; }
		public string gameTypes { get; set; } = null!;
		public bool isHandicap { get; set; }
		public byte checkoutType { get; set; }
		public short game01Score { get; set; }
		public bool IsProMode { get; set; }
		public byte TeamPlayer { get; set; }
		public string StoreName { get; set; } = null!;
	}
}
