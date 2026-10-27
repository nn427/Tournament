namespace TournamentAPI.DTOs
{
	public class RegisterRequest
	{
		public string TournamentId { get; set; }
		public string PlayerId { get; set; }
		public string DisplayName { get; set; }
	}
}
