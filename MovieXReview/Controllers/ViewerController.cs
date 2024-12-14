using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieXReview;
using MovieXReview.Models;
using MovieXReview.Service;
using MovieXReview.Interface;
using Microsoft.AspNetCore.Authorization;
using MovieXReview.Services;


namespace MovieXReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewerController : ControllerBase
    {
        private readonly ViewerInterface _ViewerService;

        // dependency injection of service interfaces
        public ViewerController(ViewerInterface ViewerService)
        {
            _ViewerService = ViewerService;
        }

        /// <summary>
        /// Returns a single Viewer specified by its {id}
        /// </summary>
        /// <param name="id">ViewerId</param>
        /// <returns>
        /// 200 OK
        /// {ViewerDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Viewer/Find/1 -> {
        /// {"ViewerId": 1,  "first_name": "Ada",  "last_name": "Smith",  "identity": "general audience",  "Membership": "N", "age": 50}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<ViewerDto>> FindViewer(int id)
        {
            var Viewer = await _ViewerService.FindViewer(id);

            // if the Viewer could not be located, return 404 Not Found
            if (Viewer == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Viewer);
            }
        }


        /// <summary>
        /// Returns a list of Viewers
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ViewerDto},{ViewerDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Viewer/List -> [{ViewerDto},{ViewerDto},..]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<ViewerDto>>> ListViewers()
        {
            // empty list of data transfer object ViewerDto
            IEnumerable<ViewerDto> ViewerDtos = await _ViewerService.ListViewers();
            // return 200 OK with ViewerDtos
            return Ok(ViewerDtos);
        }


        /// <summary>
        /// Updates a Viewer
        /// </summary>
        /// <param name="id">The ID of the Viewer to update</param>
        /// <param name="ViewerDto">The required information to update the Viewer (ViewerId,Viewer_name,year,introduction,rate	duration,director,star	ticket_quantity)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Viewer/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ViewerDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpPut(template: "Update/{id}")]
        [Authorize(Roles = "admin,user")]
        public async Task<ActionResult> UpdateViewer(int id, ViewerDto ViewerDto)
        {

            // {id} in URL must match ViewerId in POST Body
            if (id != ViewerDto.ViewerId)
            {
                //400 Bad Request
                return BadRequest();
            }
            ViewerDto.Membership = string.IsNullOrEmpty(ViewerDto.Membership) ? "N" : "Y";
            ServiceResponse response = await _ViewerService.UpdateViewer(ViewerDto);

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
        /// Adds a Viewer
        /// </summary>
        /// <param name="ViewerDto">The required information to add the Viewer (ViewerId,Viewer_name,year,introduction,rate	duration,director,star	ticket_quantity)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Viewer/Find/{ViewerId}
        /// {ViewerDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Viewer/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ViewerDto}
        /// ->
        /// Response Code: 201 Created
        /// Response Headers: Location: api/Viewer/Find/{ViewerId}
        /// </example>
        [HttpPost(template: "Add")]
        [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Viewer>> AddViewer(ViewerDto ViewerDto)
        {
            ViewerDto.Membership = string.IsNullOrEmpty(ViewerDto.Membership) ? "N" : "Y";
            ServiceResponse response = await _ViewerService.AddViewer(ViewerDto);


            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Viewer/FindViewer/{response.CreatedId}", ViewerDto);
        }

        /// <summary>
        /// Deletes the Viewer
        /// </summary>
        /// <param name="id">The id of the Viewer to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Viewer/Delete/7
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "admin,user")]
        public async Task<ActionResult> DeleteViewer(int id)
        {
            ServiceResponse response = await _ViewerService.DeleteViewer(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();

        }

        /// <summary>
        /// Show a list of viewers by movie
        /// </summary>
        /// <param name="id">MovieId</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// /api/Movie/ListForViewer/1
        //ListViewersForMovie[
        //  {
        //    "MovieId": 7,
        //    "MovieName": "Shutter Island",
        //    "year": 2010,
        //    "introduction": "Teddy Daniels and Chuck Aule, two US marshals, are sent to an asylum on a remote island in order to investigate the disappearance of a patient, where Teddy uncovers a shocking truth about the place.",
        //    "rate": 8.2,
        //    "duration": "2h 18m",
        //    "director": "Martin Scorsese",
        //    "star": "Leonardo DiCaprio,Emily Mortimer,Mark Ruffalo",
        //    "ticket_quantity": 50,
        //    "ticket_sold": 0,
        //    "ticket_available": 0,
        //    "hasPic": false,
        //    "movieImgPath": null
        //  },
        //  {
        //    "MovieId": 8,
        //    "MovieName": "Cinderella",
        //    "year": 2015,
        //    "introduction": "When her father unexpectedly dies, young Ella finds herself at the mercy of her cruel stepmother and her scheming stepsisters. Never one to give up hope, Ella's fortunes begin to change after meeting a dashing stranger.",
        //    "rate": 6.9,
        //    "duration": "1h 45m",
        //    "director": "Kenneth Branagh",
        //    "star": "Lily James,Cate Blanchett,Richard Madden",
        //    "ticket_quantity": 30,
        //    "ticket_sold": 0,
        //    "ticket_available": 0,
        //    "hasPic": false,
        //    "movieImgPath": null
        //  },
        //  {
        //    "MovieId": 9,
        //    "MovieName": "The Shawshank Redemption",
        //    "year": 1994,
        //    "introduction": "A banker convicted of uxoricide forms a friendship over a quarter century with a hardened convict, while maintaining his innocence and trying to remain hopeful through simple compassion.",
        //    "rate": 9.3,
        //    "duration": "2h 22m",
        //    "director": "Frank Darabont",
        //    "star": "Tim Robbins,Morgan Freeman,Bob Gunton",
        //    "ticket_quantity": 50,
        //    "ticket_sold": 0,
        //    "ticket_available": 0,
        //    "hasPic": false,
        //    "movieImgPath": null
        //  },
        //  {
        //    "MovieId": 23,
        //    "MovieName": "Humber History",
        //    "year": 2024,
        //    "introduction": "The history of Humber college",
        //    "rate": 10,
        //    "duration": "1 hr",
        //    "director": "Christine",
        //    "star": "Students",
        //    "ticket_quantity": 10,
        //    "ticket_sold": 0,
        //    "ticket_available": 0,
        //    "hasPic": false,
        //    "movieImgPath": null
        //  }
        //]
        /// ->
        /// </example>
        [HttpGet(template: "ListForMovie/{id}")]
        public async Task<IActionResult> ListViewersForMovie(int id)
        {
            // empty list of data transfer object ViewerDto
            IEnumerable<ViewerDto> ViewerDtos = await _ViewerService.ListViewersForMovie(id);
            // return 200 OK with ViewerDtos
            return Ok(ViewerDtos);
        }



        /// <summary>
        /// Delete a viewer under the movie
        /// </summary>
        /// <param name="id">ViewerId</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// /api/Viewer/RemoveForMovie/4
        /// -> the viewer with ViewerId = 4 has been removed from the movie
        /// </example>
        [HttpDelete(template: "RemoveForMovie/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> RemoveViewerForMovie(int id)
        {
            ServiceResponse response = await _ViewerService.RemoveViewerForMovie(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Searches for viewers by name
        /// </summary>
        /// <param name="searchTerm">The search term to filter viewers by name</param>
        /// <returns>200 OK with list of viewers</returns>
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<MovieDto>>> SearchViewers([FromQuery] string searchTerm)
        {
            var viewers = await _ViewerService.SearchViewers(searchTerm);
            return Ok(viewers);
        }

    }
}
