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

        // GET: TagPage/Details/{id}
        [HttpGet]
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

        //GET ImagePage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var reviewDto = await _reviewService.FindReview(id);
            if (reviewDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Review not found" } });
            }

            return View(reviewDto);
        }

        //POST ImagePage/Delete/{id}
        [HttpPost]
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
                return RedirectToAction("Details", "ProjectPage", new { id = imageDto.MovieId });
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        //GET ImagePage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var reviewDto = await _reviewService.FindReview(id);
            if (reviewDto == null)
            {
                return NotFound();
            }
            return View(reviewDto);
        }


    }
}
