using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class machine
{
    public string id { get; set; } = null!;

    public string macAddress { get; set; } = null!;

    public string storeId { get; set; } = null!;
}
