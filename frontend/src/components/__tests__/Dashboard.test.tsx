import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import Dashboard from '../Dashboard';

// Mock the API service
jest.mock('../../services/api', () => ({
  getAutomationJobs: jest.fn(),
  getTestExecutions: jest.fn(),
  getWebAutomations: jest.fn(),
  getUsers: jest.fn(),
}));

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

describe('Dashboard Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('renders dashboard title', () => {
    renderWithProviders(<Dashboard />);
    
    expect(screen.getByText('Industrial Automation Dashboard')).toBeInTheDocument();
  });

  it('renders statistics cards', async () => {
    renderWithProviders(<Dashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Total Jobs')).toBeInTheDocument();
      expect(screen.getByText('Active Jobs')).toBeInTheDocument();
      expect(screen.getByText('Completed Jobs')).toBeInTheDocument();
      expect(screen.getByText('Failed Jobs')).toBeInTheDocument();
    });
  });

  it('renders recent jobs section', async () => {
    renderWithProviders(<Dashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Recent Jobs')).toBeInTheDocument();
    });
  });

  it('renders system status section', async () => {
    renderWithProviders(<Dashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('System Status')).toBeInTheDocument();
    });
  });

  it('displays loading state initially', () => {
    renderWithProviders(<Dashboard />);
    
    // Check for loading indicators
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('handles error state gracefully', async () => {
    // Mock API to return error
    const { getAutomationJobs } = require('../../services/api');
    getAutomationJobs.mockRejectedValueOnce(new Error('API Error'));

    renderWithProviders(<Dashboard />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading data')).toBeInTheDocument();
    });
  });
});
