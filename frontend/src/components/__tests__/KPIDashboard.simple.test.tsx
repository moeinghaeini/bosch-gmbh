import React from 'react';
import { render, screen } from '@testing-library/react';
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

const renderWithProviders = (component: React.ReactElement) => {
  const queryClient = createTestQueryClient();
  const theme = createTheme();
  
  return render(
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <ThemeProvider theme={theme}>
          {component}
        </ThemeProvider>
      </BrowserRouter>
    </QueryClientProvider>
  );
};

describe('KPIDashboard Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('renders KPI dashboard with basic content', () => {
    mockApi.getTestExecutionKPIs.mockResolvedValue({
      testExecution: {
        totalTests: 100,
        passedTests: 85,
        failedTests: 15,
        successRate: 85.0,
        averageExecutionTime: 2.5
      }
    });

    renderWithProviders(<KPIDashboard />);
    
    expect(screen.getByText('📊 Performance KPIs')).toBeInTheDocument();
  });

  it('handles loading state', () => {
    mockApi.getTestExecutionKPIs.mockImplementation(() => new Promise(() => {}));
    
    renderWithProviders(<KPIDashboard />);
    
    // Component should render without crashing
    expect(screen.getByText('📊 Performance KPIs')).toBeInTheDocument();
  });

  it('handles error state gracefully', () => {
    mockApi.getTestExecutionKPIs.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<KPIDashboard />);
    
    // Component should render without crashing
    expect(screen.getByText('📊 Performance KPIs')).toBeInTheDocument();
  });
});
