using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDemo.Infra.EntityTypeConfigurations;

public class ActorEntityConfiguration : PersonBaseEntityTypeConfiguration<Actor>
{
    public override void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.ToTable(name: "Actors", schema: "ef");

        //Many-To-Many
        //builder.HasMany(a => a.Movies)
        //    .WithMany(m => m.Actors)
        //    .UsingEntity(j => j.ToTable("ActorMovies"));

        base.Configure(builder);
    }
}