using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class TrPasswordReset
{
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }
}
