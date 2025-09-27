import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from 'react-query';
import {
  Box,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Alert,
  CircularProgress,
  Chip,
  Grid,
  Card,
  CardContent
} from '@mui/material';
import { DataGrid, GridColDef, GridActionsCellItem } from '@mui/x-data-grid';
import { Add, Edit, Delete, Psychology, PlayArrow, Web } from '@mui/icons-material';
import { getWebAutomations, createWebAutomation, updateWebAutomation, deleteWebAutomation } from '../services/mockApi.ts';

interface WebAutomation {
  id: number;
  automationName: string;
  websiteUrl: string;
  status: string;
  automationType: string;
  targetElement: string;
  action: string;
  inputData: string;
  outputData: string;
  aiPrompt: string;
  aiResponse: string;
  elementSelector: string;
  errorMessage?: string;
  executionTime?: string;
  browser: string;
  device: string;
  userAgent: string;
  viewportSize: string;
  confidenceScore?: string;
  createdAt: string;
  updatedAt?: string;
}

const WebAutomations: React.FC = () => {
  const [open, setOpen] = useState(false);
  const [editingAutomation, setEditingAutomation] = useState<WebAutomation | null>(null);
  const [formData, setFormData] = useState({
    automationName: '',
    websiteUrl: '',
    automationType: '',
    targetElement: '',
    action: '',
    inputData: '',
    aiPrompt: '',
    browser: 'Chrome',
    device: 'Desktop',
    userAgent: '',
    viewportSize: '1920x1080'
  });

  const queryClient = useQueryClient();

  const { data: webAutomations, isLoading, error } = useQuery('webAutomations', getWebAutomations);

  const createMutation = useMutation(createWebAutomation, {
    onSuccess: () => {
      queryClient.invalidateQueries('webAutomations');
      setOpen(false);
      resetForm();
    }
  });

  const updateMutation = useMutation(updateWebAutomation, {
    onSuccess: () => {
      queryClient.invalidateQueries('webAutomations');
      setOpen(false);
      resetForm();
    }
  });

  const deleteMutation = useMutation(deleteWebAutomation, {
    onSuccess: () => {
      queryClient.invalidateQueries('webAutomations');
    }
  });

  const resetForm = () => {
    setFormData({
      automationName: '',
      websiteUrl: '',
      automationType: '',
      targetElement: '',
      action: '',
      inputData: '',
      aiPrompt: '',
      browser: 'Chrome',
      device: 'Desktop',
      userAgent: '',
      viewportSize: '1920x1080'
    });
    setEditingAutomation(null);
  };

  const handleOpen = () => {
    resetForm();
    setOpen(true);
  };

  const handleEdit = (webAutomation: WebAutomation) => {
    setEditingAutomation(webAutomation);
    setFormData({
      automationName: webAutomation.automationName,
      websiteUrl: webAutomation.websiteUrl,
      automationType: webAutomation.automationType,
      targetElement: webAutomation.targetElement,
      action: webAutomation.action,
      inputData: webAutomation.inputData,
      aiPrompt: webAutomation.aiPrompt,
      browser: webAutomation.browser,
      device: webAutomation.device,
      userAgent: webAutomation.userAgent,
      viewportSize: webAutomation.viewportSize
    });
    setOpen(true);
  };

  const handleSubmit = () => {
    if (editingAutomation) {
      updateMutation.mutate({ ...editingAutomation, ...formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const handleDelete = (id: number) => {
    if (window.confirm('Are you sure you want to delete this web automation?')) {
      deleteMutation.mutate(id);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed': return 'success';
      case 'Failed': return 'error';
      case 'Running': return 'warning';
      case 'Pending': return 'default';
      default: return 'default';
    }
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'automationName', headerName: 'Name', width: 200 },
    { field: 'websiteUrl', headerName: 'Website', width: 200 },
    { 
      field: 'status', 
      headerName: 'Status', 
      width: 120,
      renderCell: (params) => (
        <Chip 
          label={params.value} 
          color={getStatusColor(params.value) as any}
          size="small"
        />
      )
    },
    { field: 'automationType', headerName: 'Type', width: 150 },
    { field: 'action', headerName: 'Action', width: 100 },
    { field: 'browser', headerName: 'Browser', width: 100 },
    { field: 'device', headerName: 'Device', width: 100 },
    { field: 'createdAt', headerName: 'Created', width: 150 },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 150,
      getActions: (params) => [
        <GridActionsCellItem
          icon={<Psychology />}
          label="AI Analyze"
          onClick={() => {/* Handle AI analysis */}}
        />,
        <GridActionsCellItem
          icon={<Edit />}
          label="Edit"
          onClick={() => handleEdit(params.row)}
        />,
        <GridActionsCellItem
          icon={<Delete />}
          label="Delete"
          onClick={() => handleDelete(params.row.id)}
        />
      ]
    }
  ];

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error">
        Error loading web automations. Please try again later.
      </Alert>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">
          System Integration
        </Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleOpen}
        >
          Add Web Automation
        </Button>
      </Box>

      <Grid container spacing={3} mb={3}>
        <Grid item xs={12} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Total Automations
              </Typography>
              <Typography variant="h4">
                {webAutomations?.length || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Completed
              </Typography>
              <Typography variant="h4" color="success.main">
                {webAutomations?.filter(a => a.status === 'Completed').length || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Running
              </Typography>
              <Typography variant="h4" color="warning.main">
                {webAutomations?.filter(a => a.status === 'Running').length || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Failed
              </Typography>
              <Typography variant="h4" color="error.main">
                {webAutomations?.filter(a => a.status === 'Failed').length || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Box sx={{ height: 400, width: '100%' }}>
        <DataGrid
          rows={webAutomations || []}
          columns={columns}
          pageSizeOptions={[5, 10, 25]}
          initialState={{
            pagination: { paginationModel: { pageSize: 10 } }
          }}
          disableRowSelectionOnClick
        />
      </Box>

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingAutomation ? 'Edit Web Automation' : 'Add Web Automation'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 1 }}>
            <TextField
              fullWidth
              label="Automation Name"
              value={formData.automationName}
              onChange={(e) => setFormData({ ...formData, automationName: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Website URL"
              value={formData.websiteUrl}
              onChange={(e) => setFormData({ ...formData, websiteUrl: e.target.value })}
              margin="normal"
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Automation Type</InputLabel>
              <Select
                value={formData.automationType}
                onChange={(e) => setFormData({ ...formData, automationType: e.target.value })}
              >
                <MenuItem value="DataExtraction">Data Extraction</MenuItem>
                <MenuItem value="FormFilling">Form Filling</MenuItem>
                <MenuItem value="Navigation">Navigation</MenuItem>
                <MenuItem value="Clicking">Clicking</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Target Element"
              value={formData.targetElement}
              onChange={(e) => setFormData({ ...formData, targetElement: e.target.value })}
              margin="normal"
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Action</InputLabel>
              <Select
                value={formData.action}
                onChange={(e) => setFormData({ ...formData, action: e.target.value })}
              >
                <MenuItem value="Click">Click</MenuItem>
                <MenuItem value="Type">Type</MenuItem>
                <MenuItem value="Select">Select</MenuItem>
                <MenuItem value="Navigate">Navigate</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Input Data (JSON)"
              value={formData.inputData}
              onChange={(e) => setFormData({ ...formData, inputData: e.target.value })}
              margin="normal"
              multiline
              rows={3}
            />
            <TextField
              fullWidth
              label="AI Prompt"
              value={formData.aiPrompt}
              onChange={(e) => setFormData({ ...formData, aiPrompt: e.target.value })}
              margin="normal"
              multiline
              rows={3}
              placeholder="Describe what you want the AI to do..."
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Browser</InputLabel>
              <Select
                value={formData.browser}
                onChange={(e) => setFormData({ ...formData, browser: e.target.value })}
              >
                <MenuItem value="Chrome">Chrome</MenuItem>
                <MenuItem value="Firefox">Firefox</MenuItem>
                <MenuItem value="Safari">Safari</MenuItem>
                <MenuItem value="Edge">Edge</MenuItem>
              </Select>
            </FormControl>
            <FormControl fullWidth margin="normal">
              <InputLabel>Device</InputLabel>
              <Select
                value={formData.device}
                onChange={(e) => setFormData({ ...formData, device: e.target.value })}
              >
                <MenuItem value="Desktop">Desktop</MenuItem>
                <MenuItem value="Mobile">Mobile</MenuItem>
                <MenuItem value="Tablet">Tablet</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Viewport Size"
              value={formData.viewportSize}
              onChange={(e) => setFormData({ ...formData, viewportSize: e.target.value })}
              margin="normal"
              placeholder="1920x1080"
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            {editingAutomation ? 'Update' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default WebAutomations;
