using System;
using System.Collections.Generic;

namespace TournamentAPI.Models;

public partial class users
{
    public string id { get; set; } = null!;

    public string displayId { get; set; } = null!;

    public string name { get; set; } = null!;

    public string nickname { get; set; } = null!;

    public string photoURL { get; set; } = null!;

    public string nation { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password { get; set; } = null!;
}
