using Microsoft.AspNetCore.Mvc;
using MovieXReview.Models;
using MovieXReview.Interface;
using MovieXReview.Models.ViewModels;
using MovieXReview.Services;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using MovieXReview.Service;
using Microsoft.EntityFrameworkCore;
using MovieXReview.Data;

namespace MovieXReview.Controllers
{
    public class MoviePageController : Controller
    {
        private readonly MovieInterface _MovieService;
        private readonly ViewerInterface _ViewerService;
        private readonly TicketInterface _TicketService;
        private readonly TagInterface _TagService;
        private readonly ReviewInterface _ReviewService;
        private readonly ApplicationDbContext _context;

        // dependency injection of service interface
        public MoviePageController(MovieInterface MovieService, ViewerInterface ViewerService, TicketInterface TicketService, TagInterface TagService, ReviewInterface ReviewService)
        {
            _MovieService = MovieService;
            _ViewerService = ViewerService;
            _TicketService = TicketService;
            _TagService = TagService;
            _ReviewService = ReviewService;

        }
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: MoviePage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<MovieDto> movieDtos = await _MovieService.ListMovies();
            
            return View(movieDtos);
        }

        // GET: MoviePage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            MovieDto? MovieDto = await _MovieService.FindMovie(id);

            if (MovieDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Movie"] });
            }

            IEnumerable<ViewerDto> AssociatedViewers = await _ViewerService.ListViewersForMovie(id);
            if (AssociatedViewers == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Viewers"] });
            }

            IEnumerable<TicketDto> AssociatedTickets = await _TicketService.ListTicketsForMovie(id);
            if (AssociatedTickets == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Tickets"] });
            }

            IEnumerable<TagDto> AssociatedTags = await _TagService.ListTagsForMovie(id);
            if (AssociatedTags == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Tags"] });
            }


            IEnumerable<ReviewDto> MovieReviews = await _ReviewService.ListReviewsForMovie(id);
            if (AssociatedTags == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Reviews"] });
            }

            // information which drives a movie page
            MovieDetails MovieInfo = new MovieDetails()
            {
                Movie = MovieDto,
                MovieViewers = AssociatedViewers,
                MovieTickets = AssociatedTickets,
                MovieTags = AssociatedTags,
                MovieReviews = MovieReviews
            };
            return View(MovieInfo);
        }


        // GET MoviePage/New
        [Authorize(Roles = "admin")]
        public ActionResult New()
        {
            return View();
        }


        // POST MoviePage/Add
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add(MovieDto MovieDto, IFormFile MoviePic)
        {
            ServiceResponse response = await _MovieService.AddMovie(MovieDto, MoviePic);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "MoviePage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET MoviePage/Edit/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            MovieDto? MovieDto = await _MovieService.FindMovie(id);
            if (MovieDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(MovieDto);
            }
        }

        //POST MoviePage/Update/{id}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, MovieDto MovieDto
            //, IFormFile MoviePic
            )
        {
            // Check if a new image is uploaded
            //if (MoviePic != null && MoviePic.Length > 0)
            //{
            //    System.Diagnostics.Debug.WriteLine("Movie Image uploaded with length: " + MoviePic.Length);

            //    // Proceed with updating the image
            //    if (MovieDto.HasPic)
            //    {
            //        // Remove the old picture if it exists
            //        string OldFileName = $"{MovieDto.MovieImgPath}";
            //        string OldFilePath = Path.Combine("wwwroot/img/movies/", OldFileName);
            //        if (System.IO.File.Exists(OldFilePath))  // Use System.IO.File.Exists
            //        {
            //            System.IO.File.Delete(OldFilePath);  // Use System.IO.File.Delete
            //        }
            //    }

            //    // Update the movie image if a new one is uploaded
            //    ServiceResponse imageresponse = await _MovieService.UpdateMovieImg(id, MoviePic);
            //    if (imageresponse.Status != ServiceResponse.ServiceStatus.Updated)
            //    {
            //        // Handle image update error
            //        ModelState.AddModelError("", "Error updating movie image.");
            //        return View(MovieDto);
            //    }
            //}
            //else
            //{
            //    // No new image is provided, log this
            //    System.Diagnostics.Debug.WriteLine("No new movie image provided, skipping image update.");
            //}

            // Update the rest of the movie details regardless of the image update
            ServiceResponse response = await _MovieService.UpdateMovie(MovieDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                // Redirect to details page after a successful update
                return RedirectToAction("Details", "MoviePage", new { id = id });
            }
            else
            {
                // If there's an error, show the error view
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET MoviePage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            MovieDto? MovieDto = await _MovieService.FindMovie(id);
            if (MovieDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(MovieDto);
            }
        }

        //POST MoviePage/Delete/{id}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _MovieService.DeleteMovie(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "MoviePage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        
    }


}
