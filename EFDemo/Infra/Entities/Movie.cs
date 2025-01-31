using EFDemo.Infra.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDemo.Infra.Entities;

public class Movie : BaseEntity
{
    public string Name { get; set; }
    public int ViewCount { get; set; }

    public Guid GenreId { get; set; }
    public virtual Genre Genre { get; set; }

    public Guid DirectorId { get; set; }
    public virtual Director Director { get; set; }

    public virtual ICollection<Actor> Actors { get; set; }
    public virtual ICollection<MoviePhoto> Photos { get; set; }
}
