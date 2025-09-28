using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoschThesis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebAutomationsController : ControllerBase
{
    private readonly IWebAutomationRepository _webAutomationRepository;
    private readonly IAIService _aiService;
    private readonly ILogger<WebAutomationsController> _logger;

    public WebAutomationsController(
        IWebAutomationRepository webAutomationRepository,
        IAIService aiService,
        ILogger<WebAutomationsController> logger)
    {
        _webAutomationRepository = webAutomationRepository;
        _aiService = aiService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WebAutomation>>> GetWebAutomations()
    {
        try
        {
            var webAutomations = await _webAutomationRepository.GetAllAsync();
            return Ok(webAutomations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving web automations");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WebAutomation>> GetWebAutomation(int id)
    {
        try
        {
            var webAutomation = await _webAutomationRepository.GetByIdAsync(id);
            if (webAutomation == null)
                return NotFound();

            return Ok(webAutomation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving web automation {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<WebAutomation>> CreateWebAutomation(WebAutomation webAutomation)
    {
        try
        {
            var createdWebAutomation = await _webAutomationRepository.AddAsync(webAutomation);
            return CreatedAtAction(nameof(GetWebAutomation), new { id = createdWebAutomation.Id }, createdWebAutomation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating web automation");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWebAutomation(int id, WebAutomation webAutomation)
    {
        if (id != webAutomation.Id)
            return BadRequest();

        try
        {
            await _webAutomationRepository.UpdateAsync(webAutomation);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating web automation {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWebAutomation(int id)
    {
        try
        {
            var result = await _webAutomationRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting web automation {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("status/{statusId}")]
    public async Task<ActionResult<IEnumerable<WebAutomation>>> GetWebAutomationsByStatus(int statusId)
    {
        try
        {
            var webAutomations = await _webAutomationRepository.GetByStatusAsync(statusId);
            return Ok(webAutomations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving web automations by status {StatusId}", statusId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("type/{jobTypeId}")]
    public async Task<ActionResult<IEnumerable<WebAutomation>>> GetWebAutomationsByType(int jobTypeId)
    {
        try
        {
            var webAutomations = await _webAutomationRepository.GetByJobTypeAsync(jobTypeId);
            return Ok(webAutomations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving web automations by type {JobTypeId}", jobTypeId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("website/{websiteUrl}")]
    public async Task<ActionResult<IEnumerable<WebAutomation>>> GetWebAutomationsByWebsite(string websiteUrl)
    {
        try
        {
            var webAutomations = await _webAutomationRepository.GetByWebsiteAsync(websiteUrl);
            return Ok(webAutomations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving web automations by website {WebsiteUrl}", websiteUrl);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("failed")]
    public async Task<ActionResult<IEnumerable<WebAutomation>>> GetFailedAutomations()
    {
        try
        {
            var failedAutomations = await _webAutomationRepository.GetFailedAutomationsAsync();
            return Ok(failedAutomations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving failed automations");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<string>> AnalyzeWebPage([FromBody] AnalyzeWebPageRequest request)
    {
        try
        {
            var analysis = await _aiService.AnalyzeWebPageAsync(request.Url, request.Prompt);
            return Ok(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing web page");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("identify-element")]
    public async Task<ActionResult<string>> IdentifyWebElement([FromBody] IdentifyElementRequest request)
    {
        try
        {
            var elementInfo = await _aiService.IdentifyWebElementAsync(request.PageContent, request.Description);
            return Ok(elementInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error identifying web element");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("generate-selector")]
    public async Task<ActionResult<string>> GenerateWebSelector([FromBody] GenerateSelectorRequest request)
    {
        try
        {
            var selector = await _aiService.GenerateWebSelectorAsync(request.ElementDescription, request.PageContent);
            return Ok(selector);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating web selector");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("validate-action")]
    public async Task<ActionResult<string>> ValidateWebAction([FromBody] ValidateActionRequest request)
    {
        try
        {
            var validation = await _aiService.ValidateWebActionAsync(request.Action, request.Element, request.PageContent);
            return Ok(validation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating web action");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("extract-data")]
    public async Task<ActionResult<string>> ExtractDataFromWeb([FromBody] ExtractDataRequest request)
    {
        try
        {
            var extractedData = await _aiService.ExtractDataFromWebAsync(request.PageContent, request.ExtractionPrompt);
            return Ok(extractedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting data from web");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("generate-script")]
    public async Task<ActionResult<string>> GenerateAutomationScript([FromBody] GenerateScriptRequest request)
    {
        try
        {
            var script = await _aiService.GenerateAutomationScriptAsync(request.Requirements, request.TargetWebsite);
            return Ok(script);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating automation script");
            return StatusCode(500, "Internal server error");
        }
    }
}

public class AnalyzeWebPageRequest
{
    public string Url { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
}

public class IdentifyElementRequest
{
    public string PageContent { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class GenerateSelectorRequest
{
    public string ElementDescription { get; set; } = string.Empty;
    public string PageContent { get; set; } = string.Empty;
}

public class ValidateActionRequest
{
    public string Action { get; set; } = string.Empty;
    public string Element { get; set; } = string.Empty;
    public string PageContent { get; set; } = string.Empty;
}

public class ExtractDataRequest
{
    public string PageContent { get; set; } = string.Empty;
    public string ExtractionPrompt { get; set; } = string.Empty;
}

public class GenerateScriptRequest
{
    public string Requirements { get; set; } = string.Empty;
    public string TargetWebsite { get; set; } = string.Empty;
}
