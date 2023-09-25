using System;
using System.Collections.Generic;

namespace FindingPets.Data.Entities;

public partial class UserRole
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<AuthenUser> AuthenUsers { get; set; } = new List<AuthenUser>();
}
