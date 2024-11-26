using MovieXReview.Models;

namespace MovieXReview.Interface
{
    public interface TagInterface
    {
        //base CRUD
        Task<IEnumerable<TagDto>> ListTags();
        Task<TagDto?> FindTag(int id);
        Task<ServiceResponse> AddTag(TagDto tagDto);
        Task<ServiceResponse> UpdateTag(TagDto tagDto);
        Task<ServiceResponse> DeleteTag(int id);

        //Related Methods
        Task<IEnumerable<MovieDto>> ListMoviesForTag(int id);

        Task<IEnumerable<TagDto>> ListTagsForMovie(int id);


    }
}
