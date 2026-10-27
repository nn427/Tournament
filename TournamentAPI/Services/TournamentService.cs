using AutoMapper;
using TournamentAPI.Config;
using TournamentAPI.DTOs;
using TournamentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TournamentAPI.Services
{
	public class TournamentService
	{
		private readonly LocalDBContext _dbContext;
		private readonly IMapper _mapper;

		public TournamentService(LocalDBContext dbContext, IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		public async Task<GetTournamentResponse?> GetTournamentAsync(string tournamentId)
		{
			var query = from t in _dbContext.tournament
						join f in _dbContext.tournament_format
						on t.id equals f.tournamentId
						join s in _dbContext.store
						on t.storeId equals s.id
						where t.id == tournamentId
						select new
						{
							Tournament = t,
							Format = f,
							StoreName = s.name
						};

			var data = await query.FirstOrDefaultAsync();
			if (data == null)
				return null;

			var result = _mapper.Map<GetTournamentResponse>((data.Tournament, data.Format));
			result.StoreName = data.StoreName;

			return result;
		}

		public async Task<string[]> GetTournamentMachineList(string tournamentId)
		{
			var query = from m in _dbContext.tournament_machine
						where m.tournamentId == tournamentId
						orderby m.sequence
						select m.machineId;

			return await query.ToArrayAsync();
		}
	}
}
