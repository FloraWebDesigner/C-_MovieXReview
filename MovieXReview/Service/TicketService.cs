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
    public class TicketService : TicketInterface
    {
        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketDto>> ListTickets()
        {
            // include will join the Ticket with 1 movie, 1 customer
            List<Ticket> Tickets = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Viewer)
                .ToListAsync();
            // empty list of data transfer object TicketDto
            List<TicketDto> TicketDtos = new List<TicketDto>();
            // foreach Ticket record in database
            foreach (Ticket Ticket in Tickets)
            {
                // create new instance of TicketDto, add to list
                TicketDtos.Add(new TicketDto()
                {
                    TicketId = Ticket.TicketId,
                    TicketNo = Ticket.TicketNo,
                    MovieId = Ticket.Movie.MovieId,
                    MovieName = Ticket.Movie.MovieName,
                    ViewerId = Ticket.Viewer.ViewerId,
                    ViewerName = Ticket.Viewer.FirstName + " " + Ticket.Viewer.LastName,
                    Identity = Ticket.Viewer.Identity
                });
            }
            // return TicketDtos
            return TicketDtos;

        }


        public async Task<TicketDto?> FindTicket(int id)
        {
            // include will join ticket with 1 movie, 1 customer
            // first or default async will get the first ticket matching the {id}
            var Ticket = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Viewer)
                .FirstOrDefaultAsync(i => i.TicketId == id);

            // no Ticket found
            if (Ticket == null)
            {
                return null;
            }
            // create an instance of TicketDto
            TicketDto TicketDto = new TicketDto()
            {
                TicketId = Ticket.TicketId,
                TicketNo = Ticket.TicketNo,
                MovieId = Ticket.Movie.MovieId,
                MovieName = Ticket.Movie.MovieName,
                ViewerId = Ticket.Viewer.ViewerId,
                ViewerName = Ticket.Viewer.FirstName + " " + Ticket.Viewer.LastName,
                Identity = Ticket.Viewer.Identity
            };
            return TicketDto;

        }


        public async Task<ServiceResponse> UpdateTicket(TicketDto TicketDto)
        {
            ServiceResponse serviceResponse = new();
            Movie? movie = await _context.Movies.FindAsync(TicketDto.MovieId);
            Viewer? viewer = await _context.Viewers.FindAsync(TicketDto.ViewerId);
            // Posted data must link to valid entity
            if (movie == null || viewer == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                //404 Not Found
                return serviceResponse;
            }

            // Create instance of Ticket
            Ticket Ticket = new Ticket()
            {
                TicketId = Convert.ToInt32(TicketDto.TicketId),
                TicketNo = TicketDto.TicketNo,
                Movie = movie,
                Viewer = viewer,
            };
            // flags that the object has changed
            _context.Entry(Ticket).State = EntityState.Modified;

            try
            {
                // SQL Equivalent: Update Tickets set ... where TicketId={id}
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

        [HttpPost]
        public async Task<ServiceResponse> AddTicket(TicketDto TicketDto)
        {
            ServiceResponse serviceResponse = new();
            Movie? movie = await _context.Movies.FindAsync(TicketDto.MovieId);
            Viewer? viewer = await _context.Viewers.FindAsync(TicketDto.ViewerId);
            // Posted data must link to valid entity
            if (movie == null || viewer == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (movie == null)
                {
                    serviceResponse.Messages.Add("Movie was not found. ");
                }
                if (viewer == null)
                {
                    serviceResponse.Messages.Add("Viewer was not found.");
                }
                return serviceResponse;
            }

            Ticket Ticket = new Ticket()
            {
                TicketId = Convert.ToInt32(TicketDto.TicketId),
                TicketNo = TicketDto.TicketNo,
                Movie = movie,
                Viewer = viewer,
            };
            // SQL Equivalent: Insert into Tickets (..) values (..)

            try
            {
                _context.Tickets.Add(Ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the ticket.");
                serviceResponse.Messages.Add(ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Ticket.TicketId;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeleteTicket(int id)
        {
            ServiceResponse response = new();
            // Ticket must exist in the first place
            var Ticket = await _context.Tickets.FindAsync(id);
            if (Ticket == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Ticket cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Tickets.Remove(Ticket);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the ticket");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;

            return response;

        }

        public async Task<IEnumerable<TicketDto>> ListTicketsForMovie(int id)
        {
            // join MovieTickets on Ticket.TicketId = MovieTickets.TicketId WHERE MovieTickets.MovieId = {id}

            // WHERE MovieId == id
            List<Ticket> Tickets = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Viewer)
                .Where(t => t.Movie.MovieId == id && t.Movie != null && t.Viewer != null)
                .ToListAsync();

            // empty list of data transfer object TicketDto
            List<TicketDto> TicketDtos = new List<TicketDto>();
            // foreach Ticket record in database
            foreach (Ticket Ticket in Tickets)
            {
                // create new instance of TicketDto, add to list
                TicketDtos.Add(new TicketDto()
                {
                    TicketId = Ticket.TicketId,
                    TicketNo = Ticket.TicketNo,
                    MovieId = Ticket.Movie.MovieId,
                    MovieName = Ticket.Movie.MovieName,

                    ViewerId = Ticket.Viewer.ViewerId,
                    ViewerName = Ticket.Viewer.FirstName + " " + Ticket.Viewer.LastName,
                    Identity = Ticket.Viewer.Identity
                });
            }
            // return 200 OK with TicketDtos
            return TicketDtos;

        }




        public async Task<IEnumerable<TicketDto>> ListTicketsForViewer(int id)
        {
            // WHERE ViewerId == id
            List<Ticket> Tickets = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Viewer)
                .Where(t => t.Viewer.ViewerId == id)
                .ToListAsync();

            // empty list of data transfer object TicketDto
            List<TicketDto> TicketDtos = new List<TicketDto>();
            // foreach  Ticket record in database
            foreach (Ticket Ticket in Tickets)
            {
                // create new instance of TicketDto, add to list
                TicketDtos.Add(new TicketDto()
                {
                    TicketId = Ticket.TicketId,
                    TicketNo = Ticket.TicketNo,
                    MovieId = Ticket.Movie.MovieId,
                    MovieName = Ticket.Movie.MovieName,
                    ViewerId = Ticket.Viewer.ViewerId,
                    ViewerName = Ticket.Viewer.FirstName + " " + Ticket.Viewer.LastName,
                    Identity = Ticket.Viewer.Identity
                });
            }
            // return 200 OK with TicketDtos
            return TicketDtos;

        }
    }
}
