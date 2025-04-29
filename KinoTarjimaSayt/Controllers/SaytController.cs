using DataAccess.Sayt.Entities;
using Kino.Repository.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KinoTarjimaSayt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaytController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public SaytController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpPost("addMovie")]
        public async Task<int>  AddMovie(Movie movie)
        {
            return await _movieRepository.AddMovie(movie);
        }
    }
}
