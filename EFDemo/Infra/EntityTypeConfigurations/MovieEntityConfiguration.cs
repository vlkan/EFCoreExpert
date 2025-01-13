using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFDemo.Infra.EntityTypeConfigurations;

public class MovieEntityConfiguration : BaseEntityTypeConfiguration<Movie>
{
    public override void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable(name: "Movies", schema:"ef");
        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.ViewCount).HasDefaultValue(1);

        //One-To-Many - Director
        builder.HasOne(m => m.Director).WithMany(d => d.Movies).HasForeignKey(m => m.DirectorId);
        //One-To-Many - Genre
        builder.HasOne(m => m.Genre).WithMany(d => d.Movies).HasForeignKey(m => m.DirectorId);
        //Many-To-Many - Actor
        builder.HasMany(m => m.Actors).WithMany(a => a.Movies).UsingEntity(j => j.ToTable("MovieActors"));


        base.Configure(builder);
    }
}