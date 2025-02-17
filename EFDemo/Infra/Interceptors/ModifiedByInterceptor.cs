using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFDemo.Infra.Interceptors;

public class ModifiedByInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var entries = eventData.Context.ChangeTracker.Entries().ToList();

        var updatedEnrties = entries.Where(e => e.Entity is not AuditLog)
                             .Where(e => e.State == EntityState.Modified);

        foreach (var entry in updatedEnrties)
        {
            if(entry.Entity is Movie)
            {

            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);

    }
}
