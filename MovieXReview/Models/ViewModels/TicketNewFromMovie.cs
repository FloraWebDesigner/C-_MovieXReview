namespace MovieXReview.Models.ViewModels
{
    public class TicketNewFromMovie
    {
        public IEnumerable<ViewerDto> AllViewers { get; set; }
        public required MovieDto MovieDto { get; set; }

        public TicketDto Ticket { get; set; }
    }
}
