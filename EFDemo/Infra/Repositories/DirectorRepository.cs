using EFDemo.Infra.Context;
using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFDemo.Infra.Repositories;

public class DirectorRepository(MovieDbContext movieDbContext)
{
    public async Task AddDirector(Director director)
    {
        if (await IsDirectorExist(director))
            throw new Exception("Already Exist!");

        movieDbContext.Add(director);
        await movieDbContext.SaveChangesAsync();
    }

    public Task<bool> IsDirectorExist(Director director) => movieDbContext.Directors.AnyAsync(d => d.FirstName.Equals(director.FirstName));
}
