using DataAccess.Sayt.Entities;

namespace Kino.Repository.Services;

public interface IMovieRepository
{
    Task<int> AddMovie(Movie movie);
    Task<List<Movie>> GetAllMovies();
}