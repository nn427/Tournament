using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class tournament
{
    public string id { get; set; } = null!;

    public string tournamentName { get; set; } = null!;

    public string displayName { get; set; } = null!;

    public string distributorId { get; set; } = null!;

    public string storeId { get; set; } = null!;

    /// <summary>
    /// 1-&gt;硬靶, 2-&gt;軟靶
    /// </summary>
    public byte dartType { get; set; }

    /// <summary>
    /// 0-&gt;已建立但未報名完成，1-&gt;比賽期間，2-&gt;比賽結束
    /// </summary>
    public byte status { get; set; }

    public byte isDeleted { get; set; }

    /// <summary>
    /// 比賽建立的local日期
    /// </summary>
    public string tournamentDate { get; set; } = null!;

    /// <summary>
    /// 同一天同一店家建立比賽的序號, 從1開始
    /// </summary>
    public byte dailySequence { get; set; }

    /// <summary>
    /// 比賽建立時間UTC
    /// </summary>
    public DateTime createdAt { get; set; }

    /// <summary>
    /// 比賽結束時間UTC-&gt;冠軍出來才算比賽結束
    /// </summary>
    public DateTime? endDate { get; set; }
}
