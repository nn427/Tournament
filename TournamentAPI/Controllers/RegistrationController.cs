using System.Text.Json;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentAPI.Config;
using TournamentAPI.DTOs;
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

			if (tournament.status == 0)
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

		[HttpPost("register")]
		public async Task<ObjectResult> Post([FromBody] RegisterRequest request)
		{
			await using var transaction = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				//todo 要換成能lock的防止同時報名的race
				var tournament = await _tournamentService.GetTournamentAsync(request.TournamentId);

				if (tournament == null)
					return NotFound(new
					{
						success = false,
						errorCode = ErrorCode.TournamentNotFound,
					});

				if (tournament.status == 0)
				{
					var machineList = await _tournamentService.GetTournamentMachineList(request.TournamentId);
					if (machineList.Length == 0)
						return NotFound(new
						{
							success = false,
							errorCode = ErrorCode.TournamentNotFound,
						});

					// 要先知道這場限幾人報名
					int maxPlayer = machineList.Length * tournament.TeamPlayer * GameDefine.TournamentMaxTeamCount;

					// 得先拿到已報名清單才能知道sequence
					// 從資料庫拿真正的資料, 比較保險
					var registeredPlayer = await (from r in _dbContext.tournament_registration
												  where r.tournamentId == request.TournamentId
												  orderby r.sequence descending
												  select new
												  {
													  r.playerId,
													  r.teamId,
													  r.sequence
												  }).ToListAsync();
					int sequence = registeredPlayer.Count;
					// 報名人數超過限制
					if (sequence >= maxPlayer)
						return Conflict(new
						{
							success = false,
							errorCode = ErrorCode.RegistrationAlreadyFull,
						});

					string teamId = string.Empty;
					if (sequence % tournament.TeamPlayer == 0)
					{
						// 下一隊報名, 給新的teamId
						Guid teamGuid = await _dbContext.Database
							.SqlQuery<Guid>($"SELECT NEWID() AS Value")
							.SingleAsync();
						teamId = teamGuid.ToString();
					}
					else
					{
						// 仍是同一隊, 用最新報名者的teamId
						teamId = registeredPlayer.Last().teamId;
					}

					_dbContext.tournament_registration.Add(new tournament_registration
					{
						tournamentId = request.TournamentId,
						playerId = request.PlayerId,
						displayName = request.DisplayName,
						isGuest = false,
						sequence = sequence,
						teamId = teamId
					});
					int affectedRows = await _dbContext.SaveChangesAsync();

					if (affectedRows == 1)
					{
						await transaction.CommitAsync();

						return Ok(new
						{
							success = true,
							errorCode = ErrorCode.Success
						});
					}
					else
					{
						// 大多是因為playerId重複登入
						return Conflict(new
						{
							success = false,
							errorCode = ErrorCode.RegisterFailed,
						});
					}
				}
				else
				{
					// 過報名階段
					return Conflict(new
					{
						success = false,
						errorCode = ErrorCode.RegistrationAlreadyClosed,
					});
				}
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				Console.WriteLine(e.StackTrace);
				return BadRequest(new
				{
					success = false,
					errorCode = ErrorCode.RegisterFailed,
					data = request
				});
			}
		}

		[HttpPost("completeRegistration")]
		public async Task<ObjectResult> Post([FromBody] string tournamentId)
		{
			await using var transaction = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				// step0-1. 撈出tournament相關設定, todo使用會lock的查詢
				var tournament = await _tournamentService.GetTournamentAsync(tournamentId);
				if (tournament == null)
					return NotFound(new
					{
						success = false,
						errorCode = ErrorCode.TournamentNotFound,
					});
				// step0-2. 檢查tournamentstatus
				if (tournament.status != 0)
					return Conflict(new
					{
						success = false,
						errorCode = ErrorCode.RegistrationAlreadyClosed
					});
				// step0-3. 撈出registration名單
				var registeredPlayerList = await (from r in _dbContext.tournament_registration
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
											}).ToListAsync();
				// step0-4. 檢查registration名單是否符合設定(ex. 隊伍都有滿人 或 隊伍大於最低隊伍數量等等)
				var machineList = await _tournamentService.GetTournamentMachineList(tournamentId);
				if (machineList.Length == 0)
					return NotFound(new
					{
						success = false,
						errorCode = ErrorCode.TournamentNotFound,
					});
				int machineCount = machineList.Length;
				int playerCount = registeredPlayerList.Count;
				int teamPlayer = tournament.TeamPlayer;
				int teamCount = playerCount / teamPlayer;
				int leftBehindPlayer = playerCount % teamPlayer;
				if (teamCount < GameDefine.TournamentMinTeamCount
					|| teamCount > GameDefine.TournamentMaxTeamCount * machineCount
					|| leftBehindPlayer != 0)
					return Conflict(new
					{
						success = false,
						errorCode = ErrorCode.IncorrectRegisteredPlayerCount
					});

				// step1. 將報名名單移至正式的team與team_member表
				// tournament_player_snapshot要存當下的rating, 如果有讓級讓分要參照
				List<string> teamIdList = new List<string>();
				for(int i = 0; i < playerCount; i += teamPlayer)
				{
					string teamId = registeredPlayerList[i].teamId;
					teamIdList.Add(teamId);
					_dbContext.tournament_team.Add(new tournament_team
					{
						id = teamId,
						tournamentId = tournamentId,
						sequence = i / teamPlayer,
						createdAt = DateTime.UtcNow,
					});
					await _dbContext.SaveChangesAsync();
				}

				for(int i = 0; i < playerCount; i++)
				{
					_dbContext.tournament_team_member.Add(new tournament_team_member
					{
						tournamentId = tournamentId,
						teamId = registeredPlayerList[i].teamId,
						playerId = registeredPlayerList[i].playerId,
						sequence = registeredPlayerList[i].sequence,
						displayName = registeredPlayerList[i].name,
						isGuest = registeredPlayerList[i].isGuest
					});
					await _dbContext.SaveChangesAsync();

					//todo snapshot
				}

				await transaction.CommitAsync();

				return Ok(new
				{
					success = true,
					errorCode = ErrorCode.Success
				});
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();

				return BadRequest(new
				{
					success = false,
					errorCode = ErrorCode.CompleteRegistrationFailed,
					data = tournamentId
				});
			}
		}
	}
}
