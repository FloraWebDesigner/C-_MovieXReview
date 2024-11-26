using MovieXReview.Interface;
using MovieXReview.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using MovieXReview.Data;
using Microsoft.AspNetCore.Mvc;

namespace MovieXReview.Service
{
    public class TagService : TagInterface
    {
        private readonly ApplicationDbContext _context;

        public TagService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves a list of tags
        public async Task<IEnumerable<TagDto>> ListTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return tags.Select(tag => new TagDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                TagColor = tag.TagColor
            });
        }

        // Finds tag by ID
        // Returns a TagDto object if found, null if no tag is found
        public async Task<TagDto?> FindTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return null;
            }

            return new TagDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                TagColor = tag.TagColor
            };
        }

        //Adds a new tag to the database using the provided TagDto information (TagName, Tag Color).
        public async Task<ServiceResponse> AddTag(TagDto tagDto)
        {
            ServiceResponse response = new();
            var tag = new Tag
            {
                TagName = tagDto.TagName,
                TagColor = tagDto.TagColor
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = tag.TagId;
            return response;
        }

        //Updates an existing tag's details (TagName, Tag Color) based on the provided TagDto.
        public async Task<ServiceResponse> UpdateTag(TagDto tagDto)
        {
            ServiceResponse response = new();
            var tag = await _context.Tags.FindAsync(tagDto.TagId);
            if (tag == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                return response;
            }

            tag.TagName = tagDto.TagName;
            tag.TagColor = tagDto.TagColor;

            _context.Entry(tag).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        //Deletes a tag by ID
        public async Task<ServiceResponse> DeleteTag(int id)
        {
            ServiceResponse response = new();
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                return response;
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;

        }

        // Lists all movies associated with a tag by ID, Includes movie details + uploader info
        public async Task<IEnumerable<MovieDto>> ListMoviesForTag(int id)
        {
            var tag = await _context.Tags
                .Include(t => t.Movies)
                .FirstOrDefaultAsync(t => t.TagId == id);

            if (tag == null || tag.Movies == null)
            {
                return new List<MovieDto>();
            }

            // Map movies to MovieDto
            var movies = tag.Movies.Select(movie => new MovieDto
            {
                MovieId = movie.MovieId,
                MovieName = movie.MovieName,
                Year = movie.Year ?? 0, // Default value for null, although the year is most li
                Introduction = movie.Introduction,
                Rate = movie.Rate,
                Duration = movie.Duration,
                Director = movie.Director,
                Star = movie.Star
                    
            }).ToList();

            return movies;
        }


        // Lists all tags associated with a movie by ID, Includes movie details + uploader info
        public async Task<IEnumerable<TagDto>> ListTagsForMovie(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Tags)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null || movie.Tags == null)
            {
                return new List<TagDto>();
            }

            // Map movies to MovieDto
            var tags = movie.Tags.Select(tag => new TagDto
            {
                TagName = tag.TagName,  
                TagColor = tag.TagColor 
            }).ToList();
            return tags;
        }

    }
}
