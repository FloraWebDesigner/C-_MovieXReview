using Microsoft.EntityFrameworkCore;
using MovieXReview.Data;
using MovieXReview.Models;
using static MovieXReview.Interface.MovieImageInterface;
using MovieXReview.Interface;

namespace MovieXReview.Service
{
    public class MovieImageService : MovieImageInterface
    {
        private readonly ApplicationDbContext _context;

        public MovieImageService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves a list of all images by movie ID, including associated movie details.
        public async Task<IEnumerable<ImagesDto>> ListImages()
        {
            var images = await _context.Images.Include(img => img.Movie).ToListAsync();
            return images.Select(img => new ImagesDto
            {
                ImageId = img.ImageId,
                UploadedAt = img.UploadedAt,
                FileName = img.FileName,
                MovieId = img.Movie.MovieId,
                MovieName = img.Movie.MovieName,
            });
        }

        // Finds a specific image by its ID, including associated movie details.
        // Returns null if the image is not found.
        public async Task<ImagesDto?> FindImage(int id)
        {
            var img = await _context.Images.Include(img => img.Movie).FirstOrDefaultAsync(img => img.ImageId == id);
            if (img == null)
                return null;

            return new ImagesDto
            {
                ImageId = img.ImageId,
                UploadedAt = img.UploadedAt,
                FileName = img.FileName,
                MovieId = img.Movie.MovieId,
                MovieName = img.Movie.MovieName,
                HasPic = img.HasPic,
                PicExtension = img.PicExtension
            };
        }

        // Adds a new image to the database.
        // The method checks that the image's associated movie exists before adding the image.
        public async Task<ServiceResponse> AddImage(ImagesDto imagesDto)
        {
            ServiceResponse response = new();

            var movie = await _context.Movies.FindAsync(imagesDto.MovieId);
            if (movie == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Movie not found.");
                return response;
            }

            var image = new MovieImage
            {
                FileName = imagesDto.FileName,
                UploadedAt = DateTime.Now,
                MovieId = imagesDto.MovieId,
                Movie = movie,
                HasPic = imagesDto.HasPic,
                PicExtension = imagesDto.PicExtension
            };


            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = image.ImageId;
            response.Messages.Add("Image metadata added successfully.");
            return response;
        }

        // Updates the image's FileName.
        public async Task<ServiceResponse> UpdateImage(ImagesDto imagesDto)
        {
            ServiceResponse response = new();

            var image = await _context.Images.FindAsync(imagesDto.ImageId);
            if (image == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Image not found.");
                return response;
            }

            // Only update properties that are allowed to change (e.g., FileName)
            image.FileName = imagesDto.FileName;

            _context.Entry(image).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        // Deletes an image and image file by its ID.
        public async Task<ServiceResponse> DeleteImage(int id)
        {
            ServiceResponse response = new();

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Image not found.");
                return response;
            }

            //We need to delete the image file as well
            string movieImageDirectory = Path.Combine("wwwroot/images/movies/", $"{image.MovieId}");

            if (image.HasPic && image.PicExtension != null)
            {
                string filePath = Path.Combine(movieImageDirectory, $"{image.ImageId}{image.PicExtension}");

                // Delete the file if it exists
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        response.Status = ServiceResponse.ServiceStatus.Error;
                        response.Messages.Add($"Can't delete the file");
                        return response;
                    }
                }
            }

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }


        // Lists all images associated with a movie by movieId.
        public async Task<IEnumerable<ImagesDto>> ListImagesForMovie(int movieId)
        {
            var images = await _context.Images.Where(img => img.MovieId == movieId).ToListAsync();
            return images.Select(img => new ImagesDto
            {
                ImageId = img.ImageId,
                UploadedAt = img.UploadedAt,
                FileName = img.FileName,
                MovieId = img.MovieId,
                HasPic = img.HasPic,
                PicExtension = img.PicExtension
            });
        }

        // Updates the image file by ImageId.
        // Validates the file extension and replaces the old image file if one exists.
        // Saves the new image file and updates the image in the database.
        public async Task<ServiceResponse> UpdateImageFile(int id, IFormFile ImageFile)
        {
            ServiceResponse response = new();

            MovieImage? Image = await _context.Images.FindAsync(id);
            if (Image == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"Image {id} not found");
                return response;
            }

            if (ImageFile.Length > 0)
            {
                // Generate directory for movie-specific images
                string movieImageDirectory = Path.Combine("wwwroot/images/movies/", $"{Image.MovieId}");
                if (!Directory.Exists(movieImageDirectory))
                {
                    Directory.CreateDirectory(movieImageDirectory);
                }

                // Remove old image if exists
                if (Image.HasPic) // Fixed: Use 'Image.HasPic' instead of 'Images.HasPic'
                {
                    string oldFileName = $"{Image.ImageId}{Image.PicExtension}";
                    string oldFilePath = Path.Combine(movieImageDirectory, oldFileName);
                    if (File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Check if image file is provided
                if (ImageFile == null || ImageFile.Length == 0)
                {
                    response.Messages.Add("No file uploaded. Please upload a valid image file.");
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    return response;
                }

                // Establish valid file extensions
                List<string> validExtensions = new List<string> { ".jpeg", ".jpg", ".png", ".gif" };
                string imageFileExtension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                if (!validExtensions.Contains(imageFileExtension))
                {
                    response.Messages.Add($"{imageFileExtension} is not a valid file extension");
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    return response;
                }

                string newFileName = $"{id}{imageFileExtension}";
                string newFilePath = Path.Combine(movieImageDirectory, newFileName);

                // Save the new image file
                using (var targetStream = System.IO.File.Create(newFilePath))
                {
                    await ImageFile.CopyToAsync(targetStream);
                }

                // Verify if file was uploaded successfully
                if (File.Exists(newFilePath))
                {
                    Image.PicExtension = imageFileExtension; // Fixed: Correct property name
                    Image.HasPic = true; // Fixed: Correct property name

                    _context.Entry(Image).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        response.Status = ServiceResponse.ServiceStatus.Error;
                        response.Messages.Add("An error occurred while updating the image.");
                        return response;
                    }
                }
            }
            else
            {
                response.Messages.Add("No File Content");
                response.Status = ServiceResponse.ServiceStatus.Error;
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

    }
}
