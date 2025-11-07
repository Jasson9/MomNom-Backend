using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class MsVersion
{
    public int VersionId { get; set; }

    public string Version { get; set; } = null!;

    public string VersionChannel { get; set; } = null!;

    public string? Changelogs { get; set; }

    public string? DownloadLink { get; set; }

    public DateTime? CreatedDatetime { get; set; }
}
