using System;
using System.Collections.Generic;

namespace FindingPets.Data.Entities;

public partial class AuthenUser
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public bool? IsActive { get; set; }

    public int UserRole { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual UserRole UserRoleNavigation { get; set; } = null!;
}
