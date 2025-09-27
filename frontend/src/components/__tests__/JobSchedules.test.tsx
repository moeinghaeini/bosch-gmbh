import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import JobSchedules from '../JobSchedules';

// Mock the API service
const mockApi = {
  getJobSchedules: jest.fn(),
  createJobSchedule: jest.fn(),
  updateJobSchedule: jest.fn(),
  deleteJobSchedule: jest.fn(),
  enableJobSchedule: jest.fn(),
  disableJobSchedule: jest.fn(),
  runJobNow: jest.fn(),
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

const mockJobSchedules = [
  {
    id: 1,
    name: 'Daily Backup',
    jobType: 'Backup',
    status: 'Scheduled',
    priority: 'High',
    isEnabled: true,
    nextRunTime: '2024-01-01T02:00:00Z',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
  {
    id: 2,
    name: 'Weekly Report',
    jobType: 'Report',
    status: 'Scheduled',
    priority: 'Medium',
    isEnabled: false,
    nextRunTime: '2024-01-07T09:00:00Z',
    createdAt: '2024-01-02T00:00:00Z',
    updatedAt: '2024-01-02T00:00:00Z',
  },
];

describe('JobSchedules Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockApi.getJobSchedules.mockResolvedValue(mockJobSchedules);
  });

  it('renders job schedules list', async () => {
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Job Schedules')).toBeInTheDocument();
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
      expect(screen.getByText('Weekly Report')).toBeInTheDocument();
    });
  });

  it('opens create job schedule dialog when add button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Job Schedules')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    expect(screen.getByText('Create Job Schedule')).toBeInTheDocument();
  });

  it('creates a new job schedule when form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.createJobSchedule.mockResolvedValue({ id: 3, name: 'New Job' });
    
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Job Schedules')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const nameInput = screen.getByLabelText(/name/i);
    const jobTypeInput = screen.getByLabelText(/job type/i);
    const priorityInput = screen.getByLabelText(/priority/i);
    const submitButton = screen.getByRole('button', { name: /create/i });

    await user.type(nameInput, 'New Job');
    await user.selectOptions(jobTypeInput, 'Backup');
    await user.selectOptions(priorityInput, 'High');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.createJobSchedule).toHaveBeenCalledWith(
        expect.objectContaining({
          name: 'New Job',
          jobType: 'Backup',
          priority: 'High',
        })
      );
    });
  });

  it('opens edit dialog when edit button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    expect(screen.getByText('Edit Job Schedule')).toBeInTheDocument();
    expect(screen.getByDisplayValue('Daily Backup')).toBeInTheDocument();
  });

  it('updates a job schedule when edit form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.updateJobSchedule.mockResolvedValue({});
    
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    const nameInput = screen.getByDisplayValue('Daily Backup');
    const submitButton = screen.getByRole('button', { name: /update/i });

    await user.clear(nameInput);
    await user.type(nameInput, 'Updated Job');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.updateJobSchedule).toHaveBeenCalledWith(
        1,
        expect.objectContaining({
          name: 'Updated Job',
        })
      );
    });
  });

  it('deletes a job schedule when delete button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.deleteJobSchedule.mockResolvedValue({});
    
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const deleteButton = screen.getByRole('button', { name: /delete/i });
    await user.click(deleteButton);

    // Confirm deletion
    const confirmButton = screen.getByRole('button', { name: /confirm/i });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(mockApi.deleteJobSchedule).toHaveBeenCalledWith(1);
    });
  });

  it('enables a job schedule when enable button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.enableJobSchedule.mockResolvedValue({});
    
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Weekly Report')).toBeInTheDocument();
    });

    const enableButton = screen.getByRole('button', { name: /enable/i });
    await user.click(enableButton);

    await waitFor(() => {
      expect(mockApi.enableJobSchedule).toHaveBeenCalledWith(2);
    });
  });

  it('disables a job schedule when disable button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.disableJobSchedule.mockResolvedValue({});
    
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const disableButton = screen.getByRole('button', { name: /disable/i });
    await user.click(disableButton);

    await waitFor(() => {
      expect(mockApi.disableJobSchedule).toHaveBeenCalledWith(1);
    });
  });

  it('runs a job immediately when run now button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.runJobNow.mockResolvedValue({});
    
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const runNowButton = screen.getByRole('button', { name: /run now/i });
    await user.click(runNowButton);

    await waitFor(() => {
      expect(mockApi.runJobNow).toHaveBeenCalledWith(1);
    });
  });

  it('filters job schedules by status', async () => {
    const user = userEvent.setup();
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const statusFilter = screen.getByLabelText(/status/i);
    await user.click(statusFilter);
    
    const statusOption = screen.getByText('Scheduled');
    await user.click(statusOption);

    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
      expect(screen.getByText('Weekly Report')).toBeInTheDocument();
    });
  });

  it('filters job schedules by type', async () => {
    const user = userEvent.setup();
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const typeFilter = screen.getByLabelText(/type/i);
    await user.click(typeFilter);
    
    const typeOption = screen.getByText('Backup');
    await user.click(typeOption);

    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
      expect(screen.queryByText('Weekly Report')).not.toBeInTheDocument();
    });
  });

  it('filters job schedules by priority', async () => {
    const user = userEvent.setup();
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const priorityFilter = screen.getByLabelText(/priority/i);
    await user.click(priorityFilter);
    
    const priorityOption = screen.getByText('High');
    await user.click(priorityOption);

    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
      expect(screen.queryByText('Weekly Report')).not.toBeInTheDocument();
    });
  });

  it('searches job schedules by name', async () => {
    const user = userEvent.setup();
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText(/search/i);
    await user.type(searchInput, 'Daily');

    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
      expect(screen.queryByText('Weekly Report')).not.toBeInTheDocument();
    });
  });

  it('shows job schedule details in view mode', async () => {
    const user = userEvent.setup();
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const viewButton = screen.getByRole('button', { name: /view/i });
    await user.click(viewButton);

    expect(screen.getByText('Job Schedule Details')).toBeInTheDocument();
    expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    expect(screen.getByText('Backup')).toBeInTheDocument();
    expect(screen.getByText('Scheduled')).toBeInTheDocument();
    expect(screen.getByText('High')).toBeInTheDocument();
  });

  it('handles loading state', () => {
    mockApi.getJobSchedules.mockImplementation(() => new Promise(() => {}));
    
    renderWithProviders(<JobSchedules />);
    
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('handles error state', async () => {
    mockApi.getJobSchedules.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading job schedules')).toBeInTheDocument();
    });
  });

  it('validates required fields in create form', async () => {
    const user = userEvent.setup();
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Job Schedules')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const submitButton = screen.getByRole('button', { name: /create/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText('Name is required')).toBeInTheDocument();
      expect(screen.getByText('Job type is required')).toBeInTheDocument();
      expect(screen.getByText('Priority is required')).toBeInTheDocument();
    });
  });

  it('shows next run time in human readable format', async () => {
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Jan 1, 2024 2:00 AM')).toBeInTheDocument();
      expect(screen.getByText('Jan 7, 2024 9:00 AM')).toBeInTheDocument();
    });
  });

  it('shows enabled/disabled status', async () => {
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Enabled')).toBeInTheDocument();
      expect(screen.getByText('Disabled')).toBeInTheDocument();
    });
  });

  it('handles bulk operations', async () => {
    const user = userEvent.setup();
    mockApi.deleteJobSchedule.mockResolvedValue({});
    
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    // Select multiple job schedules
    const checkboxes = screen.getAllByRole('checkbox');
    await user.click(checkboxes[1]); // Select first job
    await user.click(checkboxes[2]); // Select second job

    const bulkDeleteButton = screen.getByRole('button', { name: /bulk delete/i });
    await user.click(bulkDeleteButton);

    const confirmButton = screen.getByRole('button', { name: /confirm/i });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(mockApi.deleteJobSchedule).toHaveBeenCalledTimes(2);
    });
  });

  it('shows job schedule statistics', async () => {
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Total Jobs: 2')).toBeInTheDocument();
      expect(screen.getByText('Enabled: 1')).toBeInTheDocument();
      expect(screen.getByText('Disabled: 1')).toBeInTheDocument();
      expect(screen.getByText('Scheduled: 2')).toBeInTheDocument();
    });
  });

  it('shows jobs ready to run', async () => {
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Jobs Ready to Run')).toBeInTheDocument();
    });
  });

  it('shows dependent jobs', async () => {
    const user = userEvent.setup();
    renderWithProviders(<JobSchedules />);
    
    await waitFor(() => {
      expect(screen.getByText('Daily Backup')).toBeInTheDocument();
    });

    const dependenciesButton = screen.getByRole('button', { name: /dependencies/i });
    await user.click(dependenciesButton);

    expect(screen.getByText('Dependent Jobs')).toBeInTheDocument();
  });
});
