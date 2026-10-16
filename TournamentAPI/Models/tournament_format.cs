using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class tournament_format
{
    public string tournamentId { get; set; } = null!;

    /// <summary>
    /// 1=&gt;盤，2=&gt;局
    /// </summary>
    public byte setLegType { get; set; }

    public byte setCount { get; set; }

    public byte legCount { get; set; }

    public string gameTypes { get; set; } = null!;

    public byte isHandicap { get; set; }

    /// <summary>
    /// 0-&gt;OIOO, 1-&gt;OIDO, 2-&gt;DIDO...
    /// </summary>
    public byte checkoutType { get; set; }

    /// <summary>
    /// 01遊戲的分數
    /// </summary>
    public byte game01Score { get; set; }

    /// <summary>
    /// 01遊戲專屬, true&gt;三鏢更新分數 false&gt;每鏢更新分數
    /// </summary>
    public byte isProMode { get; set; }

    /// <summary>
    /// 每隊幾個玩家
    /// </summary>
    public byte teamPlayer { get; set; }

    public DateTime createdAt { get; set; }
}
