using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieXReview.Data;
using MovieXReview.Models;
using MovieXReview.Service;
using MovieXReview.Interface;
using MovieXReview.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using MovieXReview.Services;

namespace MovieXReview.Controllers.Mvc
{
    public class ReviewPageController : Controller
    {
        private readonly ReviewInterface _reviewService;
        private readonly MovieInterface _movieService;
        private readonly ViewerInterface _viewerService;

        // dependency injection of service interface
        public ReviewPageController(ReviewInterface ReviewService, MovieInterface MovieService, ViewerInterface ViewerService)
        {
            _reviewService = ReviewService;
            _movieService = MovieService;
            _viewerService = ViewerService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ReviewPage/List
        [HttpGet]
        public async Task<IActionResult> List()
        {
            IEnumerable<ReviewDto> reviewDtos = await _reviewService.ListReviews();
            return View(reviewDtos);
        }


        // GET ReviewPage/New
        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> New(int id)
        {
            MovieDto? movieDto = await _movieService.FindMovie(id);

            if (movieDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Movie"] });
            }

            IEnumerable<ViewerDto> viewerDtos = await _viewerService.ListViewers();

            ReviewNew options = new ReviewNew()
            {
                AllViewers = viewerDtos,
                MovieDto = movieDto,
                Review = new ReviewDto()
                {
                    MovieId = id,
                    CreatedAt = DateTime.Now,
                    ReviewTitle = "", 
                    ReviewContent = "",
                    Rate = 1,
                    ImageTotal = 0 
                }
            };

            return View(options);
        }

        // POST ReviewPage/Add
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Add(ReviewDto reviewDto)
        {
            ServiceResponse response = await _reviewService.AddReview(reviewDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "MoviePage", new { id = reviewDto.MovieId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }



        // GET: ReviewPage/Details/{id}
        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Details(int id)
        {
            // Fetch review by ID
            ReviewDto? reviewDto = await _reviewService.FindReview(id);

            if (reviewDto == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new List<string> { "Review not found" }
                });
            }

            // Fetch the Movie details related to the review
            MovieDto? movieDto = await _movieService.FindMovie(reviewDto.MovieId);

            var reviewDetails = new ReviewDetails
            {
                Review = reviewDto,
                Movie = movieDto
            };

            return View(reviewDetails);

        }

        //GET ReviewPage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var reviewDto = await _reviewService.FindReview(id);
            if (reviewDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Review not found" } });
            }

            return View(reviewDto);
        }

        //POST ReviewPage/Delete/{id}
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Delete(int id)
        {
            var imageDto = await _reviewService.FindReview(id);
            if (imageDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Review not found" } });
            }

            var response = await _reviewService.DeleteReview(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("Details", "MoviePage", new { id = imageDto.MovieId });
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }


        }

        //GET ReviewPage/Edit/{id}
        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Edit(int id)
        {
            var reviewDto = await _reviewService.FindReview(id);
            if (reviewDto == null)
            {
                return NotFound();
            }
            return View(reviewDto);

        }

        //GET ReviewPage/Edit/{id}
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Update(int id, ReviewDto ReviewDto)
        {
            ServiceResponse response = await _reviewService.UpdateReview(ReviewDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "MoviePage", new { id = ReviewDto.MovieId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }

        }

    }
}
