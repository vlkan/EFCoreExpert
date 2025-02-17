using EFDemo.Infra.Entities.Base;

namespace EFDemo.Infra.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string Operation { get; set; }
    public string TableName { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
}
