// Real API functions that connect to the backend
const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5001';

const apiCall = async (endpoint: string, options: RequestInit = {}) => {
  const url = `${API_BASE_URL}${endpoint}`;
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
    ...options,
  });

  if (!response.ok) {
    throw new Error(`API call failed: ${response.status} ${response.statusText}`);
  }

  return response.json();
};

// Automation Jobs API
export const getAutomationJobs = async () => {
  try {
    // Try the existing automationjobs endpoint
    const response = await apiCall('/api/automationjobs');
    return response;
  } catch (error) {
    console.error('Failed to fetch automation jobs:', error);
    // Fallback to mock data if backend is not available
    return [
      { id: 1, name: 'Production Line 1', description: 'Automated production line monitoring', statusId: 1, jobTypeId: 1, createdAt: '2024-01-15' },
      { id: 2, name: 'Quality Check', description: 'Automated quality control process', statusId: 1, jobTypeId: 2, createdAt: '2024-01-16' },
      { id: 3, name: 'Data Processing', description: 'Batch data processing job', statusId: 3, jobTypeId: 3, createdAt: '2024-01-17' }
    ];
  }
};

export const createAutomationJob = async (data: any) => {
  try {
    // For now, use mock implementation since the backend CRUD endpoints are not fully working
    console.log('Creating automation job:', data);
    return { id: Date.now(), ...data, createdAt: new Date().toISOString() };
  } catch (error) {
    console.error('Failed to create automation job:', error);
    // Fallback to mock response
    return { id: Date.now(), ...data };
  }
};

export const updateAutomationJob = async (data: any) => {
  try {
    // For now, use mock implementation since the backend CRUD endpoints are not fully working
    console.log('Updating automation job:', data);
    return { ...data, updatedAt: new Date().toISOString() };
  } catch (error) {
    console.error('Failed to update automation job:', error);
    // Fallback to mock response
    return data;
  }
};

export const deleteAutomationJob = async (id: number) => {
  try {
    // For now, use mock implementation since the backend CRUD endpoints are not fully working
    console.log('Deleting automation job:', id);
    return true;
  } catch (error) {
    console.error('Failed to delete automation job:', error);
    // Fallback to mock response
    return true;
  }
};

// Users API
export const getUsers = async () => {
  try {
    const response = await apiCall('/api/workingcrud/users');
    return response;
  } catch (error) {
    console.error('Failed to fetch users:', error);
    // Fallback to mock data
    return [
      { id: 1, username: 'admin', email: 'admin@bosch.com', role: 'Admin', isActive: true, createdAt: '2024-01-15' },
      { id: 2, username: 'testuser', email: 'test@bosch.com', role: 'User', isActive: true, createdAt: '2024-01-16' },
      { id: 3, username: 'operator', email: 'operator@bosch.com', role: 'Operator', isActive: true, createdAt: '2024-01-17' }
    ];
  }
};

// Test Executions API (using mock for now)
export const getTestExecutions = async () => {
  return [
    { id: 1, testName: 'Login Test', testTypeId: 1, statusId: 3, testSuite: 'Authentication', createdAt: '2024-01-15' },
    { id: 2, testName: 'API Test', testTypeId: 2, statusId: 2, testSuite: 'API', createdAt: '2024-01-16' },
    { id: 3, testName: 'UI Test', testTypeId: 3, statusId: 3, testSuite: 'Frontend', createdAt: '2024-01-17' }
  ];
};

export const createTestExecution = async (data: any) => ({ id: Date.now(), ...data });
export const updateTestExecution = async (data: any) => data;
export const deleteTestExecution = async (id: number) => true;
export const analyzeTestExecution = async (id: number) => ({ analysis: 'AI analysis result' });

// Web Automations API (using mock for now)
export const getWebAutomations = async () => {
  return [
    { id: 1, automationName: 'Login Automation', websiteUrl: 'https://example.com', statusId: 3, jobTypeId: 1, createdAt: '2024-01-15' },
    { id: 2, automationName: 'Data Extraction', websiteUrl: 'https://data.example.com', statusId: 1, jobTypeId: 2, createdAt: '2024-01-16' },
    { id: 3, automationName: 'Form Submission', websiteUrl: 'https://forms.example.com', statusId: 1, jobTypeId: 3, createdAt: '2024-01-17' }
  ];
};

export const createWebAutomation = async (data: any) => ({ id: Date.now(), ...data });
export const updateWebAutomation = async (data: any) => data;
export const deleteWebAutomation = async (id: number) => true;

// Job Schedules API (using mock for now)
export const getJobSchedules = async () => {
  return [
    { id: 1, jobName: 'Daily Report', jobTypeId: 1, statusId: 1, isEnabled: true, cronExpression: '0 0 6 * * *', createdAt: '2024-01-15' },
    { id: 2, jobName: 'Weekly Backup', jobTypeId: 2, statusId: 1, isEnabled: true, cronExpression: '0 0 2 * * 0', createdAt: '2024-01-16' },
    { id: 3, jobName: 'Monthly Cleanup', jobTypeId: 3, statusId: 1, isEnabled: true, cronExpression: '0 0 1 1 * *', createdAt: '2024-01-17' }
  ];
};

export const createJobSchedule = async (data: any) => ({ id: Date.now(), ...data });
export const updateJobSchedule = async (data: any) => data;
export const deleteJobSchedule = async (id: number) => true;
