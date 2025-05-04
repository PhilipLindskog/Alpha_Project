using Business.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers;

//[Authorize]
public class ProjectsController(IProjectService projectService, IClientService clientService, IUserService userService, IStatusService statusService) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly IClientService _clientService = clientService;
    private readonly IUserService _userService = userService;
    private readonly IStatusService _statusService = statusService;

    [Route("/admin/projects")]
    public async Task<IActionResult> Index()
    {
        var viewModel = new ProjectsViewModel()
        {
            Projects = await SetProjects(),
            AddProjectFormData = new AddProjectViewModel
            {
                Clients = await SetClient(),
                Members = await SetMembers()
            },
            EditProjectFormData = new EditProjectViewModel
            {
                Clients = await SetClient(),
                Members = await SetMembers(),
                Statuses = await SetStatuses()
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddProject(AddProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { errors });
        }

        string imagePath = string.Empty;

        if (model.ProjectImage != null && model.ProjectImage.Length > 0) //skapad med hjälp av chatGPT 4o för att hjälpa till att fixa i ordning filvägen för projektets bild
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProjectImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await model.ProjectImage.CopyToAsync(fileStream);

            imagePath = "/uploads/" + uniqueFileName;
        }

        var formData = new AddProjectFormData
        {
            Images = imagePath,
            ProjectName = model.ProjectName,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            ClientId = model.ClientId,
            UserId = model.MemberId,
            StatusId = 1
        };

        var result = await _projectService.CreateProjectAsync(formData);

        if (result.Succeeded)
        {
            return Ok();
        }

        return Conflict();
    }

    public IActionResult EditProject(EditProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors?.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { errors });
        }

        //här sparar jag sen in den ändrade datan till databasen

        return Ok();
    }


    private async Task<IEnumerable<ProjectViewModel>> SetProjects()
    {
        var result = await _projectService.GetProjectsAsync();
        

        if (!result.Succeeded)
            return Enumerable.Empty<ProjectViewModel>();

        return result.Result!.Select(x => new ProjectViewModel
        {
            Id = x.Id,
            ProjectName = x.ProjectName,
            ClientName = x.Client?.ClientName ?? "No client",
            Description = x.Description ?? "",
            ProjectImage = string.IsNullOrEmpty(x.Image)
                ? "/images/projects/project-template-purple-gradient.svg"
                : x.Image,
            TimeLeft = GetTimeLeft(x.EndDate),
            Members = [x.User?.FirstName ?? "No member"]
        });

    }

    private async Task<IEnumerable<SelectListItem>> SetClient()
    {
        var result = await _clientService.GetClientsAsync();
        
        var clients = result.Result;

        if (clients != null)
        {
            return clients.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ClientName
            });
        }

        return [];

    }

    private async Task<IEnumerable<SelectListItem>> SetMembers()
    {
        var result = await _userService.GetUsersAsync();

        var members = result.Result;
        
        if (members != null)
        {
            return members.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FirstName
            });
        }

        return [];
    }

    private async Task<IEnumerable<SelectListItem>> SetStatuses()
    {
        var result = await _statusService.GetStatusesAsync();

        var status = result.Result;
        
        if (status != null)
        {
            return status.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.StatusName
            });
        }

        return [];
    }

    private string GetTimeLeft(DateTime? endDate) //Genererad med hjälp av chatGPT för att kunna visa antal dagar kvar på projectet.
    {
        if (endDate == null)
            return "No deadline";

        var daysLeft = (endDate.Value - DateTime.Now).Days;
        return daysLeft <= 0 ? "Past deadline" : $"{daysLeft} day(s) left"; 
    }
}
