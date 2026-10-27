using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class tournament_team_member
{
    public string tournamentId { get; set; } = null!;

    /// <summary>
    /// 報名順序
    /// </summary>
    public int sequence { get; set; }

    public string teamId { get; set; } = null!;

    public string playerId { get; set; } = null!;

    /// <summary>
    /// 玩家參加比賽顯示的名稱, 可能是玩家報名時的nickname或是網頁報名自己填入的名稱
    /// </summary>
    public string displayName { get; set; } = null!;

    /// <summary>
    /// 參加者是否是透過網頁訪客報名
    /// </summary>
    public bool isGuest { get; set; }
}
