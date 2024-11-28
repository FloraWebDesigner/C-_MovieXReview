using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieXReview.Data;
using MovieXReview.Interface;
using MovieXReview.Models;
using MovieXReview.Models.ViewModels;
using MovieXReview.Services;


namespace MovieXReview.Controllers.Mvc
{
    public class MovieImagePageController : Controller
    {

        private readonly MovieImageInterface _movieImageService;
        private readonly MovieInterface _movieService;

        // dependency injection of service interface
        public MovieImagePageController(MovieImageInterface movieImageService, MovieInterface movieService)
        {
            _movieImageService = movieImageService;
            _movieService = movieService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");

        }

        // GET: ImagePage/New
        // Load the form for adding a new image
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult New(int movieId)
        {
            ViewData["MovieId"] = movieId; 
            return View();
        }

        // POST: ImagePage/Add
        // Handle the image file upload
        [HttpPost]
        public async Task<IActionResult> Add(ImagesDto imagesDto, IFormFile ImageFile)
        {
            // Validate file name and also uploaded file, makes sure no empty null stuff are uploaded
            if (string.IsNullOrWhiteSpace(imagesDto.FileName))
            {
                ModelState.AddModelError("FileName", "Image name cannot be empty.");
                return View("New", imagesDto);
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please upload a valid image file.");
                return View("New", imagesDto);
            }

            // if validation is passed, then proceed with the adding of image
            var result = await _movieImageService.AddImage(imagesDto);

            //Handles errors for file upload 
            if (result.Status == ServiceResponse.ServiceStatus.Created)
            {
                var uploadResponse = await _movieImageService.UpdateImageFile(result.CreatedId, ImageFile);
                if (uploadResponse.Status == ServiceResponse.ServiceStatus.Error)
                {
                    ModelState.AddModelError("", string.Join(", ", uploadResponse.Messages));
                    return View("New", imagesDto);
                }

                return RedirectToAction("Details", "MoviePage", new { id = imagesDto.MovieId });
            }

            ModelState.AddModelError("", string.Join(", ", result.Messages));

            // Redirects to the movie details page if upload is successful
            return View("New", imagesDto);
        }

        // GET: ImagePage/Details/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int id)
        {
            // Fetch image by ID
            ImagesDto? imageDto = await _movieImageService.FindImage(id);

            if (imageDto == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new List<string> { "Image not found" }
                });
            }

            // Fetch the movie details related to the image
            MovieDto? movieDto = await _movieService.FindMovie(imageDto.MovieId);

            var imageDetails = new MovieImageDetails
            {
                Image = imageDto,
                Movie = movieDto
            };

            return View(imageDetails);
        }

        //GET ImagePage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var imageDto = await _movieImageService.FindImage(id);
            if (imageDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Image not found" } });
            }

            return View(imageDto);
        }

        //POST ImagePage/Delete/{id}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var imageDto = await _movieImageService.FindImage(id);
            if (imageDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Image not found" } });
            }

            var response = await _movieImageService.DeleteImage(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("Details", "MoviePage", new { id = imageDto.MovieId });
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        //GET ImagePage/Edit/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var imageDto = await _movieImageService.FindImage(id);
            if (imageDto == null)
            {
                return NotFound();
            }
            return View(imageDto);
        }

        //POST ImagePage/Update/{id}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(ImagesDto imagesDto, IFormFile ImageFile)
        {
            // Validate the file name and also image upload!
            if (string.IsNullOrWhiteSpace(imagesDto.FileName))
            {
                ModelState.AddModelError("FileName", "Image name cannot be empty.");
                return View("Edit", imagesDto);
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please upload a valid image file.");
                return View("Edit", imagesDto);
            }

            if (ModelState.IsValid)
            {
                //Proceed with image updating if validation passes 
                var response = await _movieImageService.UpdateImage(imagesDto);
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileResponse = await _movieImageService.UpdateImageFile(imagesDto.ImageId, ImageFile);
                    if (fileResponse.Status != ServiceResponse.ServiceStatus.Updated)
                    {
                        ModelState.AddModelError(string.Empty, "Image file update failed.");
                    }
                }

                if (response.Status == ServiceResponse.ServiceStatus.Updated)
                {
                    return RedirectToAction("Details", new { id = imagesDto.ImageId });
                }

                ModelState.AddModelError(string.Empty, "Image update failed.");
            }

            return View(imagesDto);
        }

    }
}
