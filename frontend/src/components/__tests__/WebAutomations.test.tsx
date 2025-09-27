import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import WebAutomations from '../WebAutomations';

// Mock the API service
const mockApi = {
  getWebAutomations: jest.fn(),
  createWebAutomation: jest.fn(),
  updateWebAutomation: jest.fn(),
  deleteWebAutomation: jest.fn(),
  analyzeWebPage: jest.fn(),
  identifyWebElement: jest.fn(),
  generateWebSelector: jest.fn(),
  validateWebAction: jest.fn(),
  extractDataFromWeb: jest.fn(),
  generateAutomationScript: jest.fn(),
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

const mockWebAutomations = [
  {
    id: 1,
    name: 'Login Automation',
    websiteUrl: 'https://example.com',
    automationType: 'Login',
    status: 'Completed',
    executionTime: 5000,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
  {
    id: 2,
    name: 'Data Entry Automation',
    websiteUrl: 'https://test.com',
    automationType: 'DataEntry',
    status: 'Failed',
    executionTime: 8000,
    createdAt: '2024-01-02T00:00:00Z',
    updatedAt: '2024-01-02T00:00:00Z',
  },
];

describe('WebAutomations Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockApi.getWebAutomations.mockResolvedValue(mockWebAutomations);
  });

  it('renders web automations list', async () => {
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
      expect(screen.getByText('Data Entry Automation')).toBeInTheDocument();
    });
  });

  it('opens create web automation dialog when add button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    expect(screen.getByText('Create Web Automation')).toBeInTheDocument();
  });

  it('creates a new web automation when form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.createWebAutomation.mockResolvedValue({ id: 3, name: 'New Automation' });
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const nameInput = screen.getByLabelText(/name/i);
    const websiteInput = screen.getByLabelText(/website/i);
    const typeInput = screen.getByLabelText(/type/i);
    const submitButton = screen.getByRole('button', { name: /create/i });

    await user.type(nameInput, 'New Automation');
    await user.type(websiteInput, 'https://new.com');
    await user.selectOptions(typeInput, 'Login');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.createWebAutomation).toHaveBeenCalledWith(
        expect.objectContaining({
          name: 'New Automation',
          websiteUrl: 'https://new.com',
          automationType: 'Login',
        })
      );
    });
  });

  it('opens edit dialog when edit button is clicked', async () => {
    const user = userEvent.setup();
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    expect(screen.getByText('Edit Web Automation')).toBeInTheDocument();
    expect(screen.getByDisplayValue('Login Automation')).toBeInTheDocument();
  });

  it('updates a web automation when edit form is submitted', async () => {
    const user = userEvent.setup();
    mockApi.updateWebAutomation.mockResolvedValue({});
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
    });

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    const nameInput = screen.getByDisplayValue('Login Automation');
    const submitButton = screen.getByRole('button', { name: /update/i });

    await user.clear(nameInput);
    await user.type(nameInput, 'Updated Automation');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.updateWebAutomation).toHaveBeenCalledWith(
        1,
        expect.objectContaining({
          name: 'Updated Automation',
        })
      );
    });
  });

  it('deletes a web automation when delete button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.deleteWebAutomation.mockResolvedValue({});
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
    });

    const deleteButton = screen.getByRole('button', { name: /delete/i });
    await user.click(deleteButton);

    // Confirm deletion
    const confirmButton = screen.getByRole('button', { name: /confirm/i });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(mockApi.deleteWebAutomation).toHaveBeenCalledWith(1);
    });
  });

  it('analyzes web page when analyze button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.analyzeWebPage.mockResolvedValue('Web page analysis result');
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const analyzeButton = screen.getByRole('button', { name: /analyze/i });
    await user.click(analyzeButton);

    const urlInput = screen.getByLabelText(/url/i);
    const promptInput = screen.getByLabelText(/prompt/i);
    const submitButton = screen.getByRole('button', { name: /analyze/i });

    await user.type(urlInput, 'https://example.com');
    await user.type(promptInput, 'Analyze login form');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.analyzeWebPage).toHaveBeenCalledWith(
        expect.objectContaining({
          url: 'https://example.com',
          prompt: 'Analyze login form',
        })
      );
    });
  });

  it('identifies web element when identify button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.identifyWebElement.mockResolvedValue('Element identification result');
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const identifyButton = screen.getByRole('button', { name: /identify/i });
    await user.click(identifyButton);

    const pageContentInput = screen.getByLabelText(/page content/i);
    const descriptionInput = screen.getByLabelText(/description/i);
    const submitButton = screen.getByRole('button', { name: /identify/i });

    await user.type(pageContentInput, '<html><body><button>Login</button></body></html>');
    await user.type(descriptionInput, 'Login button');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.identifyWebElement).toHaveBeenCalledWith(
        expect.objectContaining({
          pageContent: '<html><body><button>Login</button></body></html>',
          description: 'Login button',
        })
      );
    });
  });

  it('generates web selector when generate button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.generateWebSelector.mockResolvedValue('#login-button');
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const generateButton = screen.getByRole('button', { name: /generate selector/i });
    await user.click(generateButton);

    const elementDescriptionInput = screen.getByLabelText(/element description/i);
    const pageContentInput = screen.getByLabelText(/page content/i);
    const submitButton = screen.getByRole('button', { name: /generate/i });

    await user.type(elementDescriptionInput, 'Login button with text Login');
    await user.type(pageContentInput, '<html><body><button>Login</button></body></html>');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.generateWebSelector).toHaveBeenCalledWith(
        expect.objectContaining({
          elementDescription: 'Login button with text Login',
          pageContent: '<html><body><button>Login</button></body></html>',
        })
      );
    });
  });

  it('validates web action when validate button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.validateWebAction.mockResolvedValue('Action validation result');
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const validateButton = screen.getByRole('button', { name: /validate/i });
    await user.click(validateButton);

    const actionInput = screen.getByLabelText(/action/i);
    const elementInput = screen.getByLabelText(/element/i);
    const pageContentInput = screen.getByLabelText(/page content/i);
    const submitButton = screen.getByRole('button', { name: /validate/i });

    await user.type(actionInput, 'click');
    await user.type(elementInput, 'button#login');
    await user.type(pageContentInput, '<html><body><button id="login">Login</button></body></html>');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.validateWebAction).toHaveBeenCalledWith(
        expect.objectContaining({
          action: 'click',
          element: 'button#login',
          pageContent: '<html><body><button id="login">Login</button></body></html>',
        })
      );
    });
  });

  it('extracts data from web when extract button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.extractDataFromWeb.mockResolvedValue('Extracted data result');
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const extractButton = screen.getByRole('button', { name: /extract/i });
    await user.click(extractButton);

    const pageContentInput = screen.getByLabelText(/page content/i);
    const extractionPromptInput = screen.getByLabelText(/extraction prompt/i);
    const submitButton = screen.getByRole('button', { name: /extract/i });

    await user.type(pageContentInput, '<html><body><div class="price">$99.99</div></body></html>');
    await user.type(extractionPromptInput, 'Extract the price');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.extractDataFromWeb).toHaveBeenCalledWith(
        expect.objectContaining({
          pageContent: '<html><body><div class="price">$99.99</div></body></html>',
          extractionPrompt: 'Extract the price',
        })
      );
    });
  });

  it('generates automation script when generate script button is clicked', async () => {
    const user = userEvent.setup();
    mockApi.generateAutomationScript.mockResolvedValue('Generated automation script');
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const generateScriptButton = screen.getByRole('button', { name: /generate script/i });
    await user.click(generateScriptButton);

    const requirementsInput = screen.getByLabelText(/requirements/i);
    const targetWebsiteInput = screen.getByLabelText(/target website/i);
    const submitButton = screen.getByRole('button', { name: /generate script/i });

    await user.type(requirementsInput, 'Automate login process');
    await user.type(targetWebsiteInput, 'https://example.com');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockApi.generateAutomationScript).toHaveBeenCalledWith(
        expect.objectContaining({
          requirements: 'Automate login process',
          targetWebsite: 'https://example.com',
        })
      );
    });
  });

  it('filters web automations by status', async () => {
    const user = userEvent.setup();
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
    });

    const statusFilter = screen.getByLabelText(/status/i);
    await user.click(statusFilter);
    
    const statusOption = screen.getByText('Completed');
    await user.click(statusOption);

    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
      expect(screen.queryByText('Data Entry Automation')).not.toBeInTheDocument();
    });
  });

  it('filters web automations by type', async () => {
    const user = userEvent.setup();
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
    });

    const typeFilter = screen.getByLabelText(/type/i);
    await user.click(typeFilter);
    
    const typeOption = screen.getByText('Login');
    await user.click(typeOption);

    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
      expect(screen.queryByText('Data Entry Automation')).not.toBeInTheDocument();
    });
  });

  it('searches web automations by name', async () => {
    const user = userEvent.setup();
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText(/search/i);
    await user.type(searchInput, 'Login');

    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
      expect(screen.queryByText('Data Entry Automation')).not.toBeInTheDocument();
    });
  });

  it('shows web automation details in view mode', async () => {
    const user = userEvent.setup();
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
    });

    const viewButton = screen.getByRole('button', { name: /view/i });
    await user.click(viewButton);

    expect(screen.getByText('Web Automation Details')).toBeInTheDocument();
    expect(screen.getByText('Login Automation')).toBeInTheDocument();
    expect(screen.getByText('https://example.com')).toBeInTheDocument();
    expect(screen.getByText('Login')).toBeInTheDocument();
    expect(screen.getByText('Completed')).toBeInTheDocument();
  });

  it('handles loading state', () => {
    mockApi.getWebAutomations.mockImplementation(() => new Promise(() => {}));
    
    renderWithProviders(<WebAutomations />);
    
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('handles error state', async () => {
    mockApi.getWebAutomations.mockRejectedValue(new Error('API Error'));
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Error loading web automations')).toBeInTheDocument();
    });
  });

  it('validates required fields in create form', async () => {
    const user = userEvent.setup();
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const submitButton = screen.getByRole('button', { name: /create/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText('Name is required')).toBeInTheDocument();
      expect(screen.getByText('Website URL is required')).toBeInTheDocument();
      expect(screen.getByText('Automation type is required')).toBeInTheDocument();
    });
  });

  it('validates website URL format', async () => {
    const user = userEvent.setup();
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Web Automations')).toBeInTheDocument();
    });

    const addButton = screen.getByRole('button', { name: /add/i });
    await user.click(addButton);

    const websiteInput = screen.getByLabelText(/website/i);
    await user.type(websiteInput, 'invalid-url');

    const submitButton = screen.getByRole('button', { name: /create/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText('Invalid website URL format')).toBeInTheDocument();
    });
  });

  it('shows execution time in human readable format', async () => {
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('5.0s')).toBeInTheDocument();
      expect(screen.getByText('8.0s')).toBeInTheDocument();
    });
  });

  it('handles bulk operations', async () => {
    const user = userEvent.setup();
    mockApi.deleteWebAutomation.mockResolvedValue({});
    
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Login Automation')).toBeInTheDocument();
    });

    // Select multiple web automations
    const checkboxes = screen.getAllByRole('checkbox');
    await user.click(checkboxes[1]); // Select first automation
    await user.click(checkboxes[2]); // Select second automation

    const bulkDeleteButton = screen.getByRole('button', { name: /bulk delete/i });
    await user.click(bulkDeleteButton);

    const confirmButton = screen.getByRole('button', { name: /confirm/i });
    await user.click(confirmButton);

    await waitFor(() => {
      expect(mockApi.deleteWebAutomation).toHaveBeenCalledTimes(2);
    });
  });

  it('shows web automation statistics', async () => {
    renderWithProviders(<WebAutomations />);
    
    await waitFor(() => {
      expect(screen.getByText('Total Automations: 2')).toBeInTheDocument();
      expect(screen.getByText('Completed: 1')).toBeInTheDocument();
      expect(screen.getByText('Failed: 1')).toBeInTheDocument();
      expect(screen.getByText('Success Rate: 50%')).toBeInTheDocument();
    });
  });
});
