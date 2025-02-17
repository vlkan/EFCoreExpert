using EFDemo.Infra.Entities.Discriminator;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDemo.Infra.EntityTypeConfigurations;

public class VideoTypeEntityConfiguration : BaseEntityTypeConfiguration<VideoTypeEntity>
{
    public override void Configure(EntityTypeBuilder<VideoTypeEntity> builder)
    {
        builder.Property(v => v.Discriminator).HasMaxLength(50);

        builder.HasDiscriminator(v => v.Discriminator)
               .HasValue<Documentary>("Documentary")
               .HasValue<TvShow>("TvShow");

        base.Configure(builder);
    }
}
