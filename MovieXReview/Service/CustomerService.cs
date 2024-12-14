using Microsoft.AspNetCore.Identity;
using MovieXReview.Models;
using Microsoft.EntityFrameworkCore;
using MovieXReview.Data;
using MovieXReview.Interface;
using MovieXReview.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System;

namespace CoreEntityFramework.Service
{
    public class CustomerService : ICustomerService
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // dependency injection of database context, user manager
        public CustomerService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // implementations of customer create, read, update, delete go here


        public async Task<IEnumerable<CustomerDto>> ListCustomers()
        {
            // user manager over database context
            IEnumerable<IdentityUser> Users = await _userManager.GetUsersInRoleAsync("Customer");

            List<CustomerDto> CustomerDtos = new List<CustomerDto>();
            foreach (IdentityUser User in Users)
            {
                CustomerDtos.Add(new CustomerDto()
                {
                    CustomerId = User.Id,
                    CustomerName = User.UserName,
                    CustomerEmail = User.Email
                });
            }
            return CustomerDtos;
        }

        public async Task<CustomerDto?> FindCustomer(string id)
        {
            IdentityUser? User = await _userManager.FindByIdAsync(id);

            if (User == null) return null;

            CustomerDto CustomerDto = new CustomerDto()
            {
                CustomerId = User.Id,
                CustomerName = User.UserName,
                CustomerEmail = User.Email
            };

            return CustomerDto;
        }


        public async Task<CustomerDto?> Profile()
        {

            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);



            CustomerDto CustomerDto = new CustomerDto()
            {
                CustomerId = User.Id,
                CustomerName = User.UserName,
                CustomerEmail = User.Email
            };

            return CustomerDto;
        }

    }
}
