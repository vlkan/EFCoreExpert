using EFDemo.Infra.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDemo.Infra.EntityTypeConfigurations;

public class PersonBaseEntityTypeConfiguration<TEntity> : BaseEntityTypeConfiguration<TEntity> where TEntity : PersonBaseEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(e => e.FirstName)
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .HasMaxLength(100);

        base.Configure(builder);

    }
}
