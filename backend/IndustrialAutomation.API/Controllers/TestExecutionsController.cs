using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestExecutionsController : ControllerBase
{
    private readonly ITestExecutionRepository _testExecutionRepository;
    private readonly IAIService _aiService;
    private readonly ILogger<TestExecutionsController> _logger;

    public TestExecutionsController(
        ITestExecutionRepository testExecutionRepository,
        IAIService aiService,
        ILogger<TestExecutionsController> logger)
    {
        _testExecutionRepository = testExecutionRepository;
        _aiService = aiService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestExecution>>> GetTestExecutions()
    {
        try
        {
            var testExecutions = await _testExecutionRepository.GetAllAsync();
            return Ok(testExecutions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test executions");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestExecution>> GetTestExecution(int id)
    {
        try
        {
            var testExecution = await _testExecutionRepository.GetByIdAsync(id);
            if (testExecution == null)
                return NotFound();

            return Ok(testExecution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test execution {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TestExecution>> CreateTestExecution(TestExecution testExecution)
    {
        try
        {
            var createdTestExecution = await _testExecutionRepository.AddAsync(testExecution);
            return CreatedAtAction(nameof(GetTestExecution), new { id = createdTestExecution.Id }, createdTestExecution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating test execution");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTestExecution(int id, TestExecution testExecution)
    {
        if (id != testExecution.Id)
            return BadRequest();

        try
        {
            await _testExecutionRepository.UpdateAsync(testExecution);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating test execution {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTestExecution(int id)
    {
        try
        {
            var result = await _testExecutionRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting test execution {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("status/{statusId}")]
    public async Task<ActionResult<IEnumerable<TestExecution>>> GetTestExecutionsByStatus(int statusId)
    {
        try
        {
            var testExecutions = await _testExecutionRepository.GetByStatusAsync(statusId);
            return Ok(testExecutions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test executions by status {StatusId}", statusId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("type/{testTypeId}")]
    public async Task<ActionResult<IEnumerable<TestExecution>>> GetTestExecutionsByType(int testTypeId)
    {
        try
        {
            var testExecutions = await _testExecutionRepository.GetByTestTypeAsync(testTypeId);
            return Ok(testExecutions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test executions by type {TestTypeId}", testTypeId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("suite/{testSuite}")]
    public async Task<ActionResult<IEnumerable<TestExecution>>> GetTestExecutionsBySuite(string testSuite)
    {
        try
        {
            var testExecutions = await _testExecutionRepository.GetByTestSuiteAsync(testSuite);
            return Ok(testExecutions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test executions by suite {TestSuite}", testSuite);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("failed")]
    public async Task<ActionResult<IEnumerable<TestExecution>>> GetFailedTests()
    {
        try
        {
            var failedTests = await _testExecutionRepository.GetFailedTestsAsync();
            return Ok(failedTests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving failed tests");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/analyze")]
    public async Task<ActionResult<string>> AnalyzeTestExecution(int id)
    {
        try
        {
            var testExecution = await _testExecutionRepository.GetByIdAsync(id);
            if (testExecution == null)
                return NotFound();

            // Get test type name from ID
            var testTypeName = GetTestTypeName(testExecution.TestTypeId);
            var analysis = await _aiService.AnalyzeTestResultsAsync(
                testExecution.ActualResult, 
                testTypeName);

            testExecution.AIAnalysis = analysis;
            await _testExecutionRepository.UpdateAsync(testExecution);

            return Ok(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing test execution {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("generate")]
    public async Task<ActionResult<string>> GenerateTestCases([FromBody] GenerateTestCasesRequest request)
    {
        try
        {
            // Get test type name from ID
            var testTypeName = GetTestTypeName(request.TestTypeId);
            var testCases = await _aiService.GenerateTestCasesAsync(
                request.Requirements, 
                testTypeName);

            return Ok(testCases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating test cases");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("optimize")]
    public async Task<ActionResult<string>> OptimizeTestSuite([FromBody] OptimizeTestSuiteRequest request)
    {
        try
        {
            var optimization = await _aiService.OptimizeTestSuiteAsync(request.TestSuite);
            return Ok(optimization);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing test suite");
            return StatusCode(500, "Internal server error");
        }
    }

    private string GetTestTypeName(int testTypeId)
    {
        return testTypeId switch
        {
            1 => "Unit",
            2 => "Integration", 
            3 => "E2E",
            4 => "Performance",
            5 => "Security",
            6 => "UI",
            _ => "Unknown"
        };
    }
}

public class GenerateTestCasesRequest
{
    public string Requirements { get; set; } = string.Empty;
    public int TestTypeId { get; set; } = 1;
}

public class OptimizeTestSuiteRequest
{
    public string TestSuite { get; set; } = string.Empty;
}
