using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class MsUser
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<MsPlan> MsPlans { get; set; } = new List<MsPlan>();

    public virtual ICollection<TrUserSession> TrUserSessions { get; set; } = new List<TrUserSession>();
}
