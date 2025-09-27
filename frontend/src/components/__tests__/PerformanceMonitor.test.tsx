import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import PerformanceMonitor from '../PerformanceMonitor';

// Mock the API service
const mockApi = {
  getSystemHealth: jest.fn(),
  getPerformanceMetrics: jest.fn(),
  getBusinessMetrics: jest.fn(),
  getAlerts: jest.fn(),
  getDashboardData: jest.fn(),
  getLogs: jest.fn(),
  getTrace: jest.fn(),
  recordMetric: jest.fn(),
  recordEvent: jest.fn(),
  recordError: jest.fn(),
  createAlert: jest.fn(),
  startTrace: jest.fn(),
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

const mockSystemHealth = {
  status: 'healthy',
  uptime: 99.9,
  services: ['database', 'redis', 'api'],
  lastCheck: '2024-01-01T00:00:00Z'
};

const mockPerformanceMetrics = {
  cpu_usage: 65.5,
  memory_usage: 78.2,
  disk_usage: 45.8,
  network_usage: 32.1,
  response_time: 150.0,
  throughput: 25.0
};

const mockBusinessMetrics = {
  total_jobs: 100,
  successful_jobs: 95,
  failed_jobs: 5,
  active_users: 50,
  revenue: 10000
};

const mockAlerts = [
  {
    id: 1,
    message: 'High CPU usage detected',
    severity: 'warning',
    timestamp: '2024-01-01T00:00:00Z',
    status: 'active'
  },
  {
    id: 2,
    message: 'Database connection failed',
    severity: 'error',
    timestamp: '2024-01-01T00:00:00Z',
    status: 'active'
  }
];

const mockDashboardData = {
  system_health: 'good',
  active_jobs: 5,
  recent_errors: 0,
  performance_score: 85
};

const mockLogs = [
  {
    id: 1,
    level: 'info',
    message: 'User logged in',
    timestamp: '2024-01-01T00:00:00Z',
    source: 'auth'
  },
  {
    id: 2,
    level: 'error',
    message: 'Database connection failed',
    timestamp: '2024-01-01T00:00:00Z',
    source: 'database'
  }
];

const mockTrace = {
  id: 'trace-123',
  duration: 150.5,
  spans: ['span1', 'span2'],
  status: 'completed'
};

describe('PerformanceMonitor Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockApi.getSystemHealth.mockResolvedValue(mockSystemHealth);
    mockApi.getPerformanceMetrics.mockResolvedValue(mockPerformanceMetrics);
    mockApi.getBusinessMetrics.mockResolvedValue(mockBusinessMetrics);
    mockApi.getAlerts.mockResolvedValue(mockAlerts);
    mockApi.getDashboardData.mockResolvedValue(mockDashboardData);
    mockApi.getLogs.mockResolvedValue(mockLogs);
    mockApi.getTrace.mockResolvedValue(mockTrace);
  });

  it('renders performance monitor with all sections', async () => {
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Performance Monitor')).toBeInTheDocument();
      expect(screen.getByText('System Health')).toBeInTheDocument();
      expect(screen.getByText('Performance Metrics')).toBeInTheDocument();
      expect(screen.getByText('Business Metrics')).toBeInTheDocument();
      expect(screen.getByText('Alerts')).toBeInTheDocument();
      expect(screen.getByText('Logs')).toBeInTheDocument();
    });
  });

  it('displays system health status', async () => {
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Status: healthy')).toBeInTheDocument();
      expect(screen.getByText('Uptime: 99.9%')).toBeInTheDocument();
      expect(screen.getByText('Services: database, redis, api')).toBeInTheDocument();
    });
  });

  it('displays performance metrics', async () => {
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('CPU Usage: 65.5%')).toBeInTheDocument();
      expect(screen.getByText('Memory Usage: 78.2%')).toBeInTheDocument();
      expect(screen.getByText('Disk Usage: 45.8%')).toBeInTheDocument();
      expect(screen.getByText('Network Usage: 32.1%')).toBeInTheDocument();
      expect(screen.getByText('Response Time: 150.0 ms')).toBeInTheDocument();
      expect(screen.getByText('Throughput: 25.0 req/s')).toBeInTheDocument();
    });
  });

  it('displays business metrics', async () => {
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Total Jobs: 100')).toBeInTheDocument();
      expect(screen.getByText('Successful Jobs: 95')).toBeInTheDocument();
      expect(screen.getByText('Failed Jobs: 5')).toBeInTheDocument();
      expect(screen.getByText('Active Users: 50')).toBeInTheDocument();
      expect(screen.getByText('Revenue: $10,000')).toBeInTheDocument();
    });
  });

  it('displays alerts', async () => {
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('High CPU usage detected')).toBeInTheDocument();
      expect(screen.getByText('Database connection failed')).toBeInTheDocument();
      expect(screen.getByText('Warning')).toBeInTheDocument();
      expect(screen.getByText('Error')).toBeInTheDocument();
    });
  });

  it('displays logs', async () => {
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('User logged in')).toBeInTheDocument();
      expect(screen.getByText('Database connection failed')).toBeInTheDocument();
      expect(screen.getByText('INFO')).toBeInTheDocument();
      expect(screen.getByText('ERROR')).toBeInTheDocument();
    });
  });

  it('filters logs by level', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Logs')).toBeInTheDocument();
    });

    const levelFilter = screen.getByLabelText(/level/i);
    await user.click(levelFilter);
    
    const levelOption = screen.getByText('Error');
    await user.click(levelOption);

    await waitFor(() => {
      expect(screen.getByText('Database connection failed')).toBeInTheDocument();
      expect(screen.queryByText('User logged in')).not.toBeInTheDocument();
    });
  });

  it('filters logs by date range', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Logs')).toBeInTheDocument();
    });

    const fromDateInput = screen.getByLabelText(/from date/i);
    const toDateInput = screen.getByLabelText(/to date/i);
    
    await user.type(fromDateInput, '2024-01-01');
    await user.type(toDateInput, '2024-01-31');

    const filterButton = screen.getByRole('button', { name: /filter/i });
    await user.click(filterButton);

    await waitFor(() => {
      expect(mockApi.getLogs).toHaveBeenCalledWith(
        1, 50, null, expect.any(Date), expect.any(Date)
      );
    });
  });

  it('searches logs by message', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Logs')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText(/search logs/i);
    await user.type(searchInput, 'database');

    await waitFor(() => {
      expect(screen.getByText('Database connection failed')).toBeInTheDocument();
      expect(screen.queryByText('User logged in')).not.toBeInTheDocument();
    });
  });

  it('views trace details when trace is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Logs')).toBeInTheDocument();
    });

    const traceButton = screen.getByRole('button', { name: /view trace/i });
    await user.click(traceButton);

    await waitFor(() => {
      expect(screen.getByText('Trace Details')).toBeInTheDocument();
      expect(screen.getByText('Duration: 150.5 ms')).toBeInTheDocument();
      expect(screen.getByText('Status: completed')).toBeInTheDocument();
    });
  });

  it('records a custom metric', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Performance Monitor')).toBeInTheDocument();
    });

    const recordMetricButton = screen.getByRole('button', { name: /record metric/i });
    await user.click(recordMetricButton);

    const metricNameInput = screen.getByLabelText(/metric name/i);
    const metricValueInput = screen.getByLabelText(/value/i);
    const submitButton = screen.getByRole('button', { name: /record/i });

    await user.type(metricNameInput, 'custom_metric');
    await user.type(metricValueInput, '100');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.recordMetric).toHaveBeenCalledWith(
        expect.objectContaining({
          metricName: 'custom_metric',
          value: 100,
        })
      );
    });
  });

  it('records a custom event', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Performance Monitor')).toBeInTheDocument();
    });

    const recordEventButton = screen.getByRole('button', { name: /record event/i });
    await user.click(recordEventButton);

    const eventNameInput = screen.getByLabelText(/event name/i);
    const submitButton = screen.getByRole('button', { name: /record/i });

    await user.type(eventNameInput, 'custom_event');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.recordEvent).toHaveBeenCalledWith(
        expect.objectContaining({
          eventName: 'custom_event',
        })
      );
    });
  });

  it('records an error', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Performance Monitor')).toBeInTheDocument();
    });

    const recordErrorButton = screen.getByRole('button', { name: /record error/i });
    await user.click(recordErrorButton);

    const errorMessageInput = screen.getByLabelText(/error message/i);
    const submitButton = screen.getByRole('button', { name: /record/i });

    await user.type(errorMessageInput, 'Custom error message');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.recordError).toHaveBeenCalledWith(
        expect.objectContaining({
          errorMessage: 'Custom error message',
        })
      );
    });
  });

  it('creates a new alert', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Performance Monitor')).toBeInTheDocument();
    });

    const createAlertButton = screen.getByRole('button', { name: /create alert/i });
    await user.click(createAlertButton);

    const alertNameInput = screen.getByLabelText(/alert name/i);
    const descriptionInput = screen.getByLabelText(/description/i);
    const severityInput = screen.getByLabelText(/severity/i);
    const submitButton = screen.getByRole('button', { name: /create/i });

    await user.type(alertNameInput, 'Custom Alert');
    await user.type(descriptionInput, 'Custom alert description');
    await user.selectOptions(severityInput, 'warning');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.createAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          alertName: 'Custom Alert',
          description: 'Custom alert description',
          severity: 'warning',
        })
      );
    });
  });

  it('starts a new trace', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Performance Monitor')).toBeInTheDocument();
    });

    const startTraceButton = screen.getByRole('button', { name: /start trace/i });
    await user.click(startTraceButton);

    const traceNameInput = screen.getByLabelText(/trace name/i);
    const submitButton = screen.getByRole('button', { name: /start/i });

    await user.type(traceNameInput, 'custom_trace');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.startTrace).toHaveBeenCalledWith(
        expect.objectContaining({
          traceName: 'custom_trace',
        })
      );
    });
  });

  it('refreshes data when refresh button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Performance Monitor')).toBeInTheDocument();
    });

    const refreshButton = screen.getByRole('button', { name: /refresh/i });
    await user.click(refreshButton);

    await waitFor(() => {
      expect(mockApi.getSystemHealth).toHaveBeenCalledTimes(2);
      expect(mockApi.getPerformanceMetrics).toHaveBeenCalledTimes(2);
      expect(mockApi.getBusinessMetrics).toHaveBeenCalledTimes(2);
      expect(mockApi.getAlerts).toHaveBeenCalledTimes(2);
      expect(mockApi.getDashboardData).toHaveBeenCalledTimes(2);
    });
  });

  it('handles loading state', () => {
    mockApi.getSystemHealth.mockImplementation(() => new Promise(() => {}));
    mockApi.getPerformanceMetrics.mockImplementation(() => new Promise(() => {}));
    mockApi.getBusinessMetrics.mockImplementation(() => new Promise(() => {}));
    mockApi.getAlerts.mockImplementation(() => new Promise(() => {}));
    mockApi.getDashboardData.mockImplementation(() => new Promise(() => {}));
    
    renderWithProviders(<PerformanceMonitor />);
    
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('handles error state for system health', async () => {
    mockApi.getSystemHealth.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading system health')).toBeInTheDocument();
    });
  });

  it('handles error state for performance metrics', async () => {
    mockApi.getPerformanceMetrics.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading performance metrics')).toBeInTheDocument();
    });
  });

  it('handles error state for business metrics', async () => {
    mockApi.getBusinessMetrics.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading business metrics')).toBeInTheDocument();
    });
  });

  it('handles error state for alerts', async () => {
    mockApi.getAlerts.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading alerts')).toBeInTheDocument();
    });
  });

  it('handles error state for logs', async () => {
    mockApi.getLogs.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading logs')).toBeInTheDocument();
    });
  });

  it('shows performance charts', async () => {
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('CPU Usage Over Time')).toBeInTheDocument();
      expect(screen.getByText('Memory Usage Over Time')).toBeInTheDocument();
      expect(screen.getByText('Response Time Over Time')).toBeInTheDocument();
    });
  });

  it('shows system status indicators', async () => {
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('System Status: Good')).toBeInTheDocument();
      expect(screen.getByText('Performance Score: 85')).toBeInTheDocument();
    });
  });

  it('displays real-time updates', async () => {
    renderWithProviders(<PerformanceMonitor />);
    
    await waitFor(() => {
      expect(screen.getByText('Real-time Updates')).toBeInTheDocument();
      expect(screen.getByText('Last Updated: 2024-01-01 00:00:00')).toBeInTheDocument();
    });
  });
});
