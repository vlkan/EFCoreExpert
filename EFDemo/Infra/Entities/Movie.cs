using EFDemo.Infra.Entities.Base;

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

    public MovieRelease Release { get; set; }
    public ICollection<ReleaseCinema> ReleaseCinemas { get; set; }

    public byte[] Version { get; set; }
}
