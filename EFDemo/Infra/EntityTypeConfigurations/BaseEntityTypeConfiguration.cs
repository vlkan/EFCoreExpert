using EFDemo.Infra.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDemo.Infra.EntityTypeConfigurations;

public class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            //.HasDefaultValue(DateTime.Now)
            //.HasDefaultValueSql("getdate()")
            .HasColumnType("datetime2");

        builder.Property(e => e.ModifiedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);

    }
}
