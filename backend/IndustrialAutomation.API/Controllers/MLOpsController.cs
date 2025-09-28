using Microsoft.AspNetCore.Mvc;
using IndustrialAutomation.API.Services;
using IndustrialAutomation.Core.Entities;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MLOpsController : ControllerBase
{
    private readonly IMLOpsService _mlOpsService;
    private readonly ILogger<MLOpsController> _logger;

    public MLOpsController(IMLOpsService mlOpsService, ILogger<MLOpsController> logger)
    {
        _mlOpsService = mlOpsService;
        _logger = logger;
    }

    [HttpPost("deploy")]
    public async Task<IActionResult> DeployModel([FromBody] ModelDeploymentRequest request)
    {
        try
        {
            var result = await _mlOpsService.DeployModelAsync(request.ModelName, request.Version, request.Metadata);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deploying model {ModelName}", request.ModelName);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateModel([FromBody] ModelEvaluationRequest request)
    {
        try
        {
            var result = await _mlOpsService.EvaluateModelAsync(request.ModelName, request.Version, request.TestData);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating model {ModelName}", request.ModelName);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("models/{modelName}/versions")]
    public async Task<IActionResult> GetModelVersions(string modelName)
    {
        try
        {
            var result = await _mlOpsService.GetModelVersionsAsync(modelName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting model versions for {ModelName}", modelName);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("rollback")]
    public async Task<IActionResult> RollbackModel([FromBody] ModelRollbackRequest request)
    {
        try
        {
            var result = await _mlOpsService.RollbackModelAsync(request.ModelName, request.TargetVersion);
            return Ok(new { Success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back model {ModelName}", request.ModelName);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("drift")]
    public async Task<IActionResult> AnalyzeModelDrift([FromBody] ModelDriftRequest request)
    {
        try
        {
            var result = await _mlOpsService.AnalyzeModelDriftAsync(request.ModelName, request.Version, request.Data);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing model drift for {ModelName}", request.ModelName);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("abtest")]
    public async Task<IActionResult> RunAbtest([FromBody] AbtestRequest request)
    {
        try
        {
            var result = await _mlOpsService.RunAbtestAsync(request.ModelName, request.VersionA, request.VersionB, request.TestData);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running A/B test for {ModelName}", request.ModelName);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("recommend")]
    public async Task<IActionResult> GetModelRecommendation([FromBody] ModelRecommendationRequest request)
    {
        try
        {
            var result = await _mlOpsService.GetModelRecommendationAsync(request.UseCase, request.Requirements);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting model recommendation for use case {UseCase}", request.UseCase);
            return StatusCode(500, "Internal server error");
        }
    }
}
