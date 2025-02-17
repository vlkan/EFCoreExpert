using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace EFDemo.Infra.Interceptors;

public class AuditLogInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var entries = eventData.Context.ChangeTracker.Entries().ToList();

        var auditLogs = entries.Where(i => i.Entity is not AuditLog)
                               .Where(i => i.State == EntityState.Added 
                                   || i.State == EntityState.Modified 
                                   || i.State == EntityState.Deleted);

        var auditLogsEntities = new List<AuditLog>();

        foreach (var entry in auditLogs)
        {
            var log = new AuditLog()
            {
                TableName = entry.Metadata.GetTableName(),
                Operation = entry.State.ToString(),
                CreatedAt = DateTime.Now
            };

            auditLogsEntities.Add(log);

            if(entry.State == EntityState.Modified)
            {
                var oldValue = entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]);

                log.OldValues = JsonSerializer.Serialize(oldValue);

                var newValue = entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]);

                log.NewValues = JsonSerializer.Serialize(newValue);
            }

            if (entry.State == EntityState.Added)
            {
                var newValue = entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]);

                log.NewValues = JsonSerializer.Serialize(newValue);
            }

            if(entry.State == EntityState.Deleted)
            {
                var oldValue = entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]);

                log.OldValues = JsonSerializer.Serialize(oldValue);
            }
        }

        eventData.Context.Set<AuditLog>().AddRange(auditLogsEntities);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
