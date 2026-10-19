namespace TournamentAPI.DTOs
{
	public class CreateTournamentRequest
	{
		public string StoreId { get; set; }
		public int DartType { get; set; }
		public int SetLegType { get; set; }
		public int SetCount { get; set; }
		public int LegCount { get; set; }
		public int[] GameTypes { get; set; }
		public bool IsHandicap { get; set; }
		public short Game01Score { get; set; }
		public int CheckoutType { get; set; }
		public bool IsProMode { get; set; }
		public string[] MachineIds { get; set; }
		public string TournamentName { get; set; }
		public DateTime CreatedAt { get; set; }
		public string TournamentDate { get; set; }
		public int TeamPlayer { get; set; }
	}
}
