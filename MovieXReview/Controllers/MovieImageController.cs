﻿using System;
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
    public class MovieImageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MovieImageInterface _imagesService;


        public MovieImageController(ApplicationDbContext context, MovieImageInterface imagesService)
        {
            _context = context;
            _imagesService = imagesService;

        }
        /// <summary>
        /// Returns a list of all images
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ImagesDto},{ImagesDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Image/List -> [{ImageDto},{ImageDto},..]
        /// </example>
        /// 
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<MovieImage>>> ListImages()
        {
            return await _context.Images.ToListAsync();
        }

        /// <summary>
        /// Returns a single Image specified by its {id}
        /// </summary>
        /// <param name="id">The Image id</param>
        /// <returns>
        /// 200 OK
        /// {ImagesDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Image/Find/1 -> {ImagesDto}
        /// </example>
        // GET: api/Images/5
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<MovieImage>> GetImages(int id)
        {
            var images = await _context.Images.FindAsync(id);

            if (images == null)
            {
                return NotFound();
            }

            return images;
        }

        /// <summary>
        /// Updates the Name and FilePath of an Image
        /// </summary>
        /// <param name="id">The ID of the Image to update</param>
        /// <param name="ImagesDto">The required information to update the Movie (ImageName FilePath)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Image/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ImageDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        // PUT: api/Images/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut(template: "Update/{id}")]
        public async Task<IActionResult> PutImages(int id, [FromBody] ImagesDto imagesDto)
        {
            if (id != imagesDto.ImageId)
            {
                return BadRequest();
            }

            ServiceResponse response = await _imagesService.UpdateImage(imagesDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            //Status = Updated
            return NoContent();
        }

        /// <summary>
        /// Adds a new image to the database.
        /// </summary>
        /// <param name="imagesDto">The DTO containing the necessary information to create the image (e.g., ImageName, FilePath)</param>
        /// <returns>
        /// 201 Created
        /// {ImagesDto}
        /// or
        /// 404 Not Found
        /// or
        /// 500 Internal Server Error
        /// </returns>
        /// <example>
        /// POST: api/Images/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ImageDto}
        /// ->
        /// Response Code: 201 Created
        /// </example>
        /// 

        // POST: api/Images
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(template: "Add")]
        public async Task<ActionResult<MovieDto>> AddImage([FromBody] ImagesDto imagesDto)
        {
            var response = await _imagesService.AddImage(imagesDto);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Created($"api/Movie/Find/{response.CreatedId}", imagesDto);
        }

        /// <summary>
        /// Deletes the Image
        /// </summary>
        /// <param name="id">The id of the Image to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Movie/Delete/7
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        /// 

        // DELETE: api/Images/5
        [HttpDelete(template: "Delete/{id}")]
        public async Task<IActionResult> DeleteImages(int id)
        {
            var images = await _context.Images.FindAsync(id);
            if (images == null)
            {
                return NotFound();
            }

            _context.Images.Remove(images);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Receives a product picture and saves it to /wwwroot/images/movies/{id}{extension}
        /// </summary>
        /// <param name="id">The product to update an image for</param>
        /// <param name="ImageFile">The picture to change to</param>
        /// <returns>
        /// 200 OK
        /// or
        /// 404 NOT FOUND
        /// or 
        /// 500 BAD REQUEST
        /// </returns>
        /// <example>
        /// PUT : api/Product/UploadProductPic/2
        /// HEADERS: Content-Type: Multi-part/form-data, Cookie: .AspNetCore.Identity.Application={token}
        /// FORM DATA:
        /// ------boundary
        /// Content-Disposition: form-data; name="ImageFile"; filename="myimageFile.jpg"
        /// Content-Type: image/jpeg
        /// </example>
        /// <example>
        /// curl "https://localhost:xx/api/Movie/UploadImageFile/1" -X "PUT" -F ProductPic=@myproductpic.jpg
        /// </example>
        [HttpPut(template: "UploadImageFile/{id}")]
        public async Task<IActionResult> UploadImageFile(int id, IFormFile ImageFile)
        {

            ServiceResponse response = await _imagesService.UpdateImageFile(id, ImageFile);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Ok();

        }
    }
}
