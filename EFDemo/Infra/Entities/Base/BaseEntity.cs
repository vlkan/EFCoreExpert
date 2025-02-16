namespace EFDemo.Infra.Entities.Base;

public class BaseEntity
{
    //[Key]
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
