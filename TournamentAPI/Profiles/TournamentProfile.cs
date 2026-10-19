using AutoMapper;
using TournamentAPI.DTOs;
using TournamentAPI.Models;

namespace TournamentAPI.Profiles
{
	public class TournamentProfile : Profile
	{
		public TournamentProfile() {
			CreateMap<tournament, GetTournamentResponse>();
			CreateMap<tournament_format, GetTournamentResponse>();
			CreateMap<(tournament Tournament, tournament_format Format), GetTournamentResponse>()
				.IncludeMembers(
					a => a.Tournament,
					a => a.Format);
		}
	}
}
