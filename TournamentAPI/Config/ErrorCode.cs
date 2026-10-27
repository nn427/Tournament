namespace TournamentAPI.Config
{
	public static class ErrorCode
	{
		public const string Success = "E00000";
		public const string DistributorNotFound = "E00001";
		public const string StoreNotFound = "E00002";
		public const string CreateTournamentFailed = "E00003";
		public const string TournamentNotFound = "E00004";
		public const string RegistrationAlreadyClosed = "E00005";
		public const string RegistrationAlreadyFull = "E00006";
		public const string RegisterFailed = "E00007";
		public const string CompleteRegistrationFailed = "E00008";
		public const string IncorrectRegisteredPlayerCount = "E00009";
	}
}
