using System;
using System.Collections.Generic;

namespace FindingPets.Data.Entities;

public partial class Post
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public DateTime? Created { get; set; }

    public string? Contact { get; set; }

    public bool? IsBanned { get; set; }

    public bool? IsClosed { get; set; }

    public Guid OwnerId { get; set; }

    public string? Title { get; set; }

    public virtual AuthenUser Owner { get; set; } = null!;

    public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();
}
