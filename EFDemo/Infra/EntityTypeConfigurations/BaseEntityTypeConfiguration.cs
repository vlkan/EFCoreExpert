using EFDemo.Infra.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace EFDemo.Infra.EntityTypeConfigurations;

public class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(i => i.CreatedAt).ValueGeneratedOnAdd().HasValueGenerator<CreatedDateValueGenerator>();
        //builder.Property(i => i.ModifiedAt).ValueGeneratedOnUpdate().HasValueGenerator<CreatedDateValueGenerator>();

        builder.Property(e => e.CreatedAt)
            //.HasDefaultValue(DateTime.Now)
            //.HasDefaultValueSql("getdate()")
            .HasColumnType("datetime2");

        builder.Property(e => e.ModifiedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);

    }
}

public class CreatedDateValueGenerator : ValueGenerator<DateTime>
{
    public override bool GeneratesTemporaryValues => false;

    public override DateTime Next(EntityEntry entry)
    {
        return DateTime.Now;
    }
}
