namespace MovieXReview.Models.ViewModels
{
    public class MovieDetails
    {
        // MovieList
        public required MovieDto Movie { get; set; }

        // All tickets for this movie
        public IEnumerable<TicketDto>? MovieTickets { get; set; }

        // All viewers for this movie
        public IEnumerable<ViewerDto>? MovieViewers { get; set; }

        // All reviews for this movie
        public IEnumerable<ReviewDto>? MovieReviews { get; set; }

        // All images for this movie
        public IEnumerable<ImagesDto>? MovieImages { get; set; }

        // A Movie page can have many tags
        public IEnumerable<TagDto>? MovieTags { get; set; }

        // For a list of tags to choose from
        public IEnumerable<TagDto>? AllTags { get; set; }

        //A Tag page can have many associated Movies
        public IEnumerable<MovieDto>? AssociatedTags { get; set; }
    }
}
