using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class tournament_sets
{
    public string id { get; set; } = null!;

    public string battleId { get; set; } = null!;

    public int sequence { get; set; }

    /// <summary>
    /// 先攻home/away的index, -1-&gt;未決定, 0-&gt;home, 1-&gt;away
    /// </summary>
    public int firstAttackTeamIndex { get; set; }

    /// <summary>
    /// home隊這個set贏的leg數量
    /// </summary>
    public int homeWinLeg { get; set; }

    /// <summary>
    /// away隊這個set贏的leg數量
    /// </summary>
    public int awayWinLeg { get; set; }

    public string winnerTeamId { get; set; } = null!;

    public DateTime? startDate { get; set; }

    public DateTime? endDate { get; set; }

    /// <summary>
    /// set建立的UTC時間
    /// </summary>
    public DateTime createdAt { get; set; }
}
