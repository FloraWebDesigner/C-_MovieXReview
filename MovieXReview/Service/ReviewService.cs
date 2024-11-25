using MovieXReview.Interface;
using MovieXReview.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using MovieXReview.Data;
using Microsoft.AspNetCore.Mvc;
using MovieXReview.Data;
using MovieXReview.Models;

namespace MovieXReview.Service
{
    public class ReviewService : ReviewInterface
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lists all reviews, including associated viewer and movie data
        public async Task<IEnumerable<ReviewDto>> ListReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Viewer) // Viewer information
                .Include(r => r.Movie) // Movie information
                .ToListAsync();

            return reviews.Select(review => new ReviewDto
            {
                ReviewId = review.ReviewId,
                ReviewTitle = review.ReviewTitle,
                ReviewContent = review.ReviewContent,
                Rate = review.Rate,
                CreatedAt = review.CreatedAt,
                ImageTotal = review.ImageTotal,
                ViewerId = review.ViewerId,
                MovieId = review.MovieId
            });
        }
        // Finds a specific review by ID
        public async Task<ReviewDto?> FindReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Viewer) // Viewer information
                .Include(r => r.Movie) // Movie information
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
                return null;

            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                ReviewTitle = review.ReviewTitle,
                ReviewContent = review.ReviewContent,
                Rate = review.Rate,
                CreatedAt = review.CreatedAt,
                ImageTotal = review.ImageTotal,
                ViewerId = review.ViewerId,
                MovieId = review.MovieId
            };
        }

        // Adds a new review to the database
        public async Task<ServiceResponse> AddReview(ReviewDto reviewDto)
        {
            ServiceResponse response = new();

            var movie = await _context.Movies.FindAsync(reviewDto.MovieId);

            // Data must link to a valid uploader
            if (movie == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Movie not found.");
                return response;
            }

            var viewer = await _context.Viewers.FindAsync(reviewDto.ViewerId);
            if (viewer == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Viewer not found.");
                return response;
            }

            var review = new Review
            {
                ReviewTitle = reviewDto.ReviewTitle,
                ReviewContent = reviewDto.ReviewContent,
                Rate = reviewDto.Rate,
                CreatedAt = DateTime.Now,
                MovieId = reviewDto.MovieId,
                ViewerId = reviewDto.ViewerId
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = review.ReviewId;
            return response;
        }


        // Updates an existing review
        public async Task<ServiceResponse> UpdateReview(ReviewDto reviewDto)
        {
            ServiceResponse response = new();

            var review = await _context.Reviews.FindAsync(reviewDto.ReviewId);
            if (review == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Review not found.");
                return response;
            }

            // Update review details
            review.ReviewTitle = reviewDto.ReviewTitle;
            review.ReviewContent = reviewDto.ReviewContent;
            review.Rate = reviewDto.Rate;

            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        // Deletes a review by ID
        public async Task<ServiceResponse> DeleteReview(int id)
        {
            ServiceResponse response = new();

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                return response;
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        public async Task<IEnumerable<ReviewDto>> ListReviewsForMovie(int movieId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.MovieId == movieId)
                .Include(r => r.Viewer) // Include Viewer information
                .ToListAsync();

            return reviews.Select(review => new ReviewDto
            {
                ReviewId = review.ReviewId,
                ReviewTitle = review.ReviewTitle,
                ReviewContent = review.ReviewContent,
                Rate = review.Rate,
                CreatedAt = review.CreatedAt,
                ImageTotal = review.ImageTotal,
                ViewerId = review.ViewerId
            });
        }

        // Lists all reviews for a specific viewer
        public async Task<IEnumerable<ReviewDto>> ListReviewsForViewer(int viewerId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ViewerId == viewerId)
                .Include(r => r.Movie) // Include Movie information
                .ToListAsync();

            return reviews.Select(review => new ReviewDto
            {
                ReviewId = review.ReviewId,
                ReviewTitle = review.ReviewTitle,
                ReviewContent = review.ReviewContent,
                Rate = review.Rate,
                CreatedAt = review.CreatedAt,
                ImageTotal = review.ImageTotal,
                MovieId = review.MovieId
            });
        }


    }
}
