namespace MovieXReview.Models.ViewModels
{
    public class ReviewNew
    {

        public IEnumerable<ViewerDto> AllViewers { get; set; }
        public required MovieDto MovieDto { get; set; }
        public ReviewDto Review { get; set; }

    }
}