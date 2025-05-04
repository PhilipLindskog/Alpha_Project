using Business.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        var viewModel = new ProjectsVievModel()
        {
            Projects = SetProjects(),
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


    private IEnumerable<ProjectViewModel> SetProjects()
    {
        var projects = new List<ProjectViewModel>();

        projects.Add(new ProjectViewModel
        {
            Id = Guid.NewGuid().ToString(),
            ProjectImage = "/images/projects/project-template-purple-gradient.svg",
            ProjectName = "Website Redesign",
            ClientName = "Funzone Inc",
            Description = "It is necessary to develop a website redesign in a corporate style. Pokémon!",
            TimeLeft = "1 week left",
            Members = ["/images/users/user-template-male-green.svg"]
        });

        return projects;
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
}
