using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentAPI.Config;
using TournamentAPI.DTOs;
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
					errorCode = ErrorCode.StoreNotFound
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
										t.createdAt
									}).ToList();

			return Ok(new
			{
				success = true,
				errorCode = ErrorCode.Success,
				data = tournamentResult
			});
		}

		// POST api/<ValuesController>
		[HttpPost("createTournament")]
		public async Task<ObjectResult> Post([FromBody] CreateTournamentRequest request)
		{
			var store = _dbContext.store.Where(a => a.id == request.StoreId).FirstOrDefault();
			if (store == null)
				return NotFound(new
				{
					success = false,
					errorCode = ErrorCode.StoreNotFound
				});

			string distributorId = store.distributorId;

			// 0. 先判斷這家店今日(date)有沒有建立過比賽, 然後給正確的序號
			int dailySequence = 1;
			var tournamentResult = _dbContext.tournament
				.Where(a => a.distributorId == distributorId && a.dartType == request.DartType)
				.OrderByDescending(a => a.dailySequence)
				.FirstOrDefault();
			if (tournamentResult != null)
				dailySequence = tournamentResult.dailySequence + 1;

			await using var transaction = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				Guid tournamentId = await _dbContext.Database
				.SqlQuery<Guid>($"SELECT NEWID() AS Value")
				.SingleAsync();
				string tId = tournamentId.ToString();

				// 1. 建立tournament
				_dbContext.tournament.Add(new tournament
				{
					id = tId,
					storeId = request.StoreId,
					distributorId = distributorId,
					tournamentName = request.TournamentName,
					displayName = string.Empty,
					dartType = checked((byte)request.DartType),
					createdAt = request.CreatedAt,
					tournamentDate = request.TournamentDate,
					dailySequence = checked((byte)dailySequence)
				});
				await _dbContext.SaveChangesAsync();

				// 2. 建立使用機台
				var addMachines = new tournament_machine[request.MachineIds.Length];
				for (int i = 0; i < request.MachineIds.Length; i++)
				{
					tournament_machine m = new tournament_machine()
					{
						machineId = request.MachineIds[i],
						sequence = checked((byte)i),
						tournamentId = tId
					};
					addMachines[i] = m;
				}
				_dbContext.tournament_machine.AddRange(addMachines);
				await _dbContext.SaveChangesAsync();

				// 3. 建立format
				_dbContext.tournament_format.Add(new tournament_format
				{
					tournamentId = tId,
					setLegType = checked((byte)request.SetLegType),
					setCount = checked((byte)request.SetCount),
					legCount = checked((byte)request.LegCount),
					gameTypes = JsonSerializer.Serialize(request.GameTypes),
					isHandicap = request.IsHandicap,
					checkoutType = checked((byte)request.CheckoutType),
					game01Score = request.Game01Score,
					isProMode = request.IsProMode,
					teamPlayer = checked((byte)request.TeamPlayer),
					createdAt = request.CreatedAt
				});
				await _dbContext.SaveChangesAsync();

				await transaction.CommitAsync();

				return Ok(new
				{
					success = true,
					errorCode = ErrorCode.Success,
					data = new
					{
						tournamentId = tId,
						tournamentName = request.TournamentName,
						createdAt = request.CreatedAt,
						dailySequence = dailySequence
					}
				});
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();

				return BadRequest(new
				{
					success = false,
					errorCode = ErrorCode.CreateTournamentFailed,
					data = request
				});
			}
		}
	}
}
