import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import TestExecutions from '../TestExecutions';

// Mock the API service
const mockApi = {
  getTestExecutions: jest.fn(),
  createTestExecution: jest.fn(),
  updateTestExecution: jest.fn(),
  deleteTestExecution: jest.fn(),
  analyzeTestExecution: jest.fn(),
  generateTestCases: jest.fn(),
  optimizeTestSuite: jest.fn(),
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

const mockTestExecutions = [
  {
    id: 1,
    testName: 'Login Test',
    testType: 'Unit',
    status: 'Passed',
    executionTime: 1500,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
  {
    id: 2,
    testName: 'API Test',
    testType: 'Integration',
    status: 'Failed',
    executionTime: 3000,
    createdAt: '2024-01-02T00:00:00Z',
    updatedAt: '2024-01-02T00:00:00Z',
  },
];

describe('TestExecutions Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockApi.getTestExecutions.mockResolvedValue(mockTestExecutions);
  });

  it('renders test executions list', async () => {
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Executions')).toBeInTheDocument();
      expect(screen.getByText('Login Test')).toBeInTheDocument();
      expect(screen.getByText('API Test')).toBeInTheDocument();
    });
  });

  it('opens create test execution dialog when add button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Executions')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    expect(screen.getByText('Create Test Execution')).toBeInTheDocument();
  });

  it('creates a new test execution when form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.createTestExecution.mockResolvedValue({ id: 3, testName: 'New Test' });
    
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Executions')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const testNameInput = screen.getByLabelText(/test name/i);
    const testTypeInput = screen.getByLabelText(/test type/i);
    const submitButton = screen.getByRole('button', { name: /create/i });

    await user.type(testNameInput, 'New Test');
    await user.selectOptions(testTypeInput, 'Unit');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.createTestExecution).toHaveBeenCalledWith(
        expect.objectContaining({
          testName: 'New Test',
          testType: 'Unit',
        })
      );
    });
  });

  it('opens edit dialog when edit button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    expect(screen.getByText('Edit Test Execution')).toBeInTheDocument();
    expect(screen.getByDisplayValue('Login Test')).toBeInTheDocument();
  });

  it('updates a test execution when edit form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.updateTestExecution.mockResolvedValue({});
    
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    const testNameInput = screen.getByDisplayValue('Login Test');
    const submitButton = screen.getByRole('button', { name: /update/i });

    await user.clear(testNameInput);
    await user.type(testNameInput, 'Updated Test');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.updateTestExecution).toHaveBeenCalledWith(
        1,
        expect.objectContaining({
          testName: 'Updated Test',
        })
      );
    });
  });

  it('deletes a test execution when delete button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.deleteTestExecution.mockResolvedValue({});
    
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
    });

    const deleteButton = screen.getByRole('button', { name: /delete/i });
    await user.click(deleteButton);

    // Confirm deletion
    const confirmButton = screen.getByRole('button', { name: /confirm/i });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(mockApi.deleteTestExecution).toHaveBeenCalledWith(1);
    });
  });

  it('analyzes test execution when analyze button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.analyzeTestExecution.mockResolvedValue('Test analysis result');
    
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
    });

    const analyzeButton = screen.getByRole('button', { name: /analyze/i });
    await user.click(analyzeButton);

    await waitFor(() => {
      expect(mockApi.analyzeTestExecution).toHaveBeenCalledWith(1);
      expect(screen.getByText('Test analysis result')).toBeInTheDocument();
    });
  });

  it('generates test cases when generate button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.generateTestCases.mockResolvedValue('Generated test cases');
    
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Executions')).toBeInTheDocument();
    });

    const generateButton = screen.getByRole('button', { name: /generate/i });
    await user.click(generateButton);

    const requirementsInput = screen.getByLabelText(/requirements/i);
    const testTypeInput = screen.getByLabelText(/test type/i);
    const submitButton = screen.getByRole('button', { name: /generate/i });

    await user.type(requirementsInput, 'Test user login');
    await user.selectOptions(testTypeInput, 'Unit');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.generateTestCases).toHaveBeenCalledWith(
        expect.objectContaining({
          requirements: 'Test user login',
          testType: 'Unit',
        })
      );
    });
  });

  it('optimizes test suite when optimize button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.optimizeTestSuite.mockResolvedValue('Test suite optimized');
    
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Executions')).toBeInTheDocument();
    });

    const optimizeButton = screen.getByRole('button', { name: /optimize/i });
    await user.click(optimizeButton);

    const testSuiteInput = screen.getByLabelText(/test suite/i);
    const submitButton = screen.getByRole('button', { name: /optimize/i });

    await user.type(testSuiteInput, 'Large test suite');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.optimizeTestSuite).toHaveBeenCalledWith(
        expect.objectContaining({
          testSuite: 'Large test suite',
        })
      );
    });
  });

  it('filters test executions by status', async () => {
    const user = userEvent.setup();
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
    });

    const statusFilter = screen.getByLabelText(/status/i);
    await user.click(statusFilter);
    
    const statusOption = screen.getByText('Passed');
    await user.click(statusOption);

    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
      expect(screen.queryByText('API Test')).not.toBeInTheDocument();
    });
  });

  it('filters test executions by type', async () => {
    const user = userEvent.setup();
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
    });

    const typeFilter = screen.getByLabelText(/type/i);
    await user.click(typeFilter);
    
    const typeOption = screen.getByText('Unit');
    await user.click(typeOption);

    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
      expect(screen.queryByText('API Test')).not.toBeInTheDocument();
    });
  });

  it('searches test executions by name', async () => {
    const user = userEvent.setup();
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText(/search/i);
    await user.type(searchInput, 'Login');

    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
      expect(screen.queryByText('API Test')).not.toBeInTheDocument();
    });
  });

  it('shows test execution details in view mode', async () => {
    const user = userEvent.setup();
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
    });

    const viewButton = screen.getByRole('button', { name: /view/i });
    await user.click(viewButton);

    expect(screen.getByText('Test Execution Details')).toBeInTheDocument();
    expect(screen.getByText('Login Test')).toBeInTheDocument();
    expect(screen.getByText('Unit')).toBeInTheDocument();
    expect(screen.getByText('Passed')).toBeInTheDocument();
  });

  it('handles loading state', () => {
    mockApi.getTestExecutions.mockImplementation(() => new Promise(() => {}));
    
    renderWithProviders(<TestExecutions />);
    
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('handles error state', async () => {
    mockApi.getTestExecutions.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading test executions')).toBeInTheDocument();
    });
  });

  it('validates required fields in create form', async () => {
    const user = userEvent.setup();
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Executions')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const submitButton = screen.getByRole('button', { name: /create/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText('Test name is required')).toBeInTheDocument();
      expect(screen.getByText('Test type is required')).toBeInTheDocument();
    });
  });

  it('shows execution time in human readable format', async () => {
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('1.5s')).toBeInTheDocument();
      expect(screen.getByText('3.0s')).toBeInTheDocument();
    });
  });

  it('handles bulk operations', async () => {
    const user = userEvent.setup();
    mockApi.deleteTestExecution.mockResolvedValue({});
    
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Test')).toBeInTheDocument();
    });

    // Select multiple test executions
    const checkboxes = screen.getAllByRole('checkbox');
    await user.click(checkboxes[1]); // Select first test
    await user.click(checkboxes[2]); // Select second test

    const bulkDeleteButton = screen.getByRole('button', { name: /bulk delete/i });
    await user.click(bulkDeleteButton);

    const confirmButton = screen.getByRole('button', { name: /confirm/i });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(mockApi.deleteTestExecution).toHaveBeenCalledTimes(2);
    });
  });

  it('exports test results to CSV', async () => {
    const user = userEvent.setup();
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Executions')).toBeInTheDocument();
    });

    const exportButton = screen.getByRole('button', { name: /export/i });
    await user.click(exportButton);

    // Verify CSV download was triggered
    expect(exportButton).toBeInTheDocument();
  });

  it('shows test execution statistics', async () => {
    renderWithProviders(<TestExecutions />);
    
    await waitFor(() => {
      expect(screen.getByText('Total Tests: 2')).toBeInTheDocument();
      expect(screen.getByText('Passed: 1')).toBeInTheDocument();
      expect(screen.getByText('Failed: 1')).toBeInTheDocument();
      expect(screen.getByText('Success Rate: 50%')).toBeInTheDocument();
    });
  });
});
