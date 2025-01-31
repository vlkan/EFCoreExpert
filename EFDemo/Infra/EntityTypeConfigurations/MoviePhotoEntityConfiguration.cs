using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDemo.Infra.EntityTypeConfigurations;

public class MoviePhotoEntityConfiguration : BaseEntityTypeConfiguration<MoviePhoto>
{
    public override void Configure(EntityTypeBuilder<MoviePhoto> builder)
    {
        builder.ToTable(name: "MoviePhotos", schema: "ef");

        builder.Property(i => i.Url)
            .HasColumnName("Url")
            .IsRequired(true)
            .HasMaxLength(1000);

        base.Configure(builder);
    }
}
