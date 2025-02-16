using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDemo.Infra.EntityTypeConfigurations;

public class GenreEntityConfiguration : BaseEntityTypeConfiguration<Genre>
{
    public override void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable(name: "Genres", schema: "ef");
        builder.Property(i => i.Name).HasColumnName("Name").IsRequired(true).HasMaxLength(100);

        base.Configure(builder);
    }
}
