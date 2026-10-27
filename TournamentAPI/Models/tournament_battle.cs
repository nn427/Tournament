using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class tournament_battle
{
    /// <summary>
    /// battleId
    /// </summary>
    public string id { get; set; } = null!;

    public string tournamentId { get; set; } = null!;

    /// <summary>
    /// round數從1開始
    /// </summary>
    public int round { get; set; }

    public string roundName { get; set; } = null!;

    public int sequence { get; set; }

    /// <summary>
    /// 能進行對戰的唯一機台
    /// </summary>
    public string machineId { get; set; } = null!;

    public string leftBattleId { get; set; } = null!;

    public string rightBattleId { get; set; } = null!;

    public string homeTeamId { get; set; } = null!;

    public string awayTeamId { get; set; } = null!;

    public string winnerTeamId { get; set; } = null!;

    /// <summary>
    /// home隊贏的set數
    /// </summary>
    public int homeScore { get; set; }

    /// <summary>
    /// away隊贏的set數
    /// </summary>
    public int awayScore { get; set; }

    /// <summary>
    /// battle建立的UTC時間
    /// </summary>
    public DateTime createdAt { get; set; }

    public DateTime? startDate { get; set; }

    public DateTime? endDate { get; set; }

    /// <summary>
    /// 0 =&gt; 等待中, 1 =&gt; 可以允許開打, 2 =&gt; 已結束
    /// </summary>
    public int status { get; set; }

    /// <summary>
    /// 是否棄賽
    /// </summary>
    public bool isAbandon { get; set; }

    /// <summary>
    /// 執行棄賽的玩家id, 與勝負無關, 只要是這場錦標賽的參加者都可以執行棄賽
    /// </summary>
    public string abandonPlayerId { get; set; } = null!;

    public bool isByeGame { get; set; }
}
