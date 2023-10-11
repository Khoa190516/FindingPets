using System;
using System.Collections.Generic;

namespace FindingPets.Data.PostgreEntities;

public partial class Authenuser
{
    public Guid Id { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public bool? Isactive { get; set; }

    public int Userrole { get; set; }

    public string? Imageurl { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual Userrole UserroleNavigation { get; set; } = null!;
}
