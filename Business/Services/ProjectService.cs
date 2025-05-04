using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;


    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var projectEntity = formData.MapTo<ProjectEntity>();
        var statusResult = await _statusService.GetStatusByIdAsync(1);
        var status = statusResult.Result;

        projectEntity.StatusId = status!.Id;

        var result = await _projectRepository.AddAsync(projectEntity);
        
        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 201 }
            : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        var response = await _projectRepository.GetAllAsync
            (
                orderByDescending: true,
                sortBy: s => s.Created,
                where: null,
                include => include.User,
                include => include.Status,
                include => include.Client
            );

        return new ProjectResult<IEnumerable<Project>> { Succeeded = true, StatusCode = 200, Result = response.Result };
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync
            (
                where: x => x.Id == id,
                include => include.User,
                include => include.Status,
                include => include.Client
            );

        return response.Succeeded
            ? new ProjectResult<Project> { Succeeded = true, StatusCode = 200, Result = response.Result }
            : new ProjectResult<Project> { Succeeded = false, StatusCode = 404, Error = $"Project '{id}' was not found." };
    }

    public async Task<ProjectResult> UpdateProjectAsync(string id, UpdateProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var response = await _projectRepository.GetAsync
            (
                where: x => x.Id == id,
                include => include.User,
                include => include.Status,
                include => include.Client
            );
        if (!response.Succeeded)
            return new ProjectResult { Succeeded = false, StatusCode = 404, Error = $"Project '{id}' was not found." };

        var entityResponse = await _projectRepository.GetEntityAsync(x => x.Id == id);
        if (!entityResponse.Succeeded)
            return new ProjectResult { Succeeded = false, StatusCode = 500, Error = "Unexpected error fetching entity" };

        var existingEntity = entityResponse.Result;

        existingEntity!.Images = formData.Image;
        existingEntity.ProjectName = formData.ProjectName;
        existingEntity.Description = formData.Description;
        existingEntity.StartDate = formData.StartDate;
        existingEntity.EndDate = formData.EndDate;
        existingEntity.Budget = formData.Budget;
        existingEntity.ClientId = formData.ClientId;
        existingEntity.UserId = formData.UserId;
        existingEntity.StatusId = formData.StatusId;

        var result = await _projectRepository.UpdateAsync(existingEntity);

        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

    }

    public async Task<ProjectResult> DeleteProjectByIdAsync(string id)
    {
        var response = await _projectRepository.GetEntityAsync(x => x.Id == id);

        if (!response.Succeeded)
            return new ProjectResult { Succeeded = false, StatusCode = 404, Error = $"Project '{id}' was not found." };

        var existingEntity = response.Result!;

        var result = await _projectRepository.DeleteAsync(existingEntity);

        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

}
