using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class tournament_team
{
    public string id { get; set; } = null!;

    public string tournamentId { get; set; } = null!;

    /// <summary>
    /// 報名的順序, 和比賽排序無關, 從0開始
    /// </summary>
    public int sequence { get; set; }

    public DateTime createdAt { get; set; }
}
