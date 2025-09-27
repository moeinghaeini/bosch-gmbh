// Mock API functions for demonstration purposes
export const getAutomationJobs = async () => {
  return [
    { id: 1, name: 'Data Processing Job', status: 'Completed', type: 'DataProcessing', createdAt: '2024-01-15' },
    { id: 2, name: 'Report Generation', status: 'Running', type: 'ReportGeneration', createdAt: '2024-01-16' },
    { id: 3, name: 'Email Automation', status: 'Pending', type: 'EmailAutomation', createdAt: '2024-01-17' }
  ];
};

export const getUsers = async () => {
  return [
    { id: 1, username: 'admin', email: 'admin@bosch.com', role: 'Administrator' },
    { id: 2, username: 'developer', email: 'dev@bosch.com', role: 'Developer' },
    { id: 3, username: 'tester', email: 'tester@bosch.com', role: 'Tester' }
  ];
};

export const getTestExecutions = async () => {
  return [
    { id: 1, testName: 'Login Test', testType: 'E2E', status: 'Passed', testSuite: 'Authentication', createdAt: '2024-01-15' },
    { id: 2, testName: 'API Test', testType: 'Integration', status: 'Failed', testSuite: 'API', createdAt: '2024-01-16' },
    { id: 3, testName: 'UI Test', testType: 'Unit', status: 'Passed', testSuite: 'Frontend', createdAt: '2024-01-17' }
  ];
};

export const getWebAutomations = async () => {
  return [
    { id: 1, automationName: 'Data Extraction', websiteUrl: 'https://example.com', status: 'Completed', automationType: 'DataExtraction', createdAt: '2024-01-15' },
    { id: 2, automationName: 'Form Filling', websiteUrl: 'https://forms.com', status: 'Running', automationType: 'FormFilling', createdAt: '2024-01-16' },
    { id: 3, automationName: 'Navigation', websiteUrl: 'https://portal.com', status: 'Pending', automationType: 'Navigation', createdAt: '2024-01-17' }
  ];
};

export const getJobSchedules = async () => {
  return [
    { id: 1, jobName: 'Daily Report', jobType: 'ReportGeneration', status: 'Scheduled', cronExpression: '0 0 * * *', isEnabled: true, createdAt: '2024-01-15' },
    { id: 2, jobName: 'Data Sync', jobType: 'DataProcessing', status: 'Running', cronExpression: '0 */6 * * *', isEnabled: true, createdAt: '2024-01-16' },
    { id: 3, jobName: 'Backup Job', jobType: 'Backup', status: 'Scheduled', cronExpression: '0 2 * * *', isEnabled: false, createdAt: '2024-01-17' }
  ];
};

// Mock CRUD operations
export const createAutomationJob = async (data: any) => ({ id: Date.now(), ...data });
export const updateAutomationJob = async (data: any) => data;
export const deleteAutomationJob = async (id: number) => true;

export const createUser = async (data: any) => ({ id: Date.now(), ...data });
export const updateUser = async (data: any) => data;
export const deleteUser = async (id: number) => true;

export const createTestExecution = async (data: any) => ({ id: Date.now(), ...data });
export const updateTestExecution = async (data: any) => data;
export const deleteTestExecution = async (id: number) => true;
export const analyzeTestExecution = async (id: number) => ({ analysis: 'AI analysis result' });

export const createWebAutomation = async (data: any) => ({ id: Date.now(), ...data });
export const updateWebAutomation = async (data: any) => data;
export const deleteWebAutomation = async (id: number) => true;

export const createJobSchedule = async (data: any) => ({ id: Date.now(), ...data });
export const updateJobSchedule = async (data: any) => data;
export const deleteJobSchedule = async (id: number) => true;
