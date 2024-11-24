using Microsoft.AspNetCore.Mvc;
using MovieXReview.Interface;
using MovieXReview.Models.ViewModels;
using MovieXReview.Models;
using MovieXReview.Services;
using Microsoft.AspNetCore.Authorization;


namespace MovieXReview.Controllers.Mvc
{
    public class TicketPageController : Controller
    {
        private readonly MovieInterface _MovieService;
        private readonly ViewerInterface _ViewerService;
        private readonly TicketInterface _TicketService;

        // dependency injection of service interface
        public TicketPageController(TicketInterface TicketService, MovieInterface MovieService, ViewerInterface ViewerService)
        {
            _MovieService = MovieService;
            _TicketService = TicketService;
            _ViewerService = ViewerService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: TicketPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<TicketDto?> TicketDtos = await _TicketService.ListTickets();
            return View(TicketDtos);
        }

        //GET TicketPage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            TicketDto? TicketDto = await _TicketService.FindTicket(id);
            IEnumerable<MovieDto> Movies = await _MovieService.ListMovies();
            IEnumerable<ViewerDto> Viewers = await _ViewerService.ListViewers();
            if (TicketDto == null)
            {
                return View("Error");
            }
            else
            {
                TicketEdit TicketInfo = new TicketEdit()
                {
                    Ticket = TicketDto,
                    MovieOptions = Movies,
                    ViewerOptions = Viewers
                };
                return View(TicketInfo);

            }
        }

        //POST TicketPage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, TicketDto TicketDto)
        {
            ServiceResponse response = await _TicketService.UpdateTicket(TicketDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("List", "TicketPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET TicketPage/New
        public async Task<IActionResult> New()
        {

            IEnumerable<MovieDto?> MovieDtos = await _MovieService.ListMovies();

            IEnumerable<ViewerDto?> ViewerDtos = await _ViewerService.ListViewers();

            TicketNew Options = new TicketNew()
            {
                AllViewers = ViewerDtos,
                AllMovies = MovieDtos
            };

            return View(Options);
        }

        // GET TicketPage/NewFromMovie
        public async Task<IActionResult> NewFromMovie(int id)
        {
            MovieDto? MovieDto = await _MovieService.FindMovie(id);

            if (MovieDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Movie"] });
            }
            IEnumerable<ViewerDto?> ViewerDtos = await _ViewerService.ListViewers();

            TicketNewFromMovie Options = new TicketNewFromMovie()
            {
                AllViewers = ViewerDtos,
                MovieDto = MovieDto,
                Ticket = new TicketDto()
                {
                    MovieId = id
                },
            };

            return View(Options);
        }

        // GET TicketPage/NewFromViewer
        public async Task<IActionResult> NewFromViewer(int id)
        {

            ViewerDto? ViewerDto = await _ViewerService.FindViewer(id);
            if (ViewerDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Movie"] });
            }
            IEnumerable<MovieDto?> MovieDtos = await _MovieService.ListMovies();

            TicketNewFromViewer Options = new TicketNewFromViewer()
            {
                AllMovies = MovieDtos,
                ViewerDto = ViewerDto,
                Ticket = new TicketDto()
                {
                    ViewerId = id
                },

            };

            return View(Options);
        }

        // POST TicketPage/Add
        [HttpPost("Add")]
        [Authorize]
        public async Task<IActionResult> Add(TicketDto TicketDto)
        {
            ServiceResponse response = await _TicketService.AddTicket(TicketDto);

            //checking if the item was added
            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                //return RedirectToAction("Details", "TicketPage",new { id=response.CreatedId });
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }



        }


        // GET: TicketPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            TicketDto? TicketDto = await _TicketService.FindTicket(id);


            if (TicketDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Ticket"] });
            }
            else
            {
                return View(TicketDto);
            }
        }


        /*

        // POST TicketPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(TicketDto TicketDto)
        {
            ServiceResponse response = await _TicketService.AddTicket(TicketDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "TicketPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
        */




        //GET TicketPage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            TicketDto? TicketDto = await _TicketService.FindTicket(id);
            if (TicketDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(TicketDto);
            }
        }

        //POST TicketPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _TicketService.DeleteTicket(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "TicketPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }



    }
}

