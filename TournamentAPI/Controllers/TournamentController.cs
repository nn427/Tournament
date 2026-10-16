using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TournamentAPI.Config;
using TournamentAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TournamentAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TournamentController : ControllerBase
	{
		private readonly LocalDBContext _dbContext;

		public TournamentController(LocalDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet("getTournamentOverview")]
		public ObjectResult Get([FromQuery] string storeId, [FromQuery] int dartType, [FromQuery] string machineId)
		{
			var distributorId = _dbContext.store
				.Where(s => s.id == storeId)
				.Select(s => s.distributorId)
				.FirstOrDefault();

			if (string.IsNullOrEmpty(distributorId))
				return NotFound(new
				{
					success = false,
					errorCode = ErrorCode.DistributorNotFound
				});

			var tournamentResult = (from t in _dbContext.tournament
								   join m in _dbContext.tournament_machine
								   on t.id equals m.tournamentId
								   where m.machineId == machineId
								   && t.isDeleted == 0
								   && t.dartType == dartType
								   && t.distributorId == distributorId
								   && t.storeId == storeId
								   && (t.status != 2 || t.endDate.Value.AddDays(15) >= DateTime.UtcNow)
								   orderby t.createdAt descending
								   select new
								   {
									   tournamentId = t.id,
									   t.tournamentName,
									   t.displayName,
									   t.status,
									   t.dailySequence,
									   t.createdAt,
									   newEndDate = t.endDate
								   }).ToList();

			return Ok(new
			{
				success = true,
				errorCode = ErrorCode.Success,
				data = tournamentResult
			});
		}

		// POST api/<ValuesController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}
	}
}
