﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieXReview.Data;
using MovieXReview.Interface;
using MovieXReview.Models;
using MovieXReview.Service;
using MovieXReview.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MovieXReview.Controllers.Mvc
{
    public class TagPageController : Controller
    {

        private readonly TagInterface _tagService;
        private readonly MovieInterface _movieService;

        // Dependency injection of the tag service
        public TagPageController(TagInterface tagService, MovieInterface movieService)
        {
            _tagService = tagService;
            _movieService = movieService;
        }
                public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: TagPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<TagDto> tagDtos = await _tagService.ListTags();
            return View(tagDtos);
        }

        // GET: TagPage/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            TagDto? tagDto = await _tagService.FindTag(id);
            IEnumerable<MovieDto> associatedMovies = await _tagService.ListMoviesForTag(id);

            if (tagDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Tag not found" } });
            }

            var tagDetails = new TagDetails
            {
                Tag = tagDto,
                AssociatedMovies = associatedMovies
            };

            return View(tagDetails);
        }

        // GET: TagPage/New
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult New()
        {
            return View();
        }


        // POST: TagPage/Add
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add(TagDto tagDto)
        {
            if (!ModelState.IsValid)
            {
                return View("New", tagDto);
            }

            var response = await _tagService.AddTag(tagDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        // GET TagPage/Edit/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            TagDto? tagDto = await _tagService.FindTag(id);
            if (tagDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Tag not found" } });
            }
            else
            {
                return View(tagDto);
            }
        }

        // POST TagPage/Update/{id}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, TagDto tagDto)
        {
            ServiceResponse response = await _tagService.UpdateTag(tagDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        // GET: TagPage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var tagDto = await _tagService.FindTag(id);
            if (tagDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Tag not found" } });
            }

            return View(tagDto);
        }

        // POST: TagPage/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _tagService.DeleteTag(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }
    }
}
