using MovieXReview.Models;

namespace MovieXReview.Interface
{
    public interface MovieInterface
    {
        Task<IEnumerable<MovieDto>> ListMovies();

        Task<MovieDto?> FindMovie(int id);

        Task<ServiceResponse> UpdateMovie(MovieDto MovieDto);

        Task<ServiceResponse> AddMovie(MovieDto MovieDto, IFormFile MoviePic);

        Task<ServiceResponse> DeleteMovie(int id);

        Task<MovieDto?> TicketCountForMovie(int id);

        Task<IEnumerable<MovieDto>> ListMoviesForViewer(int id);

        Task<IEnumerable<MovieDto>> ListMoviesForTag(int id);

        Task<ServiceResponse> LinkTagToMovie(int tagId, int movieId);

        Task<ServiceResponse> UnlinkTagFromMovie(int tagId, int movieId);

        //Task<ServiceResponse> UpdateMovieImg(int id, IFormFile MoviePic);


    }
}
