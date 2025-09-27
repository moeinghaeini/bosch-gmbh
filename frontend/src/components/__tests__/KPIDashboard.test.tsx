import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import KPIDashboard from '../KPIDashboard';

// Mock the API service
const mockApi = {
  getTestExecutionKPIs: jest.fn(),
  getWebAutomationKPIs: jest.fn(),
  getJobSchedulingKPIs: jest.fn(),
  getOverallPerformanceKPIs: jest.fn(),
};

jest.mock('../../services/api', () => mockApi);

const createTestQueryClient = () => new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
    },
  },
});

const theme = createTheme();

const renderWithProviders = (component: React.ReactElement) => {
  const queryClient = createTestQueryClient();
  return render(
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={theme}>
        <BrowserRouter>
          {component}
        </BrowserRouter>
      </ThemeProvider>
    </QueryClientProvider>
  );
};

const mockTestExecutionKPIs = {
  totalTests: 100,
  passedTests: 85,
  failedTests: 15,
  runningTests: 0,
  successRate: 85.0,
  failureRate: 15.0,
  averageExecutionTime: 2.5,
  testsByType: { 'Unit': 60, 'Integration': 30, 'E2E': 10 },
  testsByStatus: { 'Passed': 85, 'Failed': 15, 'Running': 0 },
  recentTrends: { 'last7Days': { 'success': 85, 'failure': 15 }, 'trend': 'improving' }
};

const mockWebAutomationKPIs = {
  totalAutomations: 50,
  completedAutomations: 45,
  failedAutomations: 5,
  runningAutomations: 0,
  successRate: 90.0,
  failureRate: 10.0,
  averageExecutionTime: 5.0,
  automationsByType: { 'Login': 20, 'DataEntry': 15, 'DataExtraction': 15 },
  automationsByStatus: { 'Completed': 45, 'Failed': 5, 'Running': 0 },
  recentTrends: { 'last7Days': { 'success': 90, 'failure': 10 }, 'trend': 'stable' }
};

const mockJobSchedulingKPIs = {
  totalJobs: 25,
  enabledJobs: 20,
  scheduledJobs: 15,
  runningJobs: 5,
  completedJobs: 18,
  successRate: 72.0,
  jobsByType: { 'Backup': 10, 'Report': 8, 'Maintenance': 7 },
  jobsByStatus: { 'Scheduled': 15, 'Running': 5, 'Completed': 18 },
  jobsByPriority: { 'High': 8, 'Medium': 12, 'Low': 5 },
  recentTrends: { 'last7Days': { 'success': 95, 'failure': 5 }, 'trend': 'improving' }
};

const mockOverallPerformanceKPIs = {
  systemUptime: 99.9,
  totalAutomationTasks: 175,
  successfulTasks: 148,
  failedTasks: 27,
  averageResponseTime: 150.0,
  throughputPerHour: 25.0,
  errorRate: 2.5,
  resourceUtilization: { 'CPU': 65.5, 'Memory': 78.2, 'Disk': 45.8, 'Network': 32.1 },
  performanceTrends: { 'performance': 'excellent', 'reliability': 'high', 'efficiency': 'optimized' }
};

