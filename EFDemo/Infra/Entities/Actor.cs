using EFDemo.Infra.Entities.Base;

namespace EFDemo.Infra.Entities;

public class Actor : PersonBaseEntity
{
    public virtual ICollection<Movie> Movies { get; set; }
}
