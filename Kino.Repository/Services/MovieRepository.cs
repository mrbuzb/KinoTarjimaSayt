using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Sayt;
using DataAccess.Sayt.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kino.Repository.Services;

public class MovieRepository : IMovieRepository
{
    private readonly MainContext mainContext;

    public MovieRepository(MainContext mainContext)
    {
        this.mainContext = mainContext;
    }

    public async Task<int> AddMovie(Movie movie)
    {
        await mainContext.Movies.AddAsync(movie);
        mainContext.SaveChanges();
        return movie.Id;
    }

    public async Task<List<Movie>> GetAllMovies()
    {
        return await mainContext.Movies.ToListAsync();
    }
}