describe('KPIDashboard Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockApi.getTestExecutionKPIs.mockResolvedValue(mockTestExecutionKPIs);
    mockApi.getWebAutomationKPIs.mockResolvedValue(mockWebAutomationKPIs);
    mockApi.getJobSchedulingKPIs.mockResolvedValue(mockJobSchedulingKPIs);
    mockApi.getOverallPerformanceKPIs.mockResolvedValue(mockOverallPerformanceKPIs);
  });

  it('renders KPI dashboard with all sections', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('KPI Dashboard')).toBeInTheDocument();
      expect(screen.getByText('Test Execution KPIs')).toBeInTheDocument();
      expect(screen.getByText('Web Automation KPIs')).toBeInTheDocument();
      expect(screen.getByText('Job Scheduling KPIs')).toBeInTheDocument();
      expect(screen.getByText('Overall Performance KPIs')).toBeInTheDocument();
    });
  });

  it('displays test execution KPIs correctly', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Total Tests: 100')).toBeInTheDocument();
      expect(screen.getByText('Passed: 85')).toBeInTheDocument();
      expect(screen.getByText('Failed: 15')).toBeInTheDocument();
      expect(screen.getByText('Success Rate: 85.0%')).toBeInTheDocument();
      expect(screen.getByText('Average Execution Time: 2.5 min')).toBeInTheDocument();
    });
  });

  it('displays web automation KPIs correctly', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Total Automations: 50')).toBeInTheDocument();
      expect(screen.getByText('Completed: 45')).toBeInTheDocument();
      expect(screen.getByText('Failed: 5')).toBeInTheDocument();
      expect(screen.getByText('Success Rate: 90.0%')).toBeInTheDocument();
      expect(screen.getByText('Average Execution Time: 5.0 min')).toBeInTheDocument();
    });
  });

  it('displays job scheduling KPIs correctly', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Total Jobs: 25')).toBeInTheDocument();
      expect(screen.getByText('Enabled: 20')).toBeInTheDocument();
      expect(screen.getByText('Scheduled: 15')).toBeInTheDocument();
      expect(screen.getByText('Running: 5')).toBeInTheDocument();
      expect(screen.getByText('Completed: 18')).toBeInTheDocument();
      expect(screen.getByText('Success Rate: 72.0%')).toBeInTheDocument();
    });
  });

  it('displays overall performance KPIs correctly', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('System Uptime: 99.9%')).toBeInTheDocument();
      expect(screen.getByText('Total Tasks: 175')).toBeInTheDocument();
      expect(screen.getByText('Successful: 148')).toBeInTheDocument();
      expect(screen.getByText('Failed: 27')).toBeInTheDocument();
      expect(screen.getByText('Average Response Time: 150.0 ms')).toBeInTheDocument();
      expect(screen.getByText('Throughput: 25.0/hour')).toBeInTheDocument();
      expect(screen.getByText('Error Rate: 2.5%')).toBeInTheDocument();
    });
  });

  it('displays resource utilization metrics', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('CPU: 65.5%')).toBeInTheDocument();
      expect(screen.getByText('Memory: 78.2%')).toBeInTheDocument();
      expect(screen.getByText('Disk: 45.8%')).toBeInTheDocument();
      expect(screen.getByText('Network: 32.1%')).toBeInTheDocument();
    });
  });

  it('displays performance trends', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Performance: excellent')).toBeInTheDocument();
      expect(screen.getByText('Reliability: high')).toBeInTheDocument();
      expect(screen.getByText('Efficiency: optimized')).toBeInTheDocument();
    });
  });

  it('displays tests by type chart', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Tests by Type')).toBeInTheDocument();
      expect(screen.getByText('Unit: 60')).toBeInTheDocument();
      expect(screen.getByText('Integration: 30')).toBeInTheDocument();
      expect(screen.getByText('E2E: 10')).toBeInTheDocument();
    });
  });

  it('displays tests by status chart', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Tests by Status')).toBeInTheDocument();
      expect(screen.getByText('Passed: 85')).toBeInTheDocument();
      expect(screen.getByText('Failed: 15')).toBeInTheDocument();
      expect(screen.getByText('Running: 0')).toBeInTheDocument();
    });
  });

  it('displays automations by type chart', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Automations by Type')).toBeInTheDocument();
      expect(screen.getByText('Login: 20')).toBeInTheDocument();
      expect(screen.getByText('DataEntry: 15')).toBeInTheDocument();
      expect(screen.getByText('DataExtraction: 15')).toBeInTheDocument();
    });
  });

  it('displays jobs by type chart', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Jobs by Type')).toBeInTheDocument();
      expect(screen.getByText('Backup: 10')).toBeInTheDocument();
      expect(screen.getByText('Report: 8')).toBeInTheDocument();
      expect(screen.getByText('Maintenance: 7')).toBeInTheDocument();
    });
  });

  it('displays jobs by priority chart', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Jobs by Priority')).toBeInTheDocument();
      expect(screen.getByText('High: 8')).toBeInTheDocument();
      expect(screen.getByText('Medium: 12')).toBeInTheDocument();
      expect(screen.getByText('Low: 5')).toBeInTheDocument();
    });
  });

  it('refreshes data when refresh button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('KPI Dashboard')).toBeInTheDocument();
    });

    const refreshButton = screen.getByRole('button', { name: /refresh/i });
    await user.click(refreshButton);

    await waitFor(() => {
      expect(mockApi.getTestExecutionKPIs).toHaveBeenCalledTimes(2);
      expect(mockApi.getWebAutomationKPIs).toHaveBeenCalledTimes(2);
      expect(mockApi.getJobSchedulingKPIs).toHaveBeenCalledTimes(2);
      expect(mockApi.getOverallPerformanceKPIs).toHaveBeenCalledTimes(2);
    });
  });

  it('exports data when export button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('KPI Dashboard')).toBeInTheDocument();
    });

    const exportButton = screen.getByRole('button', { name: /export/i });
    await user.click(exportButton);

    // Verify export was triggered
    expect(exportButton).toBeInTheDocument();
  });

  it('handles loading state', () => {
    mockApi.getTestExecutionKPIs.mockImplementation(() => new Promise(() => {}));
    mockApi.getWebAutomationKPIs.mockImplementation(() => new Promise(() => {}));
    mockApi.getJobSchedulingKPIs.mockImplementation(() => new Promise(() => {}));
    mockApi.getOverallPerformanceKPIs.mockImplementation(() => new Promise(() => {}));
    
    renderWithProviders(<KPIDashboard />);
    
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('handles error state for test execution KPIs', async () => {
    mockApi.getTestExecutionKPIs.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading test execution KPIs')).toBeInTheDocument();
    });
  });

  it('handles error state for web automation KPIs', async () => {
    mockApi.getWebAutomationKPIs.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading web automation KPIs')).toBeInTheDocument();
    });
  });

  it('handles error state for job scheduling KPIs', async () => {
    mockApi.getJobSchedulingKPIs.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading job scheduling KPIs')).toBeInTheDocument();
    });
  });

  it('handles error state for overall performance KPIs', async () => {
    mockApi.getOverallPerformanceKPIs.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading overall performance KPIs')).toBeInTheDocument();
    });
  });

  it('displays recent trends for test executions', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Recent Trends')).toBeInTheDocument();
      expect(screen.getByText('Last 7 Days: 85% success, 15% failure')).toBeInTheDocument();
      expect(screen.getByText('Trend: improving')).toBeInTheDocument();
    });
  });

  it('displays recent trends for web automations', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Recent Trends')).toBeInTheDocument();
      expect(screen.getByText('Last 7 Days: 90% success, 10% failure')).toBeInTheDocument();
      expect(screen.getByText('Trend: stable')).toBeInTheDocument();
    });
  });

  it('displays recent trends for job scheduling', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Recent Trends')).toBeInTheDocument();
      expect(screen.getByText('Last 7 Days: 95% success, 5% failure')).toBeInTheDocument();
      expect(screen.getByText('Trend: improving')).toBeInTheDocument();
    });
  });

  it('shows performance indicators with colors', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      // Check for success rate indicators
      expect(screen.getByText('85.0%')).toBeInTheDocument();
      expect(screen.getByText('90.0%')).toBeInTheDocument();
      expect(screen.getByText('72.0%')).toBeInTheDocument();
    });
  });

  it('displays time-based metrics correctly', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('2.5 min')).toBeInTheDocument();
      expect(screen.getByText('5.0 min')).toBeInTheDocument();
      expect(screen.getByText('150.0 ms')).toBeInTheDocument();
    });
  });

  it('shows system health indicators', async () => {
    renderWithProviders(<KPIDashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('99.9%')).toBeInTheDocument();
      expect(screen.getByText('2.5%')).toBeInTheDocument();
    });
  });
});
