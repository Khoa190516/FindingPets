using System;
using System.Collections.Generic;

namespace FindingPets.Data.Entities;

public partial class PostImage
{
    public Guid Id { get; set; }

    public string? ImageBase64 { get; set; }

    public Guid PostId { get; set; }

    public virtual Post Post { get; set; } = null!;
}
