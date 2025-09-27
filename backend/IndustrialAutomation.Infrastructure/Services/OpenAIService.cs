using IndustrialAutomation.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace IndustrialAutomation.Infrastructure.Services;

public class OpenAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenAIService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _model;

    public OpenAIService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = _configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API Key not configured");
        _baseUrl = _configuration["OpenAI:BaseUrl"] ?? "https://api.openai.com/v1";
        _model = _configuration["OpenAI:Model"] ?? "gpt-4";
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<string> AnalyzeTestResultsAsync(string testResults, string testType)
    {
        try
        {
            var prompt = $@"
                As an AI test analysis expert, analyze the following {testType} test results and provide comprehensive insights:
                
                Test Results: {testResults}
                
                Please provide a detailed analysis including:
                1. Overall test health score (0-100)
                2. Key issues identified with confidence scores
                3. Recommendations for improvement
                4. Patterns in failures with ML clustering insights
                5. Performance insights and bottlenecks
                6. Predictive failure analysis
                7. Specific actionable steps to improve test reliability
                
                Format your response as a structured JSON object with clear sections.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing test results");
            return "AI analysis failed: " + ex.Message;
        }
    }

    public async Task<string> GenerateTestCasesAsync(string requirements, string testType)
    {
        try
        {
            var prompt = $@"
                As a senior test automation engineer, generate comprehensive test cases for {testType} based on the following requirements:
                
                Requirements: {requirements}
                
                Please provide:
                1. Positive test cases with detailed steps
                2. Negative test cases with edge cases
                3. Boundary value test cases
                4. Performance test scenarios
                5. Security test cases
                6. Integration test cases
                7. Accessibility test cases
                8. Data-driven test cases
                
                For each test case, include:
                - Test case ID and name
                - Preconditions
                - Test steps
                - Expected results
                - Priority level
                - Estimated execution time
                
                Format as a structured JSON response.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating test cases");
            return "Test case generation failed: " + ex.Message;
        }
    }

    public async Task<string> OptimizeTestSuiteAsync(string testSuite)
    {
        try
        {
            var prompt = $@"
                As a test optimization expert, analyze and optimize the following test suite for maximum efficiency and coverage:
                
                Test Suite: {testSuite}
                
                Please provide:
                1. Redundant test identification with specific recommendations
                2. Missing test coverage areas with priority levels
                3. Execution order optimization for parallel execution
                4. Resource usage optimization strategies
                5. Test data optimization recommendations
                6. Flaky test identification and fixes
                7. Performance bottlenecks and solutions
                8. Maintenance cost reduction strategies
                
                Include specific metrics and expected improvements.
                Format as a structured JSON response.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing test suite");
            return "Test suite optimization failed: " + ex.Message;
        }
    }

    public async Task<string> PredictTestOutcomeAsync(string testData, string testType)
    {
        try
        {
            var prompt = $@"
                As an AI test prediction specialist, predict the outcome of the following {testType} test based on historical patterns and data analysis:
                
                Test Data: {testData}
                
                Please provide:
                1. Success probability (0-100%) with confidence level
                2. Potential failure points with risk scores
                3. Risk factors and their impact
                4. Recommended mitigations
                5. Expected execution time
                6. Resource requirements
                7. Dependencies and prerequisites
                8. Alternative test approaches if high risk
                
                Format as a structured JSON response with probabilities and recommendations.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting test outcome");
            return "Test outcome prediction failed: " + ex.Message;
        }
    }

    public async Task<List<string>> ClassifyLogErrorsAsync(string logData)
    {
        try
        {
            var prompt = $@"
                As a log analysis expert, classify the following log data into error categories:
                
                Log Data: {logData}
                
                Please categorize and classify errors into:
                1. Critical errors (system-breaking)
                2. Warning errors (potential issues)
                3. Info messages (normal operation)
                4. Debug messages (development info)
                5. Performance issues (slow operations)
                6. Security issues (potential vulnerabilities)
                7. Network issues (connectivity problems)
                8. Database issues (data access problems)
                
                For each category, provide:
                - Error count
                - Severity level
                - Recommended actions
                - Priority for investigation
                
                Return as a structured list of classifications.
            ";

            var result = await CallOpenAIAsync(prompt);
            return result.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error classifying log errors");
            return new List<string> { "Error classification failed" };
        }
    }

    public async Task<Dictionary<string, double>> PredictTestFailureAsync(string testMetadata)
    {
        try
        {
            var prompt = $@"
                As an AI test failure prediction expert, analyze the following test metadata and predict failure probability:
                
                Test Metadata: {testMetadata}
                
                Please provide failure probability scores (0-1.0) for:
                1. Infrastructure failures
                2. Code-related failures
                3. Data-related failures
                4. Environment failures
                5. Performance failures
                6. Network failures
                7. Resource failures
                8. Configuration failures
                
                For each category, include:
                - Probability score
                - Confidence level
                - Risk factors
                - Mitigation strategies
                
                Return as a JSON object with probability scores.
            ";

            var result = await CallOpenAIAsync(prompt);
            var predictions = new Dictionary<string, double>();
            
            try
            {
                var jsonResult = JsonSerializer.Deserialize<Dictionary<string, object>>(result);
                if (jsonResult != null)
                {
                    foreach (var kvp in jsonResult)
                    {
                        if (double.TryParse(kvp.Value?.ToString(), out double value))
                        {
                            predictions[kvp.Key] = value;
                        }
                    }
                }
            }
            catch
            {
                // Fallback parsing
                var lines = result.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains(":"))
                    {
                        var parts = line.Split(':');
                        if (parts.Length == 2 && double.TryParse(parts[1].Trim().Replace("%", ""), out double value))
                        {
                            predictions[parts[0].Trim()] = value / 100.0;
                        }
                    }
                }
            }

            return predictions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting test failure");
            return new Dictionary<string, double> { { "error", 1.0 } };
        }
    }

    public async Task<List<string>> ClusterTestResultsAsync(List<string> testResults)
    {
        try
        {
            var prompt = $@"
                As a test data clustering expert, cluster the following test results into meaningful groups:
                
                Test Results: {string.Join("\n", testResults)}
                
                Please group results by:
                1. Similar failure patterns
                2. Performance characteristics
                3. Test types and categories
                4. Success/failure patterns
                5. Execution time patterns
                6. Resource usage patterns
                7. Error message similarities
                8. Test environment characteristics
                
                For each cluster, provide:
                - Cluster name and description
                - Common characteristics
                - Representative examples
                - Recommended actions
                
                Return as a structured list of clusters.
            ";

            var result = await CallOpenAIAsync(prompt);
            return result.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clustering test results");
            return new List<string> { "Clustering failed" };
        }
    }

    public async Task<string> DetectAnomaliesAsync(string testData)
    {
        try
        {
            var prompt = $@"
                As an anomaly detection specialist, analyze the following test data for anomalies:
                
                Test Data: {testData}
                
                Please identify:
                1. Statistical anomalies (outliers in metrics)
                2. Pattern deviations (unusual behavior patterns)
                3. Unexpected behaviors (non-standard operations)
                4. Performance outliers (unusual timing/throughput)
                5. Data inconsistencies (mismatched or invalid data)
                6. Resource anomalies (unusual CPU/memory usage)
                7. Network anomalies (unusual traffic patterns)
                8. Security anomalies (suspicious activities)
                
                For each anomaly, provide:
                - Anomaly type and description
                - Severity level
                - Potential causes
                - Recommended actions
                - Confidence score
                
                Format as a structured analysis report.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting anomalies");
            return "Anomaly detection failed: " + ex.Message;
        }
    }

    // Web Automation AI methods
    public async Task<string> AnalyzeWebPageAsync(string url, string prompt)
    {
        try
        {
            var aiPrompt = $@"
                As a web automation expert, analyze the web page at {url} and respond to: {prompt}
                
                Please provide comprehensive analysis including:
                1. Page structure analysis
                2. Interactive elements identification
                3. Form fields and input elements
                4. Navigation elements and links
                5. Content extraction opportunities
                6. Automation challenges and solutions
                7. Performance considerations
                8. Security considerations
                
                Format as a detailed technical analysis.
            ";

            return await CallOpenAIAsync(aiPrompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing web page");
            return "Web page analysis failed: " + ex.Message;
        }
    }

    public async Task<string> IdentifyWebElementAsync(string pageContent, string description)
    {
        try
        {
            var prompt = $@"
                As a web element identification expert, identify web elements based on the description: {description}
                
                Page Content: {pageContent}
                
                Please provide:
                1. Element selectors (CSS, XPath, ID, Class)
                2. Element attributes and properties
                3. Element hierarchy and relationships
                4. Interaction methods and capabilities
                5. Confidence scores for each selector
                6. Alternative selectors for robustness
                7. Element state and visibility
                8. Accessibility considerations
                
                Format as a structured element identification report.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error identifying web element");
            return "Web element identification failed: " + ex.Message;
        }
    }

    public async Task<string> GenerateWebSelectorAsync(string elementDescription, string pageContent)
    {
        try
        {
            var prompt = $@"
                As a web selector generation expert, generate optimal selectors for the element: {elementDescription}
                
                Page Content: {pageContent}
                
                Please provide:
                1. Primary CSS selectors (most reliable)
                2. Fallback CSS selectors (alternative options)
                3. XPath expressions (absolute and relative)
                4. Element attributes and their values
                5. Robustness considerations and trade-offs
                6. Performance implications of each selector
                7. Maintenance considerations
                8. Cross-browser compatibility notes
                
                Rank selectors by reliability and performance.
                Format as a structured selector guide.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating web selector");
            return "Web selector generation failed: " + ex.Message;
        }
    }

    public async Task<string> ValidateWebActionAsync(string action, string element, string pageContent)
    {
        try
        {
            var prompt = $@"
                As a web action validation expert, validate the web action: {action} on element: {element}
                
                Page Content: {pageContent}
                
                Please provide:
                1. Action validity assessment
                2. Potential issues and risks
                3. Alternative approaches
                4. Risk factors and mitigation
                5. Success probability
                6. Prerequisites and dependencies
                7. Error handling recommendations
                8. Performance considerations
                
                Format as a comprehensive validation report.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating web action");
            return "Web action validation failed: " + ex.Message;
        }
    }

    public async Task<string> ExtractDataFromWebAsync(string pageContent, string extractionPrompt)
    {
        try
        {
            var prompt = $@"
                As a web data extraction expert, extract data from web page based on: {extractionPrompt}
                
                Page Content: {pageContent}
                
                Please provide:
                1. Structured data extraction results
                2. Data validation and quality assessment
                3. Missing data identification
                4. Data quality scores and confidence levels
                5. Extraction confidence and reliability
                6. Data formatting and normalization
                7. Error handling for incomplete data
                8. Recommendations for data improvement
                
                Format as a structured data extraction report.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting data from web");
            return "Web data extraction failed: " + ex.Message;
        }
    }

    public async Task<string> GenerateAutomationScriptAsync(string requirements, string targetWebsite)
    {
        try
        {
            var prompt = $@"
                As a web automation script generation expert, generate automation script for: {requirements}
                Target Website: {targetWebsite}
                
                Please provide:
                1. Complete automation script (Python/Selenium or JavaScript/Playwright)
                2. Comprehensive error handling
                3. Wait strategies and timeouts
                4. Data validation and verification
                5. Performance optimizations
                6. Logging and monitoring
                7. Configuration management
                8. Testing and validation steps
                
                Include comments and documentation.
                Format as a production-ready automation script.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating automation script");
            return "Automation script generation failed: " + ex.Message;
        }
    }

    public async Task<string> AnalyzeAutomationResultsAsync(string results, string expectedOutcome)
    {
        try
        {
            var prompt = $@"
                As an automation results analysis expert, analyze automation results:
                
                Results: {results}
                Expected: {expectedOutcome}
                
                Please provide:
                1. Success/failure analysis with detailed comparison
                2. Performance metrics and timing analysis
                3. Error identification and categorization
                4. Improvement suggestions and optimizations
                5. Next steps and recommendations
                6. Risk assessment and mitigation
                7. Quality metrics and scores
                8. Lessons learned and best practices
                
                Format as a comprehensive results analysis report.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing automation results");
            return "Automation results analysis failed: " + ex.Message;
        }
    }

    public async Task<string> OptimizeAutomationFlowAsync(string currentFlow, string performanceData)
    {
        try
        {
            var prompt = $@"
                As an automation flow optimization expert, optimize automation flow:
                
                Current Flow: {currentFlow}
                Performance Data: {performanceData}
                
                Please provide:
                1. Flow optimization suggestions with specific improvements
                2. Performance improvements and bottlenecks
                3. Error handling enhancements
                4. Parallel execution opportunities
                5. Resource optimization strategies
                6. Code refactoring recommendations
                7. Monitoring and alerting improvements
                8. Scalability considerations
                
                Include expected performance gains and implementation effort.
                Format as a detailed optimization plan.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing automation flow");
            return "Automation flow optimization failed: " + ex.Message;
        }
    }

    public async Task<Dictionary<string, object>> ExtractWebElementsAsync(string screenshotPath, string description)
    {
        try
        {
            var prompt = $@"
                As a web element extraction expert, extract web elements from screenshot: {screenshotPath}
                Description: {description}
                
                Please provide:
                1. Element coordinates and positions
                2. Element types and classifications
                3. Element attributes and properties
                4. Interaction possibilities and methods
                5. Confidence scores for each element
                6. Element relationships and hierarchy
                7. Accessibility information
                8. Automation recommendations
                
                Format as a structured element extraction report.
            ";

            var result = await CallOpenAIAsync(prompt);
            return new Dictionary<string, object> { { "elements", result } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting web elements");
            return new Dictionary<string, object> { { "error", "Element extraction failed" } };
        }
    }

    public async Task<List<string>> RecognizeWebElementsAsync(byte[] screenshot)
    {
        try
        {
            var prompt = $@"
                As a web element recognition expert, recognize web elements from screenshot:
                
                Please identify and classify:
                1. Buttons and interactive elements
                2. Form fields and input elements
                3. Navigation elements and menus
                4. Content areas and text blocks
                5. Interactive components and widgets
                6. Links and clickable elements
                7. Images and media elements
                8. Layout containers and sections
                
                For each element, provide:
                - Element type and classification
                - Position and size information
                - Interaction capabilities
                - Accessibility features
                - Automation recommendations
                
                Format as a structured element recognition report.
            ";

            var result = await CallOpenAIAsync(prompt);
            return result.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recognizing web elements");
            return new List<string> { "Element recognition failed" };
        }
    }

    // Natural Language Processing methods
    public async Task<string> ProcessNaturalLanguageCommandAsync(string command)
    {
        try
        {
            var prompt = $@"
                As a natural language processing expert, process natural language command: {command}
                
                Please provide:
                1. Command interpretation and understanding
                2. Required actions and steps
                3. Parameters extraction and validation
                4. Validation rules and constraints
                5. Execution plan and sequence
                6. Error handling and fallbacks
                7. Success criteria and metrics
                8. Alternative interpretations if ambiguous
                
                Format as a structured command processing report.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing natural language command");
            return "Command processing failed: " + ex.Message;
        }
    }

    public async Task<Dictionary<string, object>> ParseUserIntentAsync(string userInput)
    {
        try
        {
            var prompt = $@"
                As an intent parsing expert, parse user intent from: {userInput}
                
                Please provide:
                1. Intent classification and categorization
                2. Entity extraction and identification
                3. Action requirements and specifications
                4. Context understanding and implications
                5. Confidence scores for each interpretation
                6. Ambiguity resolution and clarification
                7. Parameter extraction and validation
                8. Next steps and recommendations
                
                Format as a structured intent parsing report.
            ";

            var result = await CallOpenAIAsync(prompt);
            return new Dictionary<string, object> { { "intent", result } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing user intent");
            return new Dictionary<string, object> { { "error", "Intent parsing failed" } };
        }
    }

    public async Task<string> GenerateActionPlanAsync(string naturalLanguageTask)
    {
        try
        {
            var prompt = $@"
                As an action plan generation expert, generate action plan for: {naturalLanguageTask}
                
                Please provide:
                1. Step-by-step actions with detailed instructions
                2. Dependencies and prerequisites
                3. Risk assessment and mitigation strategies
                4. Success criteria and validation
                5. Alternative approaches and fallbacks
                6. Resource requirements and estimates
                7. Timeline and scheduling considerations
                8. Quality assurance and testing steps
                
                Format as a comprehensive action plan.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating action plan");
            return "Action plan generation failed: " + ex.Message;
        }
    }

    public async Task<List<string>> ExtractEntitiesAsync(string text)
    {
        try
        {
            var prompt = $@"
                As a named entity recognition expert, extract entities from: {text}
                
                Please identify and extract:
                1. Person names and individuals
                2. Organizations and companies
                3. Locations and places
                4. Dates and times
                5. Technical terms and concepts
                6. Products and services
                7. URLs and links
                8. Email addresses and contacts
                
                For each entity, provide:
                - Entity type and classification
                - Confidence score
                - Context and usage
                - Related entities
                
                Format as a structured entity extraction report.
            ";

            var result = await CallOpenAIAsync(prompt);
            return result.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting entities");
            return new List<string> { "Entity extraction failed" };
        }
    }

    public async Task<string> TranslateIntentToActionsAsync(string intent)
    {
        try
        {
            var prompt = $@"
                As an intent translation expert, translate intent to actions: {intent}
                
                Please provide:
                1. Concrete actions and implementations
                2. Implementation steps and procedures
                3. Required resources and tools
                4. Success metrics and measurements
                5. Error handling and recovery
                6. Validation and verification steps
                7. Performance considerations
                8. Maintenance and monitoring
                
                Format as a detailed action translation report.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating intent to actions");
            return "Intent translation failed: " + ex.Message;
        }
    }

    // ML Model Management methods
    public async Task<bool> TrainModelAsync(string modelType, string trainingData)
    {
        try
        {
            var prompt = $@"
                As an ML model training expert, train {modelType} model with training data:
                
                Training Data: {trainingData}
                
                Please provide:
                1. Training progress and status updates
                2. Model performance metrics and evaluation
                3. Validation results and accuracy scores
                4. Optimization suggestions and improvements
                5. Deployment readiness assessment
                6. Resource requirements and usage
                7. Training time estimates
                8. Quality assurance recommendations
                
                Format as a comprehensive training report.
            ";

            var result = await CallOpenAIAsync(prompt);
            return result.Contains("success") || result.Contains("completed") || result.Contains("ready");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training model");
            return false;
        }
    }

    public async Task<double> EvaluateModelAsync(string modelName, string testData)
    {
        try
        {
            var prompt = $@"
                As an ML model evaluation expert, evaluate {modelName} model with test data:
                
                Test Data: {testData}
                
                Please provide:
                1. Accuracy score and performance metrics
                2. Precision and recall scores
                3. F1 score and harmonic mean
                4. Overall performance assessment
                5. Confusion matrix analysis
                6. ROC curve and AUC scores
                7. Cross-validation results
                8. Benchmark comparisons
                
                Return numerical scores and detailed analysis.
            ";

            var result = await CallOpenAIAsync(prompt);
            
            // Extract accuracy score from result
            var lines = result.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains("accuracy") && line.Contains("%"))
                {
                    var accuracyStr = line.Split('%')[0].Split().Last();
                    if (double.TryParse(accuracyStr, out double accuracy))
                    {
                        return accuracy / 100.0;
                    }
                }
            }
            return 0.0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating model");
            return 0.0;
        }
    }

    public async Task<bool> DeployModelAsync(string modelName, string version)
    {
        try
        {
            var prompt = $@"
                As an ML model deployment expert, deploy {modelName} model version {version}:
                
                Please provide:
                1. Deployment status and progress
                2. Health checks and validation
                3. Performance metrics and monitoring
                4. Monitoring setup and alerts
                5. Rollback plan and procedures
                6. Load balancing and scaling
                7. Security considerations
                8. Documentation and maintenance
                
                Format as a comprehensive deployment report.
            ";

            var result = await CallOpenAIAsync(prompt);
            return result.Contains("deployed") || result.Contains("success") || result.Contains("ready");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deploying model");
            return false;
        }
    }

    public async Task<Dictionary<string, object>> GetModelMetricsAsync(string modelName)
    {
        try
        {
            var prompt = $@"
                As an ML model monitoring expert, get metrics for {modelName} model:
                
                Please provide:
                1. Performance metrics and KPIs
                2. Usage statistics and patterns
                3. Accuracy trends and improvements
                4. Resource utilization and costs
                5. Health status and alerts
                6. Error rates and failures
                7. Response times and latency
                8. Throughput and capacity
                
                Format as a comprehensive metrics dashboard.
            ";

            var result = await CallOpenAIAsync(prompt);
            return new Dictionary<string, object> { { "metrics", result } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting model metrics");
            return new Dictionary<string, object> { { "error", "Failed to get metrics" } };
        }
    }

    public async Task<List<string>> GetAvailableModelsAsync()
    {
        try
        {
            var prompt = $@"
                As an ML model management expert, list available AI models:
                
                Please provide:
                1. Model names and identifiers
                2. Model types and categories
                3. Versions and release dates
                4. Status and availability
                5. Capabilities and features
                6. Performance characteristics
                7. Resource requirements
                8. Usage recommendations
                
                Format as a comprehensive model catalog.
            ";

            var result = await CallOpenAIAsync(prompt);
            return result.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available models");
            return new List<string> { "No models available" };
        }
    }

    // Computer Vision methods
    public async Task<Dictionary<string, object>> AnalyzeScreenshotAsync(byte[] imageData)
    {
        try
        {
            var prompt = $@"
                As a computer vision expert, analyze screenshot for web automation:
                
                Please provide:
                1. Page structure and layout analysis
                2. Interactive elements identification
                3. Text content and readability
                4. Visual hierarchy and organization
                5. Automation opportunities and challenges
                6. Element positioning and relationships
                7. Color schemes and accessibility
                8. Performance and loading indicators
                
                Format as a comprehensive visual analysis report.
            ";

            var result = await CallOpenAIAsync(prompt);
            return new Dictionary<string, object> { { "analysis", result } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing screenshot");
            return new Dictionary<string, object> { { "error", "Screenshot analysis failed" } };
        }
    }

    public async Task<List<Dictionary<string, object>>> DetectWebElementsAsync(byte[] screenshot)
    {
        try
        {
            var prompt = $@"
                As a web element detection expert, detect web elements in screenshot:
                
                Please identify:
                1. Buttons and clickable elements
                2. Form fields and inputs
                3. Navigation elements and menus
                4. Content areas and sections
                5. Interactive components and widgets
                6. Links and hyperlinks
                7. Images and media elements
                8. Layout containers and grids
                
                For each element, provide:
                - Element type and classification
                - Position and bounding box
                - Attributes and properties
                - Interaction capabilities
                - Confidence scores
                
                Format as a structured element detection report.
            ";

            var result = await CallOpenAIAsync(prompt);
            return new List<Dictionary<string, object>> { new Dictionary<string, object> { { "elements", result } } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting web elements");
            return new List<Dictionary<string, object>> { new Dictionary<string, object> { { "error", "Element detection failed" } } };
        }
    }

    public async Task<string> OCRTextFromImageAsync(byte[] imageData)
    {
        try
        {
            var prompt = $@"
                As an OCR text extraction expert, extract text from image using OCR:
                
                Please provide:
                1. All visible text content
                2. Text coordinates and positions
                3. Text confidence scores
                4. Language detection and identification
                5. Formatting information and structure
                6. Text quality and readability
                7. Special characters and symbols
                8. Layout and organization
                
                Format as a comprehensive text extraction report.
            ";

            return await CallOpenAIAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from image");
            return "OCR text extraction failed: " + ex.Message;
        }
    }

    public async Task<Dictionary<string, object>> CompareImagesAsync(byte[] image1, byte[] image2)
    {
        try
        {
            var prompt = $@"
                As an image comparison expert, compare two images for differences:
                
                Please provide:
                1. Visual differences and changes
                2. Structural changes and modifications
                3. Content modifications and updates
                4. Layout changes and reorganization
                5. Similarity score and confidence
                6. Change categorization and priority
                7. Impact assessment and implications
                8. Recommendations and next steps
                
                Format as a comprehensive image comparison report.
            ";

            var result = await CallOpenAIAsync(prompt);
            return new Dictionary<string, object> { { "comparison", result } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing images");
            return new Dictionary<string, object> { { "error", "Image comparison failed" } };
        }
    }

    private async Task<string> CallOpenAIAsync(string prompt)
    {
        try
        {
            var request = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "You are an expert AI assistant specializing in industrial automation, test execution, and web automation. Provide detailed, technical, and actionable responses." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 2000,
                temperature = 0.7,
                top_p = 1.0,
                frequency_penalty = 0.0,
                presence_penalty = 0.0
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(result);
                return openAIResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "No response from AI";
            }
            else
            {
                _logger.LogWarning("OpenAI API returned error: {StatusCode} - {Content}", response.StatusCode, result);
                return "AI service temporarily unavailable";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenAI API");
            return "AI service error: " + ex.Message;
        }
    }
}

public class OpenAIResponse
{
    public List<OpenAIChoice>? Choices { get; set; }
}

public class OpenAIChoice
{
    public OpenAIMessage? Message { get; set; }
}

public class OpenAIMessage
{
    public string? Content { get; set; }
}
