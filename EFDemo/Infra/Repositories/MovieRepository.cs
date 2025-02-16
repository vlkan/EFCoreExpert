using EFDemo.Infra.Context;
using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFDemo.Infra.Repositories;

public class MovieRepository(MovieDbContext movieDbContext)
{
    public async Task AddMovie(Movie movie)
    {
        if (await IsMovieExist(movie))
            throw new Exception("Already Exist!");

        movieDbContext.Add(movie);
        await movieDbContext.SaveChangesAsync();
    }

    public Task<bool> IsMovieExist(Movie movie) => movieDbContext.Movies.AnyAsync(m => m.Name.Equals(movie.Name));
}

