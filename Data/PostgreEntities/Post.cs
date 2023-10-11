using System;
using System.Collections.Generic;

namespace FindingPets.Data.PostgreEntities;

public partial class Post
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? Created { get; set; }

    public string? Contact { get; set; }

    public bool? Isbanned { get; set; }

    public bool? Isclosed { get; set; }

    public Guid Ownerid { get; set; }

    public virtual Authenuser Owner { get; set; } = null!;

    public virtual ICollection<Postimage> Postimages { get; set; } = new List<Postimage>();
}
