using MovieXReview.Models;

namespace MovieXReview.Interface
{
    public interface ViewerInterface
    {
        Task<IEnumerable<ViewerDto>> ListViewers();

        Task<ViewerDto?> FindViewer(int id);

        Task<ServiceResponse> UpdateViewer(ViewerDto ViewerDto);

        Task<ServiceResponse> AddViewer(ViewerDto ViewerDto);

        Task<ServiceResponse> DeleteViewer(int id);

        Task<IEnumerable<ViewerDto>> ListViewersForMovie(int id);

        Task<ServiceResponse> RemoveViewerForMovie(int id);

        Task<IEnumerable<ViewerDto>> SearchViewers(string searchTerm);

       // Task<ViewerDto?> Profile();
    }
}

