using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using IndustrialAutomation.Core.Entities;
using Xunit;

namespace IndustrialAutomation.Tests.Integration;

public class AutomationJobsIntegrationTests : IntegrationTestBase
{
    public AutomationJobsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAutomationJobs_ShouldReturnOk_WhenCalled()
    {
        // Act
        var response = await Client.GetAsync("/api/automationjobs");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAutomationJob_ShouldReturnCreated_WhenValidDataProvided()
    {
        // Arrange
        var job = new AutomationJob
        {
            Name = "Test Job",
            Description = "Test Description",
            StatusId = 1,
            JobTypeId = 1,
            Configuration = "{}"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/automationjobs", job);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdJob = await response.Content.ReadFromJsonAsync<AutomationJob>();
        createdJob.Should().NotBeNull();
        createdJob!.Name.Should().Be(job.Name);
        createdJob.Description.Should().Be(job.Description);
    }

    [Fact]
    public async Task GetAutomationJob_ShouldReturnNotFound_WhenJobDoesNotExist()
    {
        // Arrange
        var nonExistentId = 99999;

        // Act
        var response = await Client.GetAsync($"/api/automationjobs/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAutomationJob_ShouldReturnNoContent_WhenValidDataProvided()
    {
        // Arrange
        var job = new AutomationJob
        {
            Name = "Test Job",
            Description = "Test Description",
            StatusId = 1,
            JobTypeId = 1,
            Configuration = "{}"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/automationjobs", job);
        var createdJob = await createResponse.Content.ReadFromJsonAsync<AutomationJob>();
        
        createdJob!.Name = "Updated Job";
        createdJob.Description = "Updated Description";

        // Act
        var response = await Client.PutAsJsonAsync($"/api/automationjobs/{createdJob.Id}", createdJob);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAutomationJob_ShouldReturnNoContent_WhenJobExists()
    {
        // Arrange
        var job = new AutomationJob
        {
            Name = "Test Job",
            Description = "Test Description",
            StatusId = 1,
            JobTypeId = 1,
            Configuration = "{}"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/automationjobs", job);
        var createdJob = await createResponse.Content.ReadFromJsonAsync<AutomationJob>();

        // Act
        var response = await Client.DeleteAsync($"/api/automationjobs/{createdJob!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetAutomationJobsByStatus_ShouldReturnOk_WhenValidStatusProvided()
    {
        // Act
        var response = await Client.GetAsync("/api/automationjobs/status/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAutomationJobsByType_ShouldReturnOk_WhenValidTypeProvided()
    {
        // Act
        var response = await Client.GetAsync("/api/automationjobs/type/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAutomationJob_ShouldReturnBadRequest_WhenInvalidDataProvided()
    {
        // Arrange
        var invalidJob = new AutomationJob
        {
            Name = "", // Invalid: empty name
            Description = "Test Description",
            StatusId = 1,
            JobTypeId = 1,
            Configuration = "{}"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/automationjobs", invalidJob);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAutomationJob_ShouldReturnBadRequest_WhenIdsDoNotMatch()
    {
        // Arrange
        var job = new AutomationJob
        {
            Id = 1,
            Name = "Test Job",
            Description = "Test Description",
            StatusId = 1,
            JobTypeId = 1,
            Configuration = "{}"
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/automationjobs/2", job);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
