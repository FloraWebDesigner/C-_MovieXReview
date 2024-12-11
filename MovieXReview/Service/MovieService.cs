using MovieXReview.Interface;
using MovieXReview.Models;
using Microsoft.EntityFrameworkCore;
using System;
//using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using MovieXReview.Data;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.CodeAnalysis;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;


namespace MovieXReview.Services
{
    public class MovieService : MovieInterface
    {
        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovieDto>> ListMovies()
        {
            // Fetch all movies and include related images
            var movies = await _context.Movies
                .Include(m => m.Images) // Include related images
                .Include(m => m.Tickets) // Include related tickets
                .Include(m => m.Tags) // Include related tags
                .ToListAsync();

            // Map movies to MovieDto
            var movieDtos = movies.Select(movie => new MovieDto
            {
                MovieId = movie.MovieId,
                MovieName = movie.MovieName ?? "Unknown Title",
                Year = movie.Year ?? 0,
                Introduction = movie.Introduction ?? "No Introduction Available",
                Rate = movie.Rate,
                Duration = movie.Duration ?? "Unknown Duration",
                Director = movie.Director ?? "Unknown Director",
                Star = movie.Star ?? "Unknown Star",
                TicketQuantity = movie.TicketQuantity ?? 0,
                TicketSold = movie.Tickets?.Count() ?? 0,
                TicketAvailable = (movie.TicketQuantity ?? 0) - (movie.Tickets?.Count() ?? 0),

                // Map Tags
                Tags = movie.Tags?.Select(tag => new TagDto
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName,
                    TagColor = tag.TagColor
                }).ToList(),

                // Include images when movie is listed
                Images = movie.Images?.Select(image => new ImagesDto
                {
                    ImageId = image.ImageId,
                    MovieId = movie.MovieId,
                    FileName = image.FileName,
                    PicExtension = image.PicExtension,
                    HasPic = image.HasPic
                }).ToList()
            });

            return movieDtos;
        }

        public async Task<MovieDto?> FindMovie(int id)
        {
            // first or default async will get the first movie matching the {id}
            var Movie = await _context.Movies
                .Include(m => m.Tickets)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            // no Movie found
            if (Movie == null)
            {
                return null;
            }

            // add TicketSold in  FindMovie instead of a sepaRate TicketCountForMovie - Oct 10
            int TicketSold = Movie.Tickets.Count();
            int TicketQuantity = (int)Movie.TicketQuantity;
            int TicketAvailable = Math.Max(0, TicketQuantity - TicketSold);
            // create an instance of MovieDto
            MovieDto MovieDto = new MovieDto()
            {
                MovieId = Movie.MovieId,
                MovieName = Movie.MovieName,
                Year = (int)Movie.Year,
                Introduction = (string)Movie.Introduction,
                Rate = (float)Movie.Rate,
                Duration = (string)Movie.Duration,
                Director = (string)Movie.Director,
                Star = (string)Movie.Star,

                TicketQuantity = TicketQuantity,
                TicketSold = TicketSold,

                // calculate the rest number of ticket
                TicketAvailable = TicketQuantity - TicketSold,
                // identify if there's a poster pic
                //HasPic = (bool)Movie.HasPic,
            };

            //if (Movie.HasPic)
            //{
            //    // if yes, then get the url of that image
            //    MovieDto.MovieImgPath = $"/img/movies/{Movie.MovieId}{Movie.PicExtension}";
            //}

            return MovieDto;

        }

