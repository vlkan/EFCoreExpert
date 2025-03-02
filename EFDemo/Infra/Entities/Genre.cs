﻿using EFDemo.Infra.Entities.Base;

namespace EFDemo.Infra.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; }
    public virtual ICollection<Movie> Movies { get; set; }
}
