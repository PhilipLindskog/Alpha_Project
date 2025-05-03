namespace WebApp.Models;

public class ProjectsVievModel
{
    public IEnumerable<ProjectViewModel> Projects { get; set; } = [];
    public AddProjectViewModel AddProjectFormData { get; set; } = new();
    public EditProjectViewModel EditProjectFormData { get; set; } = new();
}
