namespace MovieXReview.Models.ViewModels
{
    public class ReviewDetails
    {
        //A Review Page must have an review
        //FindReview(reviewID)
        public required ReviewDto Review { get; set; }

        // An image may have a movie associated to it
        // ListReviewsForMovie(movieId)
        public MovieDto? Movie { get; set; }

        public IEnumerable<ViewerDto> AllViewers { get; set; }

    }
}
