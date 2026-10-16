using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class store
{
    public string id { get; set; } = null!;

    public string distributorId { get; set; } = null!;

    public string name { get; set; } = null!;
}
