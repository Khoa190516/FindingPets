using System;
using System.Collections.Generic;

namespace FindingPets.Data.PostgreEntities;

public partial class Userrole
{
    public int Id { get; set; }

    public string Rolename { get; set; } = null!;

    public virtual ICollection<Authenuser> Authenusers { get; set; } = new List<Authenuser>();
}