        public async Task<ServiceResponse> UpdateMovie(MovieDto MovieDto)
        {
            ServiceResponse serviceResponse = new();

            Movie? Movie = await _context.Movies.FindAsync(MovieDto.MovieId);

            if (Movie == null)
            {
                serviceResponse.Messages.Add("Movie could not be found");
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                return serviceResponse;
            }

            Movie.MovieId = MovieDto.MovieId;
            Movie.MovieName = MovieDto.MovieName;
            Movie.Year = (int)MovieDto.Year;
            Movie.Introduction = (string)MovieDto.Introduction;
            Movie.Rate = (float)MovieDto.Rate;
            Movie.Duration = (string)MovieDto.Duration;
            Movie.Director = (string)MovieDto.Director;
            Movie.Star = (string)MovieDto.Star;
            Movie.TicketQuantity = (int)MovieDto.TicketQuantity;

            //// flags that the object has changed
            //_context.Entry(Movie).State = EntityState.Modified;
            //// handled by another method
            //_context.Entry(Movie).Property(p => p.HasPic).IsModified = false;
            //_context.Entry(Movie).Property(p => p.PicExtension).IsModified = false;

            try
            {
                // SQL Equivalent: Update Movies set ... where MovieId={id}
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the record");
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            return serviceResponse;
        }


        public async Task<ServiceResponse> AddMovie(MovieDto MovieDto, IFormFile MoviePic)
        {
            ServiceResponse serviceResponse = new();
            // Create instance of Movie
            Movie Movie = new Movie()
            {
                MovieId = MovieDto.MovieId,
                MovieName = MovieDto.MovieName,
                Year = (int)MovieDto.Year,
                Introduction = (string)MovieDto.Introduction,
                Rate = (float)MovieDto.Rate,
                Duration = (string)MovieDto.Duration,
                Director = (string)MovieDto.Director,
                Star = (string)MovieDto.Star,
                TicketQuantity = (int)MovieDto.TicketQuantity,
                //HasPic = (bool)MovieDto.HasPic,

            };
            //if (Movie.HasPic)
            //{

            //    MovieDto.MovieImgPath = $"/img/movies/{Movie.MovieId}{Movie.PicExtension}";
            //};

            // SQL Equivalent: Insert into Movies (..) values (..)
            //Console.WriteLine($"Movie Name: {MovieDto.MovieName}, Year: {MovieDto.Year}, Path: {MovieDto.MovieImgPath}");
            try
            {
                _context.Movies.Add(Movie);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Movie.");
                serviceResponse.Messages.Add(ex.Message);
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Movie.MovieId;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeleteMovie(int id)
        {
            ServiceResponse response = new();
            // Movie must exist in the first place
            var Movie = await _context.Movies.FindAsync(id);
            if (Movie == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Movie cannot be deleted because it does not exist.");
                return response;
            }
            try
            {
                _context.Movies.Remove(Movie);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the Movie");
                return response;
            }
            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        // combined the below code into the FindMovie - Oct-10
        public async Task<MovieDto?> TicketCountForMovie(int id)
        {
            // first or default async will get the first ticket matching the {id}
            var Movie = await _context.Movies
                .Include(m => m.Tickets)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            // no Movie found
            if (Movie == null)
            {
                return null;
            }
            // create an instance of MovieDto
            MovieDto MovieDto = new MovieDto()
            {
                MovieId = Movie.MovieId,
                MovieName = Movie.MovieName,
                TicketQuantity = (int)Movie.TicketQuantity,
                TicketSold = Movie.Tickets.Count()
            };
            return MovieDto;
        }


        public async Task<IEnumerable<MovieDto>> ListMoviesForViewer(int id)
        {
            // WHERE ViewerId == id
            List<Movie> Movies = await _context.Movies
            .Include(m => m.Tickets)
                .Where(m => m.Tickets.Any(t => t.Viewer.ViewerId == id))
                .ToListAsync();

            // empty list of data transfer object MovieDto
            List<MovieDto> MovieDtos = new List<MovieDto>();
            // foreach Viewer record in database
            foreach (Movie Movie in Movies)
            {
                // create new instance of MovieDto, add to list
                MovieDtos.Add(new MovieDto()
                {
                    MovieId = Movie.MovieId,
                    MovieName = Movie.MovieName,
                    Year = (int)Movie.Year,
                    Introduction = (string)Movie.Introduction,
                    Rate = (float)Movie.Rate,
                    Duration = (string)Movie.Duration,
                    Director = (string)Movie.Director,
                    Star = (string)Movie.Star,
                    TicketQuantity = (int)Movie.TicketQuantity
                });
            }
            // return 200 OK with MovieDtos
            return MovieDtos;
        }



        public async Task<IEnumerable<MovieDto>> ListMoviesForTag(int id)
        {
            List<Movie> Movies = await _context.Movies
                .Where(m => m.Tags.Any(t => t.TagId == id))
                .ToListAsync();

            // empty list of data transfer object MovieDto
            List<MovieDto> MovieDtos = new List<MovieDto>();

            foreach (Movie Movie in Movies) {
                
                    // create new instance of MovieDto, add to list
                    MovieDtos.Add(new MovieDto()
                    {
                        MovieId = Movie.MovieId,
                        MovieName = Movie.MovieName,
                        Year = (int)Movie.Year,
                        Introduction = (string)Movie.Introduction,
                        Rate = (float)Movie.Rate,
                        Duration = (string)Movie.Duration,
                        Director = (string)Movie.Director,
                        Star = (string)Movie.Star,
                        TicketQuantity = (int)Movie.TicketQuantity
                    });
                }
                return MovieDtos;
            }


        public async Task<ServiceResponse> LinkTagToMovie(int tagId, int movieId)
        {
            ServiceResponse serviceResponse = new();

            Tag? tag = await _context.Tags.Include(t => t.Movies).FirstOrDefaultAsync(t => t.TagId == tagId);
            Movie? movie = await _context.Movies.FindAsync(movieId);

            // Validate entities
            if (movie == null || tag == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (movie == null) serviceResponse.Messages.Add("movie not found.");
                if (tag == null) serviceResponse.Messages.Add("Tag not found.");
                return serviceResponse;
            }

            try
            {
                tag.Movies.Add(movie); // Add movie to tag
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error linking tag to movie.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkTagFromMovie(int tagId, int movieId)
        {
            ServiceResponse serviceResponse = new();

            Tag? tag = await _context.Tags.Include(t => t.Movies).FirstOrDefaultAsync(t => t.TagId == tagId);
            Movie? movie = await _context.Movies.Include(m => m.Tags).FirstOrDefaultAsync(m => m.MovieId == movieId);

            // Validate entities
            if (movie == null || tag == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (movie == null) serviceResponse.Messages.Add("Movie not found.");
                if (tag == null) serviceResponse.Messages.Add("Tag not found.");
                return serviceResponse;
            }

            try
            {
                tag.Movies.Remove(movie); // Remove project from tag
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error unlinking tag from movie.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }

    //Search movie by name
    public async Task<IEnumerable<MovieDto>> SearchMovies(string searchTerm)
{
    // If search term is null or empty, show all movies in the list 
    if (string.IsNullOrWhiteSpace(searchTerm))
        return await ListMovies();

    // EF Like to find searchTerm in movie names
    var movies = await _context.Movies
        .Include(m => m.Images)
        .Include(m => m.Tickets)
        .Include(m => m.Tags)
        .Where(m => EF.Functions.Like(m.MovieName, $"%{searchTerm}%"))
        .ToListAsync();

    // ListMovies
    var movieDtos = movies.Select(movie => new MovieDto
    {
        MovieId = movie.MovieId,
        MovieName = movie.MovieName ?? "Unknown Title",
        Year = movie.Year ?? 0,
        Introduction = movie.Introduction ?? "No Introduction Available",
        Rate = movie.Rate,
        Duration = movie.Duration ?? "Unknown Duration",
        Director = movie.Director ?? "Unknown Director",
        Star = movie.Star ?? "Unknown Star",
        TicketQuantity = movie.TicketQuantity ?? 0,
        TicketSold = movie.Tickets?.Count() ?? 0,
        TicketAvailable = (movie.TicketQuantity ?? 0) - (movie.Tickets?.Count() ?? 0),
        Tags = movie.Tags?.Select(tag => new TagDto
        {
            TagId = tag.TagId,
            TagName = tag.TagName,
            TagColor = tag.TagColor
        }).ToList(),
        Images = movie.Images?.Select(image => new ImagesDto
        {
            ImageId = image.ImageId,
            MovieId = movie.MovieId,
            FileName = image.FileName,
            PicExtension = image.PicExtension,
            HasPic = image.HasPic
        }).ToList()
    });

    return movieDtos;
}


    }
}


