using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TournamentAPI.Config;
using TournamentAPI.Models;
using TournamentAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TournamentAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RegistrationController : ControllerBase
	{
		private readonly LocalDBContext _dbContext;
		private readonly IMapper _mapper;
		private readonly TournamentService _tournamentService;

		public RegistrationController(LocalDBContext dbContext, IMapper mapper, TournamentService tournamentService)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_tournamentService = tournamentService;
		}

		[HttpGet("getRegisteredPlayerList")]
		public async Task<ObjectResult> Get([FromQuery] string tournamentId)
		{
			var tournament = await _tournamentService.GetTournamentAsync(tournamentId);

			if (tournament == null)
				return NotFound(new
				{
					success = false,
					errorCode = ErrorCode.TournamentNotFound,
				});

			if(tournament.status == 0)
			{
				var result = from r in _dbContext.tournament_registration
							 where r.tournamentId == tournamentId
							 orderby r.sequence descending
							 join u in _dbContext.users
							 on r.playerId equals u.id
							 select new
							 {
								 name = r.displayName,
								 r.tournamentId,
								 r.playerId,
								 r.teamId,
								 r.sequence,
								 r.isGuest,
								 u.photoURL
							 };

				return Ok(new
				{
					success = true,
					errorCode = ErrorCode.Success,
					registeredPlayerList = result
				});
			}
			else
			{
				return Conflict(new
				{
					success = false,
					errorCode = ErrorCode.RegistrationAlreadyClosed,
				});
			}
		}
	}
}
