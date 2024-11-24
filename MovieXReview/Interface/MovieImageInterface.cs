using MovieXReview.Models;

namespace MovieXReview.Interface
{
    public interface MovieImageInterface
    {
        // Base CRUD
        Task<IEnumerable<ImagesDto>> ListImages();
        Task<ImagesDto?> FindImage(int id);
        Task<ServiceResponse> AddImage(ImagesDto imagesDto);
        Task<ServiceResponse> UpdateImage(ImagesDto imagesDto);
        Task<ServiceResponse> DeleteImage(int id);

        // Related Methods
        Task<IEnumerable<ImagesDto>> ListImagesForMovie(int movieId);
        Task<ServiceResponse> UpdateImageFile(int id, IFormFile ImageFile);

    }
 }

