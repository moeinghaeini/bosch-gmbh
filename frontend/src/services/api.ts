import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5001/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Automation Jobs API
export const getAutomationJobs = async () => {
  const response = await api.get('/automationjobs');
  return response.data;
};

export const getAutomationJob = async (id: number) => {
  const response = await api.get(`/automationjobs/${id}`);
  return response.data;
};

export const createAutomationJob = async (job: any) => {
  const response = await api.post('/automationjobs', job);
  return response.data;
};

export const updateAutomationJob = async (job: any) => {
  const response = await api.put(`/automationjobs/${job.id}`, job);
  return response.data;
};

export const deleteAutomationJob = async (id: number) => {
  await api.delete(`/automationjobs/${id}`);
};

export const getAutomationJobsByStatus = async (status: string) => {
  const response = await api.get(`/automationjobs/status/${status}`);
  return response.data;
};

export const getAutomationJobsByType = async (jobType: string) => {
  const response = await api.get(`/automationjobs/type/${jobType}`);
  return response.data;
};

// Users API
export const getUsers = async () => {
  const response = await api.get('/users');
  return response.data;
};

export const getUser = async (id: number) => {
  const response = await api.get(`/users/${id}`);
  return response.data;
};

export const createUser = async (user: any) => {
  const response = await api.post('/users', user);
  return response.data;
};

export const updateUser = async (user: any) => {
  const response = await api.put(`/users/${user.id}`, user);
  return response.data;
};

export const deleteUser = async (id: number) => {
  await api.delete(`/users/${id}`);
};

// Test Executions API
export const getTestExecutions = async () => {
  const response = await api.get('/testexecutions');
  return response.data;
};

export const getTestExecution = async (id: number) => {
  const response = await api.get(`/testexecutions/${id}`);
  return response.data;
};

export const createTestExecution = async (data: any) => {
  const response = await api.post('/testexecutions', data);
  return response.data;
};

export const updateTestExecution = async (data: any) => {
  const response = await api.put(`/testexecutions/${data.id}`, data);
  return response.data;
};

export const deleteTestExecution = async (id: number) => {
  await api.delete(`/testexecutions/${id}`);
};

export const analyzeTestExecution = async (id: number) => {
  const response = await api.post(`/testexecutions/${id}/analyze`);
  return response.data;
};

// Web Automations API
export const getWebAutomations = async () => {
  const response = await api.get('/webautomations');
  return response.data;
};

export const getWebAutomation = async (id: number) => {
  const response = await api.get(`/webautomations/${id}`);
  return response.data;
};

export const createWebAutomation = async (data: any) => {
  const response = await api.post('/webautomations', data);
  return response.data;
};

export const updateWebAutomation = async (data: any) => {
  const response = await api.put(`/webautomations/${data.id}`, data);
  return response.data;
};

export const deleteWebAutomation = async (id: number) => {
  await api.delete(`/webautomations/${id}`);
};

// Job Schedules API
export const getJobSchedules = async () => {
  const response = await api.get('/jobschedules');
  return response.data;
};

export const getJobSchedule = async (id: number) => {
  const response = await api.get(`/jobschedules/${id}`);
  return response.data;
};

export const createJobSchedule = async (data: any) => {
  const response = await api.post('/jobschedules', data);
  return response.data;
};

export const updateJobSchedule = async (data: any) => {
  const response = await api.put(`/jobschedules/${data.id}`, data);
  return response.data;
};

export const deleteJobSchedule = async (id: number) => {
  await api.delete(`/jobschedules/${id}`);
};

export default api;
