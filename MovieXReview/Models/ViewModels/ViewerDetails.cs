namespace MovieXReview.Models.ViewModels
{
    public class ViewerDetails
    {
        // A viewer page must have a viewer
        // FindViewer(viewerid)
        public required ViewerDto Viewer { get; set; }

        // A viewer may have movies associated to it
        // ListMoviesForViewer
        public IEnumerable<MovieDto>? ViewerMovies { get; set; }

        // All tickets for this viewer
        public IEnumerable<TicketDto>? ViewerTickets { get; set; }
    }
}
