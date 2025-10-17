using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class TrUserSession
{
    public string SessionId { get; set; } = null!;

    public int? UserId { get; set; }

    public DateTime? CreateDateTime { get; set; }

    public DateTime? ExpiryDateTime { get; set; }

    public virtual MsUser? User { get; set; }
}
