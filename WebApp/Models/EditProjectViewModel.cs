using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class EditProjectViewModel
{
    public IEnumerable<SelectListItem> Clients { get; set; } = [];
    public IEnumerable<SelectListItem> Members { get; set; } = [];
    public IEnumerable<SelectListItem> Statuses { get; set; } = [];

    public string Id { get; set; } = null!;

    [Display(Name = "Project Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProjectImages { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    [Display(Name = "Project Name", Prompt = "Enter Project name")]
    [DataType(DataType.Text)]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "You must select a client")]
    [Display(Name = "Clients", Prompt = "Select a client")]
    public string? ClientId { get; set; }

    [Display(Name = "Description", Prompt = "Enter project description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "You must set a start date")]
    [Display(Name = "Start Date", Prompt = "Enter start date")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End Date", Prompt = "Enter end date")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "You must select a member")]
    [Display(Name = "Member", Prompt = "Select a member")]
    public string? MemberId { get; set; }


    [Display(Name = "Budget", Prompt = "Enter budget")]
    [DataType(DataType.Currency)]
    public decimal? Budget { get; set; }

    [Display(Name = "Status", Prompt = "Enter status")]
    public int StatusId { get; set; }

}
