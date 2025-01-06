using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDemo.Infra.EntityTypeConfigurations;

public class DirectorEntityConfiguration : PersonBaseEntityTypeConfiguration<Director>
{
    public override void Configure(EntityTypeBuilder<Director> builder)
    {
        builder.ToTable(name: "Directors", schema: "ef");

        //TODO: Movies Releational
        //One-To-Many
        //builder.HasMany(d => d.Movies)
        //    .WithOne(m => m.Director)
        //    .HasForeignKey(m => m.DirectorId);

        base.Configure(builder);
    }
}
