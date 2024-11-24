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

namespace MovieXReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketInterface _TicketService;

        // dependency injection of service interfaces
        public TicketController(TicketInterface TicketService)
        {
            _TicketService = TicketService;
        }

        /// <summary>
        /// Returns a list of Tickets, each represented by an TicketDto with their associated Movie, and Customer
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{TicketDto},{TicketDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Tickets/List -> [{TicketDto},{TicketDto},..]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> ListTickets()
        {
            // returns a list of data transfer object TicketDto
            IEnumerable<TicketDto> TicketDtos = await _TicketService.ListTickets();
            // return 200 OK with TicketDtos
            return Ok(TicketDtos);
        }

        /// <summary>
        /// Returns a single Ticket specified by its {id}, represented by an Ticket Dto with its associated Movie, and Viewer
        /// </summary>
        /// <param name="id">TicketId</param>
        /// <returns>
        /// 200 OK
        /// {TicketDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Tickets/Find/1 -> {TicketDto}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<TicketDto>> FindTicket(int id)
        {
            // include will join ticket with 1 movie, 1 customer
            // first or default async will get the first ticket matching the {id}
            var Ticket = await _TicketService.FindTicket(id);

            // if the item could not be located, return 404 Not Found
            if (Ticket == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Ticket);
            }
        }

        /// <summary>
        /// Updates an Ticket
        /// </summary>
        /// <param name="id">The ID of Ticket to update</param>
        /// <param name="TicketDto">The required information to update the Ticket (TicketId,TicketNo,MovieId,ViewerId)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        [HttpPut(template: "Update/{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateTicket(int id, TicketDto TicketDto)
        {
            // {id} in URL must match TicketId in POST Body
            if (id != TicketDto.TicketId)
            {
                //400 Bad Request
                return BadRequest();
            }

            ServiceResponse response = await _TicketService.UpdateTicket(TicketDto);

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
        /// Add an Ticket
        /// </summary>
        /// <param name="TicketDto">The required information to add the ticket (TicketId,TicketNo,MovieId,ViewerId)</param>
        /// <example>
        /// POST: api/Ticket/Add
        /// </example>
        /// <returns>
        /// 201 Created
        /// Location: api/Ticket/Find/{Ticket}
        /// {TicketDto}
        /// or
        /// 404 Not Found
        /// </returns>
        [HttpPost(template: "Add")]
        [Authorize]
        public async Task<ActionResult<Ticket>> AddTicket(TicketDto TicketDto)
        {
            ServiceResponse response = await _TicketService.AddTicket(TicketDto);
            Console.WriteLine($"MovieId: {TicketDto.MovieId}, ViewerId: {TicketDto.ViewerId}, TicketNo: {TicketDto.TicketNo}");
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                Console.WriteLine("Error messages: " + string.Join(", ", response.Messages));
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Ticket/FindTicket/{response.CreatedId}", TicketDto);
        }

        /// <summary>
        /// Deletes the Ticket
        /// </summary>
        /// <param name="id">The id of the Ticket to delete</param>
        /// <returns>
        /// 201 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            ServiceResponse response = await _TicketService.DeleteTicket(id);

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
        /// Returns a Ticket list specified by its associated {MovieId}, represented by an Ticket Dto with its associated Movie, and Viewer
        /// </summary>
        /// <param name="id">MovieId</param>
        /// <returns>
        /// 200 OK
        /// {TicketDto}
        /// </returns>
        /// <example>
        /// GET: api/ListForMovie/1 -> {TicketDto}
        /// </example>
        //ListTicketsForMovie
        [HttpGet(template: "ListForMovie/{id}")]
        [Authorize]
        public async Task<IActionResult> ListTicketsForMovie(int id)
        {
            // empty list of data transfer object TicketDto
            IEnumerable<TicketDto> TicketDtos = await _TicketService.ListTicketsForMovie(id);
            // return 200 OK with TicketDtos
            return Ok(TicketDtos);
        }

        /// <summary>
        /// Returns a Ticket list specified by its associated {ViewerId}, represented by an Ticket Dto with its associated Movie, and Viewer
        /// </summary>
        /// <param name="id">ViewerId</param>
        /// <returns>
        /// 200 OK
        /// {TicketDto}
        /// </returns>
        /// <example>
        /// GET: api/ListForViewer/1 -> {TicketDto}
        /// </example>
        //ListTicketsForViewer
        [HttpGet(template: "ListForViewer/{id}")]
        public async Task<IActionResult> ListTicketsForViewer(int id)
        {
            // empty list of data transfer object TicketDto
            IEnumerable<TicketDto> TicketDtos = await _TicketService.ListTicketsForViewer(id);
            // return 200 OK with RicketDtos
            return Ok(TicketDtos);
        }


    }
}

