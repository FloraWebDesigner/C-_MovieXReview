using MovieXReview.Interface;
using MovieXReview.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using MovieXReview.Data;

namespace MovieXReview.Services
{
    public class ViewerService : ViewerInterface
    {
        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public ViewerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ViewerDto>> ListViewers()
        {
            // all Viewers
            List<Viewer> Viewers = await _context.Viewers
                .ToListAsync();
            // empty list of data transfer object ViewerDto
            List<ViewerDto> ViewerDtos = new List<ViewerDto>();
            // foreach Viewer record in database
            foreach (Viewer Viewer in Viewers)
            {
                // create new instance of ViewerDto, add to list
                ViewerDtos.Add(new ViewerDto()
                {
                    ViewerId = Viewer.ViewerId,
                    FirstName = Viewer.FirstName,
                    LastName = Viewer.LastName,
                    Identity = Viewer.Identity,
                    Membership = Viewer.Membership,
                    Age = (int)Viewer.Age

                });
            }
            return ViewerDtos;
        }

        public async Task<ViewerDto?> FindViewer(int id)
        {
            // first or default async will get the first viewer matching the {id}
            var Viewer = await _context.Viewers
                .FirstOrDefaultAsync(v => v.ViewerId == id);

            // no Viewer found
            if (Viewer == null)
            {
                return null;
            }
            // create an instance of ViewerDto
            ViewerDto ViewerDto = new ViewerDto()
            {
                ViewerId = Viewer.ViewerId,
                FirstName = Viewer.FirstName,
                LastName = Viewer.LastName,
                Identity = Viewer.Identity,
                Membership = Viewer.Membership,
                Age = (int)Viewer.Age
            };
            return ViewerDto;

        }

        public async Task<ServiceResponse> UpdateViewer(ViewerDto ViewerDto)
        {
            ServiceResponse serviceResponse = new();

            // Create instance of Viewer
            Viewer Viewer = new Viewer()
            {
                ViewerId = ViewerDto.ViewerId,
                FirstName = ViewerDto.FirstName,
                LastName = ViewerDto.LastName,
                Identity = ViewerDto.Identity,
                Membership = ViewerDto.Membership,
                Age = ViewerDto.Age
            };
            // flags that the object has changed
            _context.Entry(Viewer).State = EntityState.Modified;

            try
            {
                // SQL Equivalent: Update Viewers set ... where Viewer_id={id}
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

        public async Task<ServiceResponse> AddViewer(ViewerDto ViewerDto)
        {
            ServiceResponse serviceResponse = new();
            // Create instance of Viewer
            Viewer Viewer = new Viewer()
            {
                ViewerId = ViewerDto.ViewerId,
                FirstName = ViewerDto.FirstName,
                LastName = ViewerDto.LastName,
                Identity = ViewerDto.Identity,
                Membership = ViewerDto.Membership,
                Age = ViewerDto.Age
            };
            // SQL Equivalent: Insert into Viewers (..) values (..)

            try
            {
                _context.Viewers.Add(Viewer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Viewer.");
                serviceResponse.Messages.Add(ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Viewer.ViewerId;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeleteViewer(int id)
        {
            ServiceResponse response = new();
            // Viewer must exist in the first place
            var Viewer = await _context.Viewers.FindAsync(id);
            if (Viewer == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Viewer cannot be deleted because it does not exist.");
                return response;
            }
            try
            {
                _context.Viewers.Remove(Viewer);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the Viewer");
                return response;
            }
            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        public async Task<IEnumerable<ViewerDto>> ListViewersForMovie(int id)
        {
            // WHERE MovieId == id
            List<Viewer> Viewers = await _context.Viewers
            .Include(v => v.Tickets)
                .Where(v => v.Tickets.Any(t => t.Movie.MovieId == id))
                .ToListAsync();

            // empty list of data transfer object ViewertDto
            List<ViewerDto> ViewerDtos = new List<ViewerDto>();
            // foreach Viewer record in database
            foreach (Viewer Viewer in Viewers)
            {
                // create new instance of ViewerDto, add to list
                ViewerDtos.Add(new ViewerDto()
                {
                    ViewerId = Viewer.ViewerId,
                    FirstName = Viewer.FirstName,
                    LastName = Viewer.LastName,
                    Identity = Viewer.Identity,
                    Membership = Viewer.Membership,
                    Age = Convert.ToInt32(Viewer.Age)
                });
            }
            // return 200 OK with ViewerDtos
            return ViewerDtos;
        }

        public async Task<ServiceResponse> RemoveViewerForMovie(int id)
        {

            ServiceResponse response = new();

            // WHERE MovieId == id         
            var Viewers = await _context.Viewers
                .Include(v => v.Tickets)
                .Where(v => v.Tickets.Any(t => t.Movie.MovieId == id))
                .ToListAsync();
            if (Viewers == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Viewer cannot be deleted because it does not exist.");
                return response;
            }
            try
            {
                // RemoveRange - Reference //stackoverflow.com/questions/30623096/enable-removerange-to-remove-by-predicate-on-entity
                _context.Viewers.RemoveRange(Viewers);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the Viewer");
                return response;
            }
            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

    }

}

