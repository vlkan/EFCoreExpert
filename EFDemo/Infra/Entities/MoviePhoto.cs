using EFDemo.Infra.Entities.Base;

namespace EFDemo.Infra.Entities;

public class MoviePhoto : BaseEntity
{
    public string Url { get; set; }
    public Guid MovieId { get; set; }
    public Movie Movie { get; set; }
}
