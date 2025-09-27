import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import AutomationJobs from '../AutomationJobs';

// Mock the API service
const mockApi = {
  getAutomationJobs: jest.fn(),
  createAutomationJob: jest.fn(),
  updateAutomationJob: jest.fn(),
  deleteAutomationJob: jest.fn(),
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

const mockJobs = [
  {
    id: 1,
    name: 'Test Job 1',
    description: 'Test Description 1',
    statusId: 1,
    jobTypeId: 1,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
  {
    id: 2,
    name: 'Test Job 2',
    description: 'Test Description 2',
    statusId: 2,
    jobTypeId: 2,
    createdAt: '2024-01-02T00:00:00Z',
    updatedAt: '2024-01-02T00:00:00Z',
  },
];

describe('AutomationJobs Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockApi.getAutomationJobs.mockResolvedValue(mockJobs);
  });

  it('renders automation jobs list', async () => {
    renderWithProviders(<AutomationJobs />);
    
    await waitFor(() => {
      expect(screen.getByText('Automation Jobs')).toBeInTheDocument();
      expect(screen.getByText('Test Job 1')).toBeInTheDocument();
      expect(screen.getByText('Test Job 2')).toBeInTheDocument();
    });
  });

  it('opens create job dialog when add button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<AutomationJobs />);
    
    await waitFor(() => {
      expect(screen.getByText('Automation Jobs')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    expect(screen.getByText('Create Automation Job')).toBeInTheDocument();
  });

  it('creates a new job when form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.createAutomationJob.mockResolvedValue({ id: 3, name: 'New Job' });
    
    renderWithProviders(<AutomationJobs />);
    
    await waitFor(() => {
      expect(screen.getByText('Automation Jobs')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const nameInput = screen.getByLabelText(/name/i);
    const descriptionInput = screen.getByLabelText(/description/i);
    const submitButton = screen.getByRole('button', { name: /create/i });

    await user.type(nameInput, 'New Job');
    await user.type(descriptionInput, 'New Description');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.createAutomationJob).toHaveBeenCalledWith(
        expect.objectContaining({
          name: 'New Job',
          description: 'New Description',
        })
      );
    });
  });

  it('opens edit dialog when edit button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<AutomationJobs />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Job 1')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    expect(screen.getByText('Edit Automation Job')).toBeInTheDocument();
    expect(screen.getByDisplayValue('Test Job 1')).toBeInTheDocument();
  });

  it('updates a job when edit form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.updateAutomationJob.mockResolvedValue({});
    
    renderWithProviders(<AutomationJobs />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Job 1')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    const nameInput = screen.getByDisplayValue('Test Job 1');
    const submitButton = screen.getByRole('button', { name: /update/i });

    await user.clear(nameInput);
    await user.type(nameInput, 'Updated Job');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.updateAutomationJob).toHaveBeenCalledWith(
        1,
        expect.objectContaining({
          name: 'Updated Job',
        })
      );
    });
  });

  it('deletes a job when delete button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.deleteAutomationJob.mockResolvedValue({});
    
    renderWithProviders(<AutomationJobs />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Job 1')).toBeInTheDocument();
    });

    const deleteButton = screen.getByRole('button', { name: /delete/i });
    await user.click(deleteButton);

    // Confirm deletion
    const confirmButton = screen.getByRole('button', { name: /confirm/i });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(mockApi.deleteAutomationJob).toHaveBeenCalledWith(1);
    });
  });

  it('filters jobs by status', async () => {
    const user = userEvent.setup();
    renderWithProviders(<AutomationJobs />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Job 1')).toBeInTheDocument();
    });

    const statusFilter = screen.getByLabelText(/status/i);
    await user.click(statusFilter);
    
    const statusOption = screen.getByText('Active');
    await user.click(statusOption);

    await waitFor(() => {
      expect(mockApi.getAutomationJobs).toHaveBeenCalledWith(
        expect.objectContaining({
          statusId: 1,
        })
      );
    });
  });

  it('searches jobs by name', async () => {
    const user = userEvent.setup();
    renderWithProviders(<AutomationJobs />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Job 1')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText(/search/i);
    await user.type(searchInput, 'Test Job 1');

    await waitFor(() => {
      expect(screen.getByText('Test Job 1')).toBeInTheDocument();
      expect(screen.queryByText('Test Job 2')).not.toBeInTheDocument();
    });
  });

  it('handles loading state', () => {
    mockApi.getAutomationJobs.mockImplementation(() => new Promise(() => {}));
    
    renderWithProviders(<AutomationJobs />);
    
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('handles error state', async () => {
    mockApi.getAutomationJobs.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<AutomationJobs />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading automation jobs')).toBeInTheDocument();
    });
  });
});
