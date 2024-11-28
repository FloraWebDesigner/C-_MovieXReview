using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieXReview.Data;
using MovieXReview.Models;
using MovieXReview.Interface;
using Microsoft.AspNetCore.Authorization;


namespace MovieXReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewInterface _reviewService;

        public ReviewController(ReviewInterface reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Returns a list of all reviews.
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ReviewDto},{ReviewDto},...]
        /// </returns>
        /// <example>
        /// GET: api/Review/List -> [{ReviewDto},{ReviewDto},...]
        /// </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> ListReviews()
        {
            var reviews = await _reviewService.ListReviews();
            return Ok(reviews);
        }

        /// <summary>
        /// Returns a single review specified by its {id}.
        /// </summary>
        /// <param name="id">The review ID</param>
        /// <returns>
        /// 200 OK
        /// {ReviewDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Review/Find/1 -> {ReviewDto}
        /// </example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ReviewDto>> FindReview(int id)
        {
            var review = await _reviewService.FindReview(id);
            if (review == null)
            {
                return NotFound($"Review with ID {id} not found.");
            }
            else
            {
                return Ok(review);
            }
        }

        /// <summary>
        /// Adds a new review.
        /// </summary>
        /// <param name="reviewDto">The required information to create the review</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Review/Find/{ReviewId}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Review/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ReviewDto}
        /// -> Response Code: 201 Created
        /// Response Headers: Location: api/Review/Find/{ReviewId}
        /// </example>
        [HttpPost("Add")]
        public async Task<ActionResult<ReviewDto>> AddReview([FromBody] ReviewDto reviewDto)
        {
            var response = await _reviewService.AddReview(reviewDto);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Created($"api/Review/Find/{response.CreatedId}", reviewDto);
        }

        /// <summary>
        /// Updates an existing review.
        /// </summary>
        /// <param name="id">The ID of the review to update</param>
        /// <param name="reviewDto">The updated review data</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// PUT: api/Review/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ReviewDto}
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateReview(int id, [FromBody] ReviewDto reviewDto)
        {
            if (id != reviewDto.ReviewId)
            {
                return BadRequest("The ID in the URL does not match the ID in the body.");
            }

            var response = await _reviewService.UpdateReview(reviewDto);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a review specified by its {id}.
        /// </summary>
        /// <param name="id">The ID of the review to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Review/Delete/5
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteReview(int id)
        {
            var response = await _reviewService.DeleteReview(id);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Returns a list of reviews for a specific movie by its {movieId}.
        /// </summary>
        /// <param name="movieId">The ID of the movie</param>
        /// <returns>
        /// 200 OK
        /// [{ReviewDto},{ReviewDto},...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Review/ListReviewsForMovie/3 -> [{ReviewDto},{ReviewDto},...]
        /// </example>
        [HttpGet("ListReviewsForMovie/{movieId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> ListReviewsForMovie(int movieId)
        {
            var reviews = await _reviewService.ListReviewsForMovie(movieId);

            if (reviews == null || !reviews.Any())
            {
                return NotFound($"No reviews found for movie with ID {movieId}.");
            }

            return Ok(reviews);
        }

        /// <summary>
        /// Returns a list of reviews for a specific viewer by their {viewerId}.
        /// </summary>
        /// <param name="viewerId">The ID of the viewer</param>
        /// <returns>
        /// 200 OK
        /// [{ReviewDto},{ReviewDto},...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Review/ListReviewsForViewer/2 -> [{ReviewDto},{ReviewDto},...]
        /// </example>
        [HttpGet("ListReviewsForViewer/{viewerId}")]

        public async Task<ActionResult<IEnumerable<ReviewDto>>> ListReviewsForViewer(int viewerId)
        {
            var reviews = await _reviewService.ListReviewsForViewer(viewerId);

            if (reviews == null || !reviews.Any())
            {
                return NotFound($"No reviews found for viewer with ID {viewerId}.");
            }

            return Ok(reviews);
        }
    }
}