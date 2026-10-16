using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class tournament_machine
{
    public string tournamentId { get; set; } = null!;

    public string machineId { get; set; } = null!;

    /// <summary>
    /// 機台指定的順序,從0開始
    /// </summary>
    public byte sequence { get; set; }
}
