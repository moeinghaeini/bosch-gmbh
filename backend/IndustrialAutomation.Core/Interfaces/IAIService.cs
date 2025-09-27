namespace IndustrialAutomation.Core.Interfaces;

public interface IAIService
{
    // Test Execution AI methods
    Task<string> AnalyzeTestResultsAsync(string testResults, string testType);
    Task<string> GenerateTestCasesAsync(string requirements, string testType);
    Task<string> OptimizeTestSuiteAsync(string testSuite);
    Task<string> PredictTestOutcomeAsync(string testData, string testType);
    Task<List<string>> ClassifyLogErrorsAsync(string logData);
    Task<Dictionary<string, double>> PredictTestFailureAsync(string testMetadata);
    Task<List<string>> ClusterTestResultsAsync(List<string> testResults);
    Task<string> DetectAnomaliesAsync(string testData);

    // Web Automation AI methods
    Task<string> AnalyzeWebPageAsync(string url, string prompt);
    Task<string> IdentifyWebElementAsync(string pageContent, string description);
    Task<string> GenerateWebSelectorAsync(string elementDescription, string pageContent);
    Task<string> ValidateWebActionAsync(string action, string element, string pageContent);
    Task<string> ExtractDataFromWebAsync(string pageContent, string extractionPrompt);
    Task<string> GenerateAutomationScriptAsync(string requirements, string targetWebsite);
    Task<string> AnalyzeAutomationResultsAsync(string results, string expectedOutcome);
    Task<string> OptimizeAutomationFlowAsync(string currentFlow, string performanceData);
    Task<Dictionary<string, object>> ExtractWebElementsAsync(string screenshotPath, string description);
    Task<List<string>> RecognizeWebElementsAsync(byte[] screenshot);

    // Natural Language Processing methods
    Task<string> ProcessNaturalLanguageCommandAsync(string command);
    Task<Dictionary<string, object>> ParseUserIntentAsync(string userInput);
    Task<string> GenerateActionPlanAsync(string naturalLanguageTask);
    Task<List<string>> ExtractEntitiesAsync(string text);
    Task<string> TranslateIntentToActionsAsync(string intent);

    // ML Model Management methods
    Task<bool> TrainModelAsync(string modelType, string trainingData);
    Task<double> EvaluateModelAsync(string modelName, string testData);
    Task<bool> DeployModelAsync(string modelName, string version);
    Task<Dictionary<string, object>> GetModelMetricsAsync(string modelName);
    Task<List<string>> GetAvailableModelsAsync();

    // Computer Vision methods
    Task<Dictionary<string, object>> AnalyzeScreenshotAsync(byte[] imageData);
    Task<List<Dictionary<string, object>>> DetectWebElementsAsync(byte[] screenshot);
    Task<string> OCRTextFromImageAsync(byte[] imageData);
    Task<Dictionary<string, object>> CompareImagesAsync(byte[] image1, byte[] image2);
}
