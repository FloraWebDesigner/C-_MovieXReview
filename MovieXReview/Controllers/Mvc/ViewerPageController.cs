
using Microsoft.AspNetCore.Mvc;
using MovieXReview.Interface;
using MovieXReview.Models.ViewModels;
using MovieXReview.Models;
using Microsoft.AspNetCore.Authorization;

namespace MovieXReview.Controllers.Mvc
{
    public class ViewerPageController : Controller
    {
        private readonly MovieInterface _MovieService;
        private readonly ViewerInterface _ViewerService;
        private readonly TicketInterface _TicketService;

        // dependency injection of service interface
        public ViewerPageController(ViewerInterface ViewerService, MovieInterface MovieService, TicketInterface TicketService)
        {

            _ViewerService = ViewerService;
            _MovieService = MovieService;
            _TicketService = TicketService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ViewerPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<ViewerDto?> ViewerDtos = await _ViewerService.ListViewers();
            return View(ViewerDtos);
        }

        // GET: ViewerPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ViewerDto? ViewerDto = await _ViewerService.FindViewer(id);
            IEnumerable<MovieDto> AssociatedMovies = await _MovieService.ListMoviesForViewer(id);
            // IEnumerable<MovieDto> Movies = await _MovieService.ListMovies();

            //need the tickets for this Viewer
            IEnumerable<TicketDto> AssociatedTickets = await _TicketService.ListTicketsForViewer(id);

            if (ViewerDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Viewer"] });
            }
            else
            {
                // information which drives a Viewer page
                ViewerDetails ViewerInfo = new ViewerDetails()
                {
                    Viewer = ViewerDto,
                    ViewerMovies = AssociatedMovies,
                    // AllMovies = Movies,
                    ViewerTickets = AssociatedTickets,
                };
                return View(ViewerInfo);
            }
        }

        // GET ViewerPage/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }


        // POST ViewerPage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(ViewerDto ViewerDto)
        {
            ViewerDto.Membership = string.IsNullOrEmpty(ViewerDto.Membership) ? "N" : "Y";
            ServiceResponse response = await _ViewerService.AddViewer(ViewerDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "ViewerPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET ViewerPage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            ViewerDto? ViewerDto = await _ViewerService.FindViewer(id);
            if (ViewerDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(ViewerDto);
            }
        }

        //POST ViewerPage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, ViewerDto ViewerDto)
        {
            ServiceResponse response = await _ViewerService.UpdateViewer(ViewerDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "ViewerPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET ViewerPage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            ViewerDto? ViewerDto = await _ViewerService.FindViewer(id);
            if (ViewerDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(ViewerDto);
            }
        }

        //POST ViewerPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _ViewerService.DeleteViewer(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "ViewerPage");
            }
            else
            {
                return RedirectToAction("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }


    }
}


