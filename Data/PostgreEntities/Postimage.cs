using System;
using System.Collections.Generic;

namespace FindingPets.Data.PostgreEntities;

public partial class Postimage
{
    public Guid Id { get; set; }

    public string? Imagebase64 { get; set; }

    public Guid Postid { get; set; }

    public virtual Post Post { get; set; } = null!;
}
