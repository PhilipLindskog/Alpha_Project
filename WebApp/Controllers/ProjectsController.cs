using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    [Route("/admin/projects")]
    public IActionResult Index()
    {
        var viewModel = new ProjectsVievModel()
        {
            Projects = SetProjects(),
            AddProjectFormData = new AddProjectViewModel
            {
                Clients = SetClient(),
                Members = SetMembers()
            },
            EditProjectFormData = new EditProjectViewModel
            {
                Clients = SetClient(),
                Members = SetMembers(),
                Statuses = SetStatuses()
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult AddProject(AddProjectViewModel model)
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

        //här sparar jag sen ner datan men Project service

        return Ok();
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

    private IEnumerable<SelectListItem> SetClient()
    {
        var client = new List<SelectListItem> //hämta detta från databasen
        { 
            new() { Value = "1", Text = "EPN Sverige AB" }, 
            new() { Value = "2", Text = "Rövarhålan Inc." },
            new() { Value = "3", Text = "Franz Jaeger AB"}
        };
        
        return client;
    }

    private IEnumerable<SelectListItem> SetMembers()
    {
        var member = new List<SelectListItem> //hämta detta från databasen
        {
            new() { Value = "1", Text = "Philip" },
            new() { Value = "2", Text = "Tyr" },    
            new() { Value = "3", Text = "Lisen"}
        };

        return member;
    }

    private IEnumerable<SelectListItem> SetStatuses()
    {
        var status = new List<SelectListItem> //hämta detta från databasen
        {
            new() { Value = "1", Text = "STARTED", Selected = true},
            new() { Value = "2", Text = "COMPLETED" }
        };

        return status;
    }
}
