using MovieXReview.Models;

namespace MovieXReview.Interface
{
    public interface ReviewInterface
    {
        // Base CRUD
        Task<IEnumerable<ReviewDto>> ListReviews();
        Task<ReviewDto?> FindReview(int id);
        Task<ServiceResponse> AddReview(ReviewDto reviewDto);
        Task<ServiceResponse> UpdateReview(ReviewDto reviewDto);
        Task<ServiceResponse> DeleteReview(int id);

        // Related Methods
        Task<IEnumerable<ReviewDto>> ListReviewsForMovie(int movieId);
        Task<IEnumerable<ReviewDto>> ListReviewsForViewer(int viewerId);

    }
 }

