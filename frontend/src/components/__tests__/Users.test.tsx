import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import Users from '../Users';

// Mock the API service
const mockApi = {
  getUsers: jest.fn(),
  createUser: jest.fn(),
  updateUser: jest.fn(),
  deleteUser: jest.fn(),
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

const mockUsers = [
  {
    id: 1,
    username: 'admin',
    email: 'admin@example.com',
    role: 'Admin',
    isActive: true,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
  {
    id: 2,
    username: 'user1',
    email: 'user1@example.com',
    role: 'User',
    isActive: true,
    createdAt: '2024-01-02T00:00:00Z',
    updatedAt: '2024-01-02T00:00:00Z',
  },
];

describe('Users Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockApi.getUsers.mockResolvedValue(mockUsers);
  });

  it('renders users list', async () => {
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument();
      expect(screen.getByText('admin')).toBeInTheDocument();
      expect(screen.getByText('user1')).toBeInTheDocument();
    });
  });

  it('opens create user dialog when add button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    expect(screen.getByText('Create User')).toBeInTheDocument();
  });

  it('creates a new user when form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.createUser.mockResolvedValue({ id: 3, username: 'newuser' });
    
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const usernameInput = screen.getByLabelText(/username/i);
    const emailInput = screen.getByLabelText(/email/i);
    const passwordInput = screen.getByLabelText(/password/i);
    const submitButton = screen.getByRole('button', { name: /create/i });

    await user.type(usernameInput, 'newuser');
    await user.type(emailInput, 'newuser@example.com');
    await user.type(passwordInput, 'password123');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.createUser).toHaveBeenCalledWith(
        expect.objectContaining({
          username: 'newuser',
          email: 'newuser@example.com',
        })
      );
    });
  });

  it('opens edit dialog when edit button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    expect(screen.getByText('Edit User')).toBeInTheDocument();
    expect(screen.getByDisplayValue('admin')).toBeInTheDocument();
  });

  it('updates a user when edit form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.updateUser.mockResolvedValue({});
    
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    const usernameInput = screen.getByDisplayValue('admin');
    const submitButton = screen.getByRole('button', { name: /update/i });

    await user.clear(usernameInput);
    await user.type(usernameInput, 'updateduser');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.updateUser).toHaveBeenCalledWith(
        1,
        expect.objectContaining({
          username: 'updateduser',
        })
      );
    });
  });

  it('deletes a user when delete button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.deleteUser.mockResolvedValue({});
    
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
    });

    const deleteButton = screen.getByRole('button', { name: /delete/i });
    await user.click(deleteButton);

    // Confirm deletion
    const confirmButton = screen.getByRole('button', { name: /confirm/i });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(mockApi.deleteUser).toHaveBeenCalledWith(1);
    });
  });

  it('filters users by role', async () => {
    const user = userEvent.setup();
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
    });

    const roleFilter = screen.getByLabelText(/role/i);
    await user.click(roleFilter);
    
    const roleOption = screen.getByText('Admin');
    await user.click(roleOption);

    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
      expect(screen.queryByText('user1')).not.toBeInTheDocument();
    });
  });

  it('searches users by username', async () => {
    const user = userEvent.setup();
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText(/search/i);
    await user.type(searchInput, 'admin');

    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
      expect(screen.queryByText('user1')).not.toBeInTheDocument();
    });
  });

  it('handles loading state', () => {
    mockApi.getUsers.mockImplementation(() => new Promise(() => {}));
    
    renderWithProviders(<Users />);
    
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('handles error state', async () => {
    mockApi.getUsers.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading users')).toBeInTheDocument();
    });
  });

  it('validates required fields in create form', async () => {
    const user = userEvent.setup();
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const submitButton = screen.getByRole('button', { name: /create/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText('Username is required')).toBeInTheDocument();
      expect(screen.getByText('Email is required')).toBeInTheDocument();
      expect(screen.getByText('Password is required')).toBeInTheDocument();
    });
  });

  it('validates email format in create form', async () => {
    const user = userEvent.setup();
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const emailInput = screen.getByLabelText(/email/i);
    await user.type(emailInput, 'invalid-email');

    const submitButton = screen.getByRole('button', { name: /create/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText('Invalid email format')).toBeInTheDocument();
    });
  });

  it('shows user details in view mode', async () => {
    const user = userEvent.setup();
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
    });

    const viewButton = screen.getByRole('button', { name: /view/i });
    await user.click(viewButton);

    expect(screen.getByText('User Details')).toBeInTheDocument();
    expect(screen.getByText('admin@example.com')).toBeInTheDocument();
    expect(screen.getByText('Admin')).toBeInTheDocument();
  });

  it('toggles user active status', async () => {
    const user = userEvent.setup();
    mockApi.updateUser.mockResolvedValue({});
    
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
    });

    const toggleButton = screen.getByRole('button', { name: /toggle/i });
    await user.click(toggleButton);

    await waitFor(() => {
      expect(mockApi.updateUser).toHaveBeenCalledWith(
        1,
        expect.objectContaining({
          isActive: false,
        })
      );
    });
  });

  it('handles bulk operations', async () => {
    const user = userEvent.setup();
    mockApi.deleteUser.mockResolvedValue({});
    
    renderWithProviders(<Users />);
    
    await waitFor(() => {
      expect(screen.getByText('admin')).toBeInTheDocument();
    });

    // Select multiple users
    const checkboxes = screen.getAllByRole('checkbox');
    await user.click(checkboxes[1]); // Select first user
    await user.click(checkboxes[2]); // Select second user

    const bulkDeleteButton = screen.getByRole('button', { name: /bulk delete/i });
    await user.click(bulkDeleteButton);

    const confirmButton = screen.getByRole('button', { name: /confirm/i });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(mockApi.deleteUser).toHaveBeenCalledTimes(2);
    });
  });
});
