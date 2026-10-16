using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class tournament_registration
{
    public string tournamentId { get; set; } = null!;

    public string playerId { get; set; } = null!;

    public string teamId { get; set; } = null!;

    /// <summary>
    /// sequence
    /// </summary>
    public int sequence { get; set; }

    public string displayName { get; set; } = null!;

    /// <summary>
    /// 參加者是否是透過網頁訪客報名
    /// </summary>
    public byte isGuest { get; set; }
}
