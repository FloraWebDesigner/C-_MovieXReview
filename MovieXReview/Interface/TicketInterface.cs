using MovieXReview.Models;

namespace MovieXReview.Interface
{
    public interface TicketInterface
    {
        Task<IEnumerable<TicketDto>> ListTickets();

        Task<TicketDto?> FindTicket(int id);

        Task<ServiceResponse> UpdateTicket(TicketDto TicketDto);

        Task<ServiceResponse> AddTicket(TicketDto TicketDto);

        Task<ServiceResponse> DeleteTicket(int id);

        Task<IEnumerable<TicketDto>> ListTicketsForMovie(int id);

        Task<IEnumerable<TicketDto>> ListTicketsForViewer(int id);

    }
}
